﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Alex.Common;
using Alex.Common.Graphics;
using Alex.Common.Services;
using Alex.Common.Utils;
using Alex.Common.Utils.Vectors;
using Alex.Gamestates.Common;
using Alex.Gamestates.InGame.Hud;
using Alex.Graphics.Camera;
using Alex.Gui;
using Alex.Gui.Elements;
using Alex.Items;
using Alex.Net;
using Alex.Net.Bedrock;
using Alex.ResourcePackLib.Json.Models.Items;
using Alex.Services.Discord;
using Alex.Utils;
using Alex.Worlds;
using Alex.Worlds.Abstraction;
using Alex.Worlds.Chunks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiNET.Utils;
using NLog;
using RocketUI;
using GpuResourceManager = Alex.Common.Graphics.GpuResources.GpuResourceManager;
using PlayerLocation = Alex.Common.Utils.Vectors.PlayerLocation;

namespace Alex.Gamestates.InGame
{
	public class PlayingState : GuiGameStateBase
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(PlayingState));
		
		public World World { get; private set; }

        private WorldProvider WorldProvider { get; set; }
        private NetworkProvider NetworkProvider { get; set; }

		private readonly PlayingHud _playingHud;
		private readonly GuiDebugInfo _debugInfo;
		private readonly NetworkDebugHud _networkDebugHud;
		
		public PlayingState(Alex alex, GraphicsDevice graphics, WorldProvider worldProvider, NetworkProvider networkProvider) : base()
		{
			NetworkProvider = networkProvider;

			World = new World(alex.Services, graphics, Options, networkProvider);
			World.Player.IsFirstPersonMode = true;
			
			WorldProvider = worldProvider;
			var title = new TitleComponent();

			WorldProvider = worldProvider;
			WorldProvider.Init(World);

			Alex.ParticleManager.Initialize(World.Camera);

			WorldProvider.TitleComponent = title;

			_playingHud = new PlayingHud(Alex, World, title, networkProvider);
			
			WorldProvider.ScoreboardView = _playingHud.Scoreboard;
			WorldProvider.ChatRecipient = _playingHud;
			WorldProvider.BossBarContainer = _playingHud.BossBar;
			//WorldProvider.ScoreboardView
			
			_debugInfo = new GuiDebugInfo();
            InitDebugInfo();
            
            _networkDebugHud = new NetworkDebugHud(NetworkProvider);
            RenderNetworking = Options.MiscelaneousOptions.ShowNetworkInfoByDefault.Value;
            //Alex.Instance.l
            World.TickManager.RegisterTicked(WorldProvider);
		}

		protected override void OnLoad(IRenderArgs args)
		{
			Alex.InGame = true;
			
			World.SpawnPoint = WorldProvider.GetSpawnPoint();
			World.Camera.MoveTo(World.SpawnPoint, Vector3.Zero);

			base.OnLoad(args);
		}

		private TimeSpan _targetElapsed = TimeSpan.Zero;
		protected override void OnShow()
		{
			var world = World;
			if (world == null)
				return;
			
			var player = World?.Player;
			if (player == null)
				return;
			
			_targetElapsed = Alex.TotalTimeSpan.Add(TimeSpan.FromMilliseconds(500));
			
			Alex.IsMouseVisible = false;
			player?.SkipUpdate();
			
			//if (RenderNetworking) 
			Alex.GuiManager.AddScreen(_networkDebugHud);
			
			base.OnShow();
			Alex.GuiManager.AddScreen(_playingHud);

			if (RenderDebug)
				Alex.GuiManager.AddScreen(_debugInfo);

			world.TickManager.RegisterTicked(_playingHud.Title);
			_playingHud.Title.Ready();
			Alex.ResetFrameRateLimiter();
		}

		protected override void OnHide()
		{
			var world = World;
			if (world == null)
				return;
	
			world.TickManager.UnregisterTicked(_playingHud.Title);
			
			if (RenderDebug)
				Alex.GuiManager.RemoveScreen(_debugInfo);
			
			Alex.GuiManager.RemoveScreen(_playingHud);
			Alex.GuiManager.RemoveScreen(_networkDebugHud);
			
			base.OnHide();
		}
		
		private bool RenderDebug { get; set; } = false;
		
		private bool RenderNetworking
		{
			get
			{
				return _networkDebugHud.Advanced;
			}
			set
			{
				if (value != _networkDebugHud.Advanced)
				{
					_networkDebugHud.Advanced = value;
				}
			}
		}
		
		private KeyboardState _oldKeyboardState;
		protected void CheckInput(GameTime gameTime)
		{
			KeyboardState currentKeyboardState = Keyboard.GetState();
			if (currentKeyboardState != _oldKeyboardState)
			{
				if (KeyBinds.NetworkDebugging.All(x => currentKeyboardState.IsKeyDown(x)))
				{
					RenderNetworking = !RenderNetworking;
				}
				else if (KeyBinds.EntityBoundingBoxes.All(x => currentKeyboardState.IsKeyDown(x)))
				{
					World.RenderBoundingBoxes = !World.RenderBoundingBoxes;
				}
				else if (currentKeyboardState.IsKeyDown(KeyBinds.DebugInfo))
				{
					RenderDebug = !RenderDebug;
					if (!RenderDebug)
					{
						Alex.GuiManager.RemoveScreen(_debugInfo);
					}
					else
					{
						Alex.GuiManager.AddScreen(_debugInfo);
					}
				}

				if (currentKeyboardState.IsKeyDown(KeyBinds.Fog) && !_oldKeyboardState.IsKeyDown(KeyBinds.Fog))
				{
					World.ChunkManager.FogEnabled = !World.ChunkManager.FogEnabled;
				}

				if (currentKeyboardState.IsKeyDown(KeyBinds.ToggleWireframe))
				{
					World.ToggleWireFrame();
				}
			}

			_oldKeyboardState = currentKeyboardState;
		}

		private long _ramUsage = 0;
		private Biome _currentBiome = BiomeUtils.GetBiome(0);
		private int _currentBiomeId = 0;
		private void InitDebugInfo()
		{
			string gameVersion = VersionUtils.GetVersion();

			_debugInfo.AddDebugLeft(
				() => $"Alex {gameVersion} ({Alex.FpsMonitor.Value:##} FPS, {World.TickManager.TicksPerSecond:##} TPS, Chunk Updates: {World.ChunkManager.EnqueuedChunkUpdates} queued, {World.ChunkManager.ConcurrentChunkUpdates} active)", TimeSpan.FromMilliseconds(50));
			
			_debugInfo.AddDebugLeft(() =>
			{
				var pos = World?.Player?.KnownPosition ?? new PlayerLocation();
				var blockPos = pos.GetCoordinates3D();
				return $"Position: (X={pos.X:F2}, Y={pos.Y:F2}, Z={pos.Z:F2}, OnGround={pos.OnGround}) / Block: ({blockPos.X:D}, {blockPos.Y:D}, {blockPos.Z:D})";
			}, TimeSpan.FromMilliseconds(50));
			
			_debugInfo.AddDebugLeft(() =>
			{
				var pos =  World?.Player?.KnownPosition ?? new PlayerLocation();
				return  $"Facing: {pos.GetCardinalDirection()} (HeadYaw={pos.HeadYaw:F2}, Yaw={pos.Yaw:F2}, Pitch={pos.Pitch:F2})";
			}, TimeSpan.FromMilliseconds(50));
			
			_debugInfo.AddDebugLeft(() =>
			{
				var pos = World?.Player?.Velocity ?? Vector3.Zero;
				return $"Velocity: (X={pos.X:F2}, Y={pos.Y:F2}, Z={pos.Z:F2}) ({World.Player.Movement.MetersPerSecond:F3} m/s)";// / Target Speed: {(World.Player.CalculateMovementSpeed() * 20f):F3} m/s";
			});

			_debugInfo.AddDebugLeft(() => $"Primitives: {Alex.Metrics.PrimitiveCount:N0} Draw count: {Alex.Metrics.DrawCount}", TimeSpan.FromMilliseconds(500));
			_debugInfo.AddDebugLeft(() => $"Textures: {Alex.Metrics.TextureCount:N0} Sprite count: {Alex.Metrics.SpriteCount}", TimeSpan.FromMilliseconds(500));
			_debugInfo.AddDebugLeft(() => $"Graphic Resources: {GpuResourceManager.ResourceCount}", TimeSpan.FromMilliseconds(500));

			_debugInfo.AddDebugLeft(() => $"Chunks: {World.ChunkManager.ChunkCount}, {World.ChunkManager.RenderedChunks}, {World.ChunkDrawCount}", TimeSpan.FromMilliseconds(500));
			_debugInfo.AddDebugLeft(() => $"Entities: {World.EntityManager.EntityCount}, {World.EntityManager.EntitiesRendered}, {World.EntityManager.DrawCount}", TimeSpan.FromMilliseconds(500));
			_debugInfo.AddDebugLeft(() => $"Particles: {Alex.ParticleManager.ParticleCount}", TimeSpan.FromMilliseconds(500));
			_debugInfo.AddDebugLeft(() =>
			{
				return $"Biome: {_currentBiome.Name} ({_currentBiomeId})";
			}, TimeSpan.FromMilliseconds(500));
			//_debugInfo.AddDebugLeft(() => { return $"Do DaylightCycle: {World.DoDaylightcycle}"; });

			_debugInfo.AddDebugLeft(
				() =>
				{
					return $"Exhaustion: {World.Player.HealthManager.Exhaustion:F1}/{World.Player.HealthManager.MaxExhaustion}";
				}, TimeSpan.FromMilliseconds(250));
			
			_debugInfo.AddDebugLeft(
				() =>
				{
					return $"Saturation: {World.Player.HealthManager.Saturation:F1}/{World.Player.HealthManager.MaxSaturation}";
				}, TimeSpan.FromMilliseconds(250));
			
			_debugInfo.AddDebugLeft(
				() =>
				{
					return $"Health: {World.Player.HealthManager.Health:F1}/{World.Player.HealthManager.MaxHealth}";
				}, TimeSpan.FromMilliseconds(250));
			
			_debugInfo.AddDebugLeft(
				() =>
				{
					return $"Gamemode: {World.Player.Gamemode}";
				}, TimeSpan.FromSeconds(5));
			
			_debugInfo.AddDebugLeft(
				() =>
				{
					var effects = World.Player.Effects.AppliedEffects().ToArray();
					return $"Applied Effects ({effects.Length}): {string.Join('\n',effects.Select(x => x.EffectId.ToString()))}";
				}, TimeSpan.FromSeconds(1));
			
			_debugInfo.AddDebugRight(Alex.OperatingSystem);
			_debugInfo.AddDebugRight(Alex.Gpu);
			_debugInfo.AddDebugRight($"{Alex.DotnetRuntime}\n");
			_debugInfo.AddDebugRight(Alex.RenderingEngine);
			
			_debugInfo.AddDebugRight(() => $"RAM: {FormattingUtils.GetBytesReadable(_ramUsage, 2)}", TimeSpan.FromMilliseconds(1000));
			_debugInfo.AddDebugRight(() => $"GPU: {FormattingUtils.GetBytesReadable(GpuResourceManager.MemoryUsage, 2)}", TimeSpan.FromMilliseconds(1000));
			_debugInfo.AddDebugRight(() => $"Threads: {(ThreadPool.ThreadCount):00}/{Environment.ProcessorCount:00}\nPending: {ThreadPool.PendingWorkItemCount:00}\n", TimeSpan.FromMilliseconds(50));
			
			_debugInfo.AddDebugRight(() => $"Updates: {ChunkColumn.AverageUpdateTime:F2}ms avg\nUpload: {ChunkData.AverageUploadTime:F2}ms avg\n", TimeSpan.FromMilliseconds(50));
			_debugInfo.AddDebugRight(() => $"UI Tasks: {Alex.UiTaskManager.Pending:00}\nR: {Alex.UiTaskManager.AverageExecutionTime:F2}ms\nQ: {Alex.UiTaskManager.AverageTimeTillExecution:F2}ms", TimeSpan.FromMilliseconds(50));
			_debugInfo.AddDebugRight(() => $"IsRunningSlow: {Alex.FpsMonitor.IsRunningSlow}");
			_debugInfo.AddDebugRight(() =>
			{
				var player = World?.Player;

				if (player == null)
					return "";
				
				if (player.Raytracer.HasValue)
				{
					var raytracedBlock = player.Raytracer.ResultingCoordinates;
					var raytracedFace = player.Raytracer.Face;
					var adjacentBlock  = player.Raytracer.AdjacentBlockCoordinates;
					//var adj             =  Vector3.Floor(adjacentBlock) - Vector3.Floor(raytracedBlock);
					//adj.Normalize();

				//	var face = adj.GetBlockFace();

                    StringBuilder sb = new StringBuilder();
					sb.AppendLine($"Target: {raytracedBlock} Face: {raytracedFace}");
					sb.AppendLine(
						$"Skylight: {World.GetSkyLight(raytracedBlock)} Face Skylight: {World.GetSkyLight(adjacentBlock)}");
					sb.AppendLine(
						$"Blocklight: {World.GetBlockLight(raytracedBlock)} Face Blocklight: {World.GetBlockLight(adjacentBlock)}");

					foreach (var bs in World
						.GetBlockStates((int) raytracedBlock.X, (int) raytracedBlock.Y, (int) raytracedBlock.Z))
					{
						var blockstate = bs.State;
						if (blockstate != null && blockstate.Block.HasHitbox)
						{
							sb.AppendLine($"{blockstate.Name} (S: {bs.Storage})");
							var dict = blockstate.ToArray();

							if (dict.Length > 0)
							{
								sb.AppendLine();
								sb.AppendLine("Blockstate:");

								foreach (var kv in dict)
								{
									sb.AppendLine(kv.ToFormattedString());
								}
							}
						}
					}

					return sb.ToString();
				}
				else
				{
					return string.Empty;
				}
			}, TimeSpan.FromMilliseconds(500));
			
			_debugInfo.AddDebugRight(() =>
			{
				var player = World.Player;
				if (player == null || player.HitEntity == null) return string.Empty;

				var entity = player.HitEntity;
				return $"Hit entity: {entity.EntityId} / {entity.ToString()}\n{ChatFormatting.Reset}Hide nametag: {!entity.HideNameTag}\nNoAI: {entity.NoAi}\nHas Gravity: {entity.IsAffectedByGravity}\nFlying: {entity.IsFlying}\nOn Ground: {entity.KnownPosition.OnGround}\nHas Collisions: {entity.HasCollision}\nHas Model: {entity.ModelRenderer != null}\nHas Texture: {entity.Texture != null} (2nd={entity.ModelRenderer?.Texture != null})\nScale: {entity.Scale}\nTextureSize: {entity.ModelRenderer?.TextureSize}\n";
			}, TimeSpan.FromMilliseconds(500));
			
		/*	_debugInfo.AddDebugRight(
				() =>
				{
					var modelRenderer = World?.Player?.ModelRenderer?.Model;

					if (modelRenderer == null)
						return string.Empty;
					
					StringBuilder sb = new StringBuilder();

					foreach (var bone in modelRenderer.Bones)
					{
						sb.AppendLine($"{bone.Name}");
						sb.AppendLine($"Rotation (X: {(bone.Rotation.X):F3} Y: {bone.Rotation.Y:F3} Z: {bone.Rotation.Z:F3})");
						sb.AppendLine($"Position (X: {(bone.Position.X):F3} Y: {bone.Position.Y:F3} Z: {bone.Position.Z:F3})");
						sb.AppendLine();
					}
					
					return sb.ToString();
				});*/
		}

		private float AspectRatio { get; set; }

		private DateTime _previousMemUpdate = DateTime.UtcNow;

		protected override void OnUpdate(GameTime gameTime)
		{
			if (World?.Camera == null)
				return;

			var world = World;
			var player = world?.Player;

			if (world == null || player == null)
				return;
			
			var graphics = Alex.GraphicsDevice;
			var args = new UpdateArgs() {Camera = world.Camera, GraphicsDevice = graphics, GameTime = gameTime};

			if (Alex.TotalTimeSpan < _targetElapsed)
				player.SkipUpdate();
			
			_playingHud.CheckInput = Alex.GuiManager.ActiveDialog == null;

			//	if (Alex.IsActive)

			if (Math.Abs(AspectRatio - graphics.Viewport.AspectRatio) > 0f)
			{
				world.Camera.UpdateAspectRatio(graphics.Viewport.AspectRatio);
				AspectRatio = graphics.Viewport.AspectRatio;
			}

			bool hasActiveDialog = Alex.Instance.GuiManager.ActiveDialog != null
			                       || ((player.Network is BedrockClient c)
			                           && c.WorldProvider.FormManager.IsShowingForm);
			
			player.Controller.CheckMovementInput = Alex.IsActive && !hasActiveDialog;
			if (!_playingHud.Chat.Focused && Alex.GameStateManager.GetActiveState() is PlayingState)
			{
				player.Controller.CheckInput = Alex.IsActive;

				if (Alex.GuiManager.ActiveDialog == null)
				{
					CheckInput(gameTime);
				}
			}
			else
			{
				player.Controller.CheckInput = false;
			}

			world.Update(args);

			var now = DateTime.UtcNow;

			if (now - _previousMemUpdate > TimeSpan.FromSeconds(5))
			{
				_previousMemUpdate = now;
				_ramUsage = Environment.WorkingSet;

				var pos     = player.KnownPosition.GetCoordinates3D();
				var biomeId = world.GetBiome(pos.X, pos.Y, pos.Z);
				var biome   = BiomeUtils.GetBiome(biomeId);
				_currentBiomeId = biomeId;
				_currentBiome = biome;
			}

			var dir = world.Camera.Position - World.Camera.Target;
			dir.Normalize();
			//dir = new Vector3(MathF.Round(dir.X), MathF.Round(dir.Y), MathF.Round(dir.Z));

			// Calculate the direction vector.
			var direction = Vector3.Normalize(world.Camera.Target - world.Camera.Position );

			// Calculate the angle between direction and forward on XZ.
			var xzAngle = MathF.Acos(Vector2.Dot(
				new Vector2( Vector3.Forward.X, Vector3.Forward.Z),
				new Vector2( direction.X, direction.Z)));

			// Rotate about up.
			var rotationY = float.IsNaN( xzAngle )
				? Quaternion.Identity
				: Quaternion.CreateFromAxisAngle( Vector3.Up, xzAngle );

			// Get rotation axis.
			var rotatedForward = Vector3.Transform( Vector3.Forward, rotationY );

			//dir.Normalize();
			Alex.AudioEngine.Update(gameTime, world.Camera.Position, Vector3.Normalize(rotatedForward));
			
			//Alex.ParticleManager.Update(gameTime);
			
			//base.OnUpdate(gameTime);
		}
		
		protected override void OnDraw(IRenderArgs args)
		{
			var world = World;
			var player = world?.Player;

			if (world == null || player == null)
				return;
			
			if (world?.Camera == null)
				return;

			args.Camera = world?.Camera;

			world?.Render(args);

			world?.RenderSprites(args);
		}

		protected override void OnUnload()
		{
			Alex.InGame = false;
			Alex.ParticleManager.Hide();

			LoadingOverlay loadingOverlay = new LoadingOverlay();
			loadingOverlay.Text = "Disconnecting...";
			Alex.GuiManager.AddScreen(loadingOverlay);
			
			ThreadPool.QueueUserWorkItem(
				o =>
				{
					try
					{
						NetworkProvider?.Close();
						NetworkProvider = null;

						World?.Dispose();
						World = null;

						WorldProvider?.Dispose();
						WorldProvider = null;

						_playingHud?.Unload();

						loadingOverlay.Text = "Reloading resources...";
						Alex.Resources.ReloadBedrockResources(loadingOverlay);
						
						RichPresenceProvider.ClearPresence();
						GC.Collect();
					}
					catch (Exception ex)
					{
						Log.Warn(ex, $"Failed to close session!");
					}
					finally
					{
						Alex.GuiManager.RemoveScreen(loadingOverlay);
					}
				});

			
			//GetService<IEventDispatcher>().UnregisterEvents(_playingHud.Chat);
			//_playingHud.Chat = 
		}
	}
}
