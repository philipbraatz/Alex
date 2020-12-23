﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Alex.API.Graphics;
using Alex.API.Resources;
using Alex.API.Utils;
using Alex.Graphics.Models.Entity;
using Alex.ResourcePackLib;
using Alex.ResourcePackLib.Json.Bedrock.Entity;
using Alex.ResourcePackLib.Json.Models.Entities;
using Alex.Utils;
using fNbt;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using NLog;
using PlayerLocation = Alex.API.Utils.PlayerLocation;
using ResourceLocation = Alex.API.Resources.ResourceLocation;
using UUID = Alex.API.Utils.UUID;

namespace Alex.Entities
{
	public static class EntityFactory
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(EntityFactory));

		private static ConcurrentDictionary<ResourceLocation, Func<PooledTexture2D, EntityModelRenderer>> _registeredRenderers =
			new ConcurrentDictionary<ResourceLocation, Func<PooledTexture2D, EntityModelRenderer>>();

		private static IReadOnlyDictionary<long, EntityData> _idToData;
		public static void Load(ResourceManager resourceManager, IProgressReceiver progressReceiver)
		{
			progressReceiver?.UpdateProgress(0, "Loading entity data...");

            Dictionary<long, EntityData> networkIdToData = new Dictionary<long, EntityData>();
            EntityData[] entityObjects = JsonConvert.DeserializeObject<EntityData[]>(ResourceManager.ReadStringResource("Alex.Resources.NewEntities.txt"));

            long unknownId = 0;
            for (int i = 0; i < entityObjects.Length; i++)
			{
                EntityData p = entityObjects[i];
                var originalName = p.Name;
                p.OriginalName = originalName;
                p.Name = p.Name.Replace("_", "");
                
                long id = 0;
				progressReceiver?.UpdateProgress(i, entityObjects.Length, "Loading entity data...", p.Name);
				if (resourceManager.Registries.Entities.Entries.TryGetValue($"minecraft:{originalName}",
					out var registryEntry))
                {
                    id = registryEntry.ProtocolId;
					networkIdToData.TryAdd(registryEntry.ProtocolId, p);
                    //networkIdToData.TryAdd(p.InternalId + 1, p);
                }
				else
				{
					Log.Warn($"Could not resolve {p.Name}'s protocol id!");
                    id = unknownId++;
                }
			}

			_idToData = networkIdToData;
        }

		public static bool ModelByNetworkId(long networkId, out EntityModelRenderer renderer, out EntityData data)
		{
			if (_idToData.TryGetValue(networkId, out data))
			{
				renderer = TryGetRendererer(data, null);
				if (renderer != null)
				{
					return true;
                }
				else
				{
				//	if (data.OriginalName.Equals("armor_stand"))
						Log.Warn($"No entity model renderer found for {data.Name} - {data.OriginalName}");
				}
			}

			renderer = null;
			return false;
		}

		private static EntityModelRenderer TryGetRendererer(EntityData data, PooledTexture2D texture)
		{
			string lookupName = data.OriginalName;

			if (lookupName == "firework_rocket")
			{
				lookupName = "fireworks_rocket";
			}
			
			if (_registeredRenderers.TryGetValue(lookupName, out var func))
			{
				return func(texture);
			}
			else
			{
				var f = _registeredRenderers.Where(x => x.Key.Path.Length >= data.OriginalName.Length)
				   .OrderBy(x => (x.Key.Path.Length - data.OriginalName.Length)).FirstOrDefault(
						x => x.Key.ToString().ToLowerInvariant().Contains(data.OriginalName.ToLowerInvariant())).Value;

				if (f != null)
				{
					return f(texture);
				}
			}

			return null;
		}

		public static EntityModelRenderer GetEntityRenderer(string name, PooledTexture2D texture)
		{
			if (_registeredRenderers.TryGetValue(name, out var func))
			{
				if (func != null) return func(texture);
			}
			else
			{
				var f = _registeredRenderers.FirstOrDefault(x => x.Key.ToString().ToLowerInvariant().Contains(name.ToLowerInvariant())).Value;

				if (f != null)
				{
					return f(texture);
				}
			}
			return null;
		}
		
		public static void LoadModels(ResourceManager resourceManager, GraphicsDevice graphics, bool replaceModels, IProgressReceiver progressReceiver = null)
		{
			var entityDefinitions = resourceManager.BedrockResourcePack.EntityDefinitions;
			int done = 0;
			int total = entityDefinitions.Count;

			foreach (var def in entityDefinitions)
			{
			//	double percentage = 100D * ((double)done / (double)total);
				progressReceiver?.UpdateProgress(done, total, $"Importing entity definitions...", def.Key.ToString());

                try
				{
					if (def.Value.Textures == null) continue;
					if (def.Value.Geometry == null) continue;
					if (def.Value.Textures.Count == 0) continue;
					if (def.Value.Geometry.Count == 0) continue;

					var geometry = def.Value.Geometry;
					string modelKey;
					if (!geometry.TryGetValue("default", out modelKey) && !geometry.TryGetValue(new ResourceLocation(def.Value.Identifier).Path, out modelKey))
					{
						modelKey = geometry.FirstOrDefault().Value;
					}

					EntityModel model;
					if (ModelFactory.TryGetModel(modelKey + ".v1.8", out model) && model != null)
					{
						Add(resourceManager, graphics, def.Value, model, def.Value.Identifier);
						Add(resourceManager, graphics, def.Value, model, def.Key.ToString());
					}
				    else if (ModelFactory.TryGetModel(modelKey, out model) && model != null)
				    {
				        Add(resourceManager, graphics, def.Value, model, def.Value.Identifier);
				        Add(resourceManager, graphics, def.Value, model, def.Key.ToString());
                    }
				}
				catch (Exception ex)
				{
					Log.Warn(ex, $"Failed to load model {def.Key}!");
				}
				finally
                {
	                done++;
                }
			}

			if (_registeredRenderers.TryGetValue("minecraft:armor_stand", out var func))
				_registeredRenderers.TryAdd("minecraft:armorstand", func);
			
		//    Log.Info($"Registered {(Assembly.GetExecutingAssembly().GetTypes().Count(t => t.Namespace == "Alex.Entities.Models"))} entity models");
		    Log.Info($"Registered {_registeredRenderers.Count} entity model renderers");
        }

		private static void Add(ResourceManager resourceManager, GraphicsDevice graphics, EntityDescription def, EntityModel model, ResourceLocation name)
		{
			_registeredRenderers.AddOrUpdate(name,
				(t) =>
				{
					if (t == null)
					{
						var textures = def.Textures;
						string texture;
						if (!textures.TryGetValue("default", out texture) && !textures.TryGetValue(name.Path, out texture))
						{
							texture = textures.FirstOrDefault().Value;
						}

						if (resourceManager.BedrockResourcePack.Textures.TryGetValue(texture,
							out var bmp))
						{
							t = TextureUtils.BitmapToTexture2D(graphics, bmp);
						}
					}

					return new EntityModelRenderer(model, t);
				},
				(s, func) =>
				{
					return (t) =>
					{
						var textures = def.Textures;
						string texture;
						if (!(textures.TryGetValue("default", out texture) || textures.TryGetValue(name.Path, out texture)))
						{
							texture = textures.FirstOrDefault().Value;
						}

						if (resourceManager.BedrockResourcePack.Textures.TryGetValue(texture,
							out var bmp))
						{
							t = TextureUtils.BitmapToTexture2D(graphics, bmp);
						}

						return new EntityModelRenderer(model, t);
					};
				});
		}
	}
}
