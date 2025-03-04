using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Alex.Common.Graphics;
using Alex.Common.Graphics.GpuResources;
using Alex.Common.Utils;
using Alex.Common.Utils.Collections;
using Alex.MoLang.Parser;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;
using Alex.MoLang.Utils;
using Alex.ResourcePackLib.Json.Bedrock.Particles;
using Alex.ResourcePackLib.Json.Bedrock.Particles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace Alex.Particles
{
	public class ParticleEmitter : IDisposable
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(ParticleEmitter));
		private ThreadSafeList<ParticleInstance> _instances = new ThreadSafeList<ParticleInstance>();
		private ParticleInstance[] _activeInstances = new ParticleInstance[0];
		
		private Texture2D Texture { get; }

		private AppearanceComponent AppearanceComponent { get; }
		public int MaxParticles { get; set; } = 500;
		public ParticleDefinition Definition { get; }
		public ParticleEmitter(Texture2D texture, ParticleDefinition definition)
		{
			Texture = texture;
			Definition = definition;

			MaxParticles = definition.MaxParticles;
			if (definition.Components.TryGetValue("minecraft:particle_appearance_billboard", out var value)
			    && value is AppearanceComponent apc)
			{
				AppearanceComponent = apc;
			}
			else
			{
				AppearanceComponent = null;
			}

		}

		public void Reset()
		{
			_instances.Clear();
		}

		public bool Spawn(Vector3 position, long data, ParticleDataMode dataMode, out ParticleInstance instance)
		{
			if (AppearanceComponent == null)
			{
				instance = null;
				return false;
			}

			MoLangRuntime runtime = new MoLangRuntime();

			instance = new ParticleInstance(this, runtime, position);

			runtime.Environment.Structs.TryAdd("query", instance);
			instance.SetData(data, dataMode);

			foreach (var component in Definition.Components)
			{
				component.Value?.OnCreate(instance, runtime);
			}
			
			_instances.Add(instance);

			return true;
		}
		
		private static readonly MoPath _emitterRnd1 = new MoPath("emitter_random_1");
		private static readonly MoPath _emitterRnd2 = new MoPath("emitter_random_2");
		private static readonly MoPath _emitterRnd3 = new MoPath("emitter_random_3");
		private static readonly MoPath _emitterRnd4 = new MoPath("emitter_random_4");
		public void Tick()
		{
			if (_instances.Count == 0)
				return;

			var rnd1 = FastRandom.Instance.NextDouble();
			var rnd2 = FastRandom.Instance.NextDouble();
			var rnd3 = FastRandom.Instance.NextDouble();
			var rnd4 = FastRandom.Instance.NextDouble();

			List<ParticleInstance> toRemove = new List<ParticleInstance>();

			foreach (var instance in _instances)
			{
				if (instance == null)
					continue;
				if (instance.Lifetime >= instance.MaxLifetime)
				{
					toRemove.Add(instance);
				}
				else
				{
					var variables = instance.Runtime.Environment.Structs["variable"];
					variables.Set(_emitterRnd1, new DoubleValue(rnd1));
					variables.Set(_emitterRnd2, new DoubleValue(rnd2));
					variables.Set(_emitterRnd3, new DoubleValue(rnd3));
					variables.Set(_emitterRnd4, new DoubleValue(rnd4));

					foreach (var component in Definition.Components)
					{
						component.Value.Update(instance, instance.Runtime);
						component.Value.PreRender(instance, instance.Runtime);
					}

					instance.OnTick();
				}
			}

			foreach (var removed in toRemove)
				_instances.Remove(removed);

			_activeInstances = _instances.ToArray();
		}
		
		public void Update(GameTime gameTime)
		{
			var instances = _activeInstances;
			foreach (var instance in instances)
			{
				instance?.Update(gameTime);
			}
		}

		public int Draw(GameTime gameTime, SpriteBatch spriteBatch, ICamera camera)
		{
			int count = 0;

			var instances = _activeInstances;
			foreach (var instance in instances)
			{
				if (instance  == null || count >= MaxParticles)
					continue;

				var pos = instance.Position;

				//var scale = 1f - (Vector3.DistanceSquared(camera.Position, pos) / camera.FarDistance);
				//if (scale <= 0f)
				//	continue;
				
				var screenSpace = spriteBatch.GraphicsDevice.Viewport.Project(
					pos, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
				
				bool isOnScreen = spriteBatch.GraphicsDevice.Viewport.Bounds.Contains((int) screenSpace.X, (int) screenSpace.Y);

				if (!isOnScreen) continue;
				
				count++;
				float depth = screenSpace.Z;
				float scale =  1f - (Vector3.Distance(camera.Position, pos) / camera.FarDistance);// 1.0f / depth;
				if (scale <= 0f)
					continue;

				if (scale > 1f)
					scale = 1f;
				
				Vector2 textPosition;
				textPosition.X = screenSpace.X;
				textPosition.Y = screenSpace.Y;

				var textureLocation = instance.UvPosition;
				var textureSize = instance.UvSize;

				spriteBatch.Draw(
					Texture, textPosition, new Rectangle(textureLocation.ToPoint(), textureSize.ToPoint()),
					instance.Color, 0f, Vector2.Zero,
					//2f * ((scale)),
					new Vector2( scale * instance.Size.X, scale * instance.Size.Y) * 16f,
					SpriteEffects.None, depth);
			}

			return count;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Reset();
			
			//if (Texture != null)
			//{
			//	if (!Texture.IsDisposed)
				//	Texture.Dispose();
			//}
		}
	}
}