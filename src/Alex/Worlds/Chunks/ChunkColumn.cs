﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Alex.Blocks;
using Alex.Blocks.State;
using Alex.Common.Utils;
using Alex.Common.Utils.Vectors;
using Alex.Worlds.Abstraction;
using Alex.Worlds.Singleplayer;
using fNbt;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace Alex.Worlds.Chunks
{
	public class ChunkColumn
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(ChunkColumn));
		
		public const int ChunkWidth = 16;
		public const int ChunkDepth = 16;

		public int X { get; set; }
		public int Z { get; set; }
		
		public           bool           IsNew           { get; set; } = true;
		public           ChunkSection[] Sections { get; set; }
		private readonly int[] _biomeId;
		private readonly  short[]        _height  = new short[256];
		
		public  object                                              UpdateLock       { get; set; } = new object();
		public ConcurrentDictionary<BlockCoordinates, NbtCompound> BlockEntities    { get; }
		//public  NbtCompound[]                                       GetBlockEntities => BlockEntities.ToArray();

		internal ChunkData ChunkData { get; private set; }
		private object _dataLock = new object();

		//private System.Collections.BitArray _scheduledUpdates;
		private int _scheduledUpdateCount = 0;
		public WorldSettings WorldSettings { get; }
		private readonly int _sectionOffset;
		public ChunkColumn(int x, int z, WorldSettings worldSettings)
		{
			X = x;
			Z = z;
			WorldSettings = worldSettings;
			_sectionOffset = worldSettings.MinY < 0 ? Math.Abs(worldSettings.MinY >> 4) : 0;

			int realHeight = worldSettings.WorldHeight + Math.Abs(worldSettings.MinY);
			
			Sections = new ChunkSection[realHeight / 16];
			for (int i = 0; i < Sections.Length; i++)
			{
				Sections[i] = null;
			}

			BlockEntities = new ConcurrentDictionary<BlockCoordinates, NbtCompound>();
			//_scheduledUpdates = new System.Collections.BitArray((16 * 16 * realHeight), false);
			_biomeId = new int[16 * 16 * realHeight];

			ChunkData = ChunkData.Create(x, z);// new ChunkData(x,z);
		}

		public ChunkColumn(int x, int z) : this(x,z, WorldSettings.Default)
		{
			
		}

		private bool CheckWithinCoordinates(int x, int y, int z, bool throwException = true)
		{
			if (y < WorldSettings.MinY || y >= WorldSettings.WorldHeight)
			{
				if (throwException)
					throw new Exception(
						$"Y level is out side of support range. (Min: {WorldSettings.MinY} Max: {WorldSettings.WorldHeight} Value: {y})");

				return false;
			}

			return true;
		}
		
		protected void SetScheduled(int x, int y, int z, bool value)
		{
			if (!CheckWithinCoordinates(x, y, z, false))
				return;
			
			/*var queue = _scheduledUpdates;

			if (queue != null)
			{
				var wasQueued = queue[GetCoordinateIndex(x, y, z)];
				queue[GetCoordinateIndex(x, y, z)] = value;

				if (wasQueued && !value)
				{
					Interlocked.Decrement(ref _scheduledUpdateCount);
				}else if (!wasQueued && value)
				{
					Interlocked.Increment(ref _scheduledUpdateCount);
				}
			}*/
		}

		public void ScheduleBorder(ChunkCoordinates neighbor)
		{
			int? validX = null;
			int? validZ = null;

			if (neighbor.X == X + 1)
			{
				validX = 15;
			}
			else if (neighbor.X == X - 1)
			{
				validX = 0;
			}
			
			if (neighbor.Z == Z + 1)
			{
				validZ = 15;
			}
			else if (neighbor.Z == Z - 1)
			{
				validZ = 0;
			}
			
			for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++)
				{
					for (int y = WorldSettings.MinY; y < WorldSettings.WorldHeight; y++)
					{
						if (validX.HasValue && x == validX.Value)
						{
							SetScheduled(x,y,z, true);
						}
						else if (validZ.HasValue && z == validZ.Value)
						{
							SetScheduled(x,y,z, true);
						}
						if (x == 0 || x == 15 || z == 0 || z == 15)
						{
							//SetScheduled(x,y,z, true);
						}
					}
				}
			}
		}

		public static float AverageUpdateTime => MovingAverage.Average;
		public static float MaxUpdateTime => MovingAverage.Maximum;
		public static float MinUpdateTime => MovingAverage.Minimum;
		public bool Scheduled { get; set; } = false;

		private static readonly MovingAverage MovingAverage = new MovingAverage();

		public bool UpdateBuffer(IBlockAccess world, bool applyChanges)
		{
			//Monitor.Enter(_dataLock);
			if (!Monitor.TryEnter(_dataLock, 0))
				return false;

			Stopwatch time = Stopwatch.StartNew();
		
			try
			{
				var chunkData = ChunkData;
			//	chunkData.Buffer = 

				if (chunkData == null)
					return false;

				//var scheduleQueue = _scheduledUpdates;

			//	if (scheduleQueue == null)
				//	return false;
				
				//bool isNew = IsNew;
				//bool didChange = false;
				world = new OffsetBlockAccess(new BlockCoordinates(X << 4, 0, Z << 4), world);
				
				for (int sectionIndex = 0; sectionIndex < Sections.Length; sectionIndex++)
				{
					var section = Sections[sectionIndex];

					if (section == null)
						continue;

					var si = sectionIndex;
					si -= _sectionOffset;
					
					var yOffset = (si << 4);
					for (int x = 0; x < 16; x++)
					{
						for (int z = 0; z < 16; z++)
						{
							for (int y = 0; y < 16; y++)
							{
								var idx = GetCoordinateIndex(x, yOffset + y, z);

								//bool scheduled = scheduleQueue[idx]; // IsScheduled(x, y, z)

								//if ((!isNew && !scheduled))
								//	continue;
								
								var blockPosition = new BlockCoordinates(x, yOffset + y, z);
								//if (scheduled)
								{
								//	Interlocked.Decrement(ref _scheduledUpdateCount);
								//	scheduleQueue[idx] = false;
									//didChange = true;
								}
								//chunkData?.Remove(blockPosition);

								for (int storage = 0; storage < section.StorageCount; storage++)
								{
									var blockState = section.Get(x, y, z, storage);

									if (blockState == null || blockState?.VariantMapper?.Model == null
									                       || blockState.Block == null || !blockState.Block.Renderable)
										continue;

									var model = blockState.VariantMapper.Model;

									if ((blockState.Block.RequiresUpdate
									              || blockState.VariantMapper.IsMultiPart))
									{
										var newblockState = blockState.Block.BlockPlaced(
											world, blockState, blockPosition);

										if (blockState.ID != newblockState.ID)
										{
											blockState = newblockState;

											section.Set(storage, x, y, z, blockState);
											model = blockState?.VariantMapper?.Model;
										}
									}

									if (model != null)
									{
										model.GetVertices(world, chunkData, blockPosition, blockState);
									}
								}

							}
						}
					}
				}

				//if (applyChanges && (didChange || isNew))
				//{
				if (!Destroyed && !chunkData.Disposed)
				{
					chunkData.ApplyChanges(world);
				}
				else
				{
					return false;
				}
				
				ChunkData = chunkData;
				IsNew = false;
			}
			finally
			{
				//_previousKeepInMemory = keepInMemory;
				Monitor.Exit(_dataLock);
				time.Stop();
				
				MovingAverage.ComputeAverage((float) time.Elapsed.TotalMilliseconds);
			}

			return true;
		}

		public bool Destroyed { get; set; } = false;
		public List<ChunkCoordinates> Neighbors { get; set; } = new List<ChunkCoordinates>();

		public IEnumerable<BlockCoordinates> GetLightSources()
		{
			for (int i = 0; i < Sections.Length; i++)
			{
				var section = Sections[i];
				if (section == null)
					continue;
				
				var si = i;
				si -= _sectionOffset;
					
				var yOffset = (si * 16);
				
				foreach (var ls in section.LightSources.ToArray())
				{
					yield return new BlockCoordinates(ls.X, yOffset + ls.Y, ls.Z);
				}
			}
		}
		
		protected virtual ChunkSection CreateSection(bool storeSkylight, int sections)
		{
			return new ChunkSection(sections);
		}

		public ChunkSection GetSection(int y)
		{
			y = y >> 4;
			y += _sectionOffset;

			if (y >= Sections.Length || y < 0)
			{
				throw new IndexOutOfRangeException($"Y value out of range! Expected a number between 0 & {Sections.Length}, Got: {y}");
			}
			
			var section = Sections[y];
			
			if (section == null)
			{
				var storage = CreateSection(true, 2);
				Sections[y] = storage;
				return storage;
			}

			return (ChunkSection) section;
		}

		public void SetBlockState(int x, int y, int z, BlockState blockState)
		{
			SetBlockState(x, y, z, blockState, 0);
		}

		public void SetBlockState(int x, int y, int z, BlockState state, int storage)
		{
			if (!CheckWithinCoordinates(x, y, z, false))
				return;
			
			if ((x < 0 || x > ChunkWidth) || (z < 0 || z > ChunkDepth))
				return;

			var section  = GetSection(y);
			//- 16 * (y >> 4)
			section.Set(storage, x, y & 0xf, z, state);

			//_scheduledUpdates[GetCoordinateIndex(x, y, z)] = true;
			//	_heightDirty = true;
		}

		private void RecalculateHeight(int x, int z, bool doLighting = true)
		{
			bool inLight = doLighting;

			bool calculatingHeight = true;
			for (int y = WorldSettings.WorldHeight - 1; y > WorldSettings.MinY; y--)
			{
				var section = GetSection(y);
				var block = section.Get(x, y & 0xf, z).Block;
				if (calculatingHeight)
				{
					if (block.Renderable)
					{
						calculatingHeight = false;
						SetHeight(x, z, (short) (y + 1));
					}
				}
				
				if (inLight)
				{
					if (!block.Renderable || (!block.BlockMaterial.BlocksLight))
					{
						SetSkyLight(x, y, z, 15);
					}
					else
					{
						SetSkyLight(x, y, z, 0);
						inLight = false;
					}
				}

				if (!inLight && !calculatingHeight)
					break;
			}
		}

		public int GetRecalculatedHeight(int x, int z)
		{
			bool isInAir = true;

			for (int y = WorldSettings.WorldHeight - 1; y >= WorldSettings.MinY; y--)
			{
				{
					var chunk = GetSection(y);
					if (isInAir && chunk.IsAllAir)
					{
						y -= 15;
						continue;
					}

					isInAir = false;

					var block = GetBlockState(x, y, z).Block;

					if (!block.Renderable || (block.Transparent && !block.BlockMaterial.BlocksLight))
						continue;

					return y + 1;
				}
			}

			return 0;
		}
		
		public void CalculateHeight(bool doLighting = true)
		{
			for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++)
				{
					RecalculateHeight(x, z, doLighting);
				}
			}

			foreach (var section in Sections)
			{
				section?.RemoveInvalidBlocks();
			}
		}

		private static BlockState Air = BlockFactory.GetBlockState("minecraft:air");

		public IEnumerable<ChunkSection.BlockEntry> GetBlockStates(int bx, int by, int bz)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
			{
				//yield return new ChunkSection.BlockEntry(Air, 0);
				yield break;
			}
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
			{
				//yield return new ChunkSection.BlockEntry(Air, 0);
				yield break;
			}

			var chunk = GetSection(by);
			if (chunk == null)
			{
				//yield return new ChunkSection.BlockEntry(Air, 0);
				yield break;
			}
			
			foreach (var bs in chunk.GetAll(bx, by & 0xf, bz))
			{
				yield return bs;
			}
		}

		public BlockState GetBlockState(int bx, int by, int bz)
		{
			return GetBlockState(bx, by, bz, 0);
		}

		public BlockState GetBlockState(int bx, int by, int bz, int storage)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return Air;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return Air;

			var chunk = GetSection(by);
			if (chunk == null) return Air;

			return chunk.Get(bx, by & 0xf, bz, storage) ?? Air;
		}

		public void SetHeight(int bx, int bz, short h)
		{
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return;

			_height[((bz << 4) + (bx))] = h;
		}

		public int GetHeight(int bx, int bz)
		{
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return WorldSettings.WorldHeight;

			return _height[((bz << 4) + (bx))];
		}

		public void SetBiome(int bx, int by, int bz, int biome)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return;

			_biomeId[GetCoordinateIndex(bx, by, bz)] = biome;
		}

		public int GetBiome(int bx, int by, int bz)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return 0;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return 0;

			return _biomeId[GetCoordinateIndex(bx, by, bz)];
		}

		public byte GetBlocklight(int bx, int by, int bz)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return 0;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return 0;

			var section = GetSection(by);
			if (section == null) return 0;

			return (byte) section.GetBlocklight(bx, by & 0xf, bz);
		}

		public bool SetBlocklight(int bx, int by, int bz, byte data)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return false;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return false;
			
			return GetSection(by).SetBlocklight(bx, by & 0xf, bz, data);
		}

		public void GetLight(int bx, int by, int bz, out byte skyLight, out byte blockLight)
		{
			skyLight = 0xff;
			blockLight = 0;
			
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return;

			var section = GetSection(by);
			if (section == null) return;

			section.GetLight(bx, by & 0xf, bz, out skyLight, out blockLight);
		}

		public byte GetSkylight(int bx, int by, int bz)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return 0xff;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return 0xff;

			var section = GetSection(by);
			if (section == null) return 0xff;

			return section.GetSkylight(bx, by  & 0xf, bz);
		}

		public bool SetSkyLight(int bx, int by, int bz, byte data)
		{
			if (!CheckWithinCoordinates(bx, by, bz, false))
				return false;
			
			if ((bx < 0 || bx > ChunkWidth) || (bz < 0 || bz > ChunkDepth))
				return false;

			return GetSection(by).SetSkylight(bx, by &  0xf, bz, data);
		}

		protected int GetCoordinateIndex(int x, int y, int z)
		{
			y += Math.Abs(this.WorldSettings.MinY);
			
			//return x + 16 * z + _realHeight * y;
			
			return (y << 8 | z << 4 | x);
		}
		
		public void ScheduleBlockUpdate(int x, int y, int z)
		{
			SetScheduled(x,y,z, true);
		}
		
		public bool AddBlockEntity(BlockCoordinates coordinates, NbtCompound entity)
		{
			//entity.Block = GetBlockState(coordinates.X & 0x0f, coordinates.Y & 0xf, coordinates.Z & 0x0f).Block;
			return BlockEntities.TryAdd(coordinates, entity);
		}

		public bool TryGetBlockEntity(BlockCoordinates coordinates, out NbtCompound entity)
		{
			return BlockEntities.TryGetValue(coordinates, out entity);
		}
	    
		public bool RemoveBlockEntity(BlockCoordinates coordinates)
		{
			return BlockEntities.TryRemove(coordinates, out _);
		}

		public void Dispose()
		{
			//lock (_dataLock)
			{
				Destroyed = true;
				
				for (var index = 0; index < Sections.Length; index++)
				{
					var chunksSection = Sections[index];
					Sections[index] = null;
					
					chunksSection?.Dispose();
				}

				//Sections = null;
				ChunkData?.Dispose();
				ChunkData = null;
				
				//_scheduledUpdates = null;
			}
		}
	}
}
