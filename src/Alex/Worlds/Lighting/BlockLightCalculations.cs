using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alex.API.Utils;
using Alex.Blocks.Minecraft;
using Alex.Worlds.Chunks;
using NLog;

namespace Alex.Worlds.Lighting
{
    public class BlockLightCalculations : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger(typeof(BlockLightCalculations));
        //private static ConcurrentQueue<(BlockCoordinates coords, Func<bool> action)> Queue { get; } = new ConcurrentQueue<(BlockCoordinates coords, Func<bool> action)>();
		private static ConcurrentDictionary<ChunkCoordinates, ConcurrentQueue<BlockCoordinates>> ChunkQueues { get; } = new ConcurrentDictionary<ChunkCoordinates, ConcurrentQueue<BlockCoordinates>>();
		
        private World             World             { get; }
        private CancellationToken CancellationToken { get; }
        public BlockLightCalculations(World level, CancellationToken cancellationToken)
        {
	        World = level;
	        CancellationToken = cancellationToken;
        }

        public void Remove(ChunkCoordinates coordinates)
        {
	        ChunkQueues.TryRemove(coordinates, out _);
        }

        public void Clear()
        {
	        ChunkQueues.Clear();
	      //  Queue.Clear();
        }
        
        public bool HasEnqueued(ChunkCoordinates coordinates)
        {
	        if (ChunkQueues.TryGetValue(coordinates, out var queue))
	        {
		        return !queue.IsEmpty;
	        }

	        return false;
        }

        public void Recalculate(ChunkColumn chunk)
        {
	        ChunkQueues.TryRemove(new ChunkCoordinates(chunk.X, chunk.Z), out _);
	      //  var lightSources = chunk.GetLightSources().ToArray();

	        foreach (var section in chunk.Sections)
	        {
		        section?.ResetLight(true, false);
		        section?.RemoveInvalidBlocks();
	        }
	        
	        var chunkpos = new BlockCoordinates(chunk.X << 4, 0, chunk.Z << 4);
	        foreach (var lightsource in chunk.GetLightSources())
	        {
		        Enqueue(chunkpos + lightsource);
	        }
	        //chunk.GetLightSources()
        }
        
        public bool Process(ChunkCoordinates coordinates)
        {
	        int count = 0;
	        if (ChunkQueues.TryGetValue(coordinates, out var queue))
	        {
		        while (queue.TryDequeue(out var coords) && !CancellationToken.IsCancellationRequested)
		        {
			        ProcessNode(World, coords, queue);
			        count++;
		        }

		        if (queue.IsEmpty)
		        {
			        ChunkQueues.TryRemove(coordinates, out _);
		        }
	        }

	        return count > 0;
        }

        public void Enqueue(BlockCoordinates coordinates)
        {
	       /* if (!Queue.Contains(coordinates))
	        {
		        Queue.Enqueue(coordinates);*/

		        ChunkQueues.AddOrUpdate((ChunkCoordinates) coordinates,
			        chunkCoordinates =>
			        {
				        var newQueue = new ConcurrentQueue<BlockCoordinates>();
				        newQueue.Enqueue(coordinates);

				        return newQueue;
			        },
			        (chunkCoordinates, queue) =>
			        {
				        if (!queue.Contains(coordinates))
				        {
					        queue.Enqueue(coordinates);
				        }

				        return queue;
			        });
	       // }
        }
        
       /* public bool TryProcess(Func<BlockCoordinates, bool> canProcess, out BlockCoordinates processedCoordinates)
        {
	        processedCoordinates = default;
	        if (Queue.TryDequeue(out var queued))
	        {
		        processedCoordinates = queued.coords;
		        if (canProcess.Invoke(processedCoordinates))
		        {
			        var result = queued.action?.Invoke();
			        if (result.HasValue && !result.Value)
			        {
				        Queue.Enqueue(queued);
				        return false;
			        }

			        if (!result.HasValue)
				        return false;

			        return true;
		        }

		        /*if (ChunkQueues.TryGetValue((ChunkCoordinates) coordinates, out var chunkQueue))
		        {
			        ProcessNode(World, coordinates, chunkQueue);
		        }*

		        //return true;
	        }

	        return false;
        }*/
       private bool GetHeighestNeighbor(World level,
	       BlockCoordinates block,
	       out BlockCoordinates position,
	       out byte lightLevel)
       {
	       lightLevel = 0;

	       byte             lvl    = 0;
	       BlockCoordinates result = block;
	       
	       void test(BlockCoordinates p, ref BlockCoordinates r, ref byte l)
	       {
		       if (level.TryGetBlockLight(p, out var up) && up >= l)
		       {
			       l = up;
			       r = p;
		       }
	       }
	       
	       test(block.BlockUp(), ref result, ref lvl);
	       test(block.BlockDown(), ref result, ref lvl);
	       test(block.BlockWest(), ref result, ref lvl);
	       test(block.BlockEast(), ref result, ref lvl);
	       test(block.BlockSouth(), ref result, ref lvl);
	       test(block.BlockNorth(), ref result, ref lvl);
	       
	       position = result;
	       lightLevel = lvl;

	       return result != block;
       }
       
        private void ProcessNode(World level, BlockCoordinates coord, ConcurrentQueue<BlockCoordinates> lightBfsQueue)
		{
			if (level.TryGetBlockLight(coord, out var lightLevel))
			{
				//if (lightLevel )
				/*var neighboringBlock = level.GetBlockState(neighbor);
				var isLightSource    = neighboringBlock.Block.LightValue > 0;
				
				if (neighborLight + 1 < lightLevel) //Neighbor is darker than we are.
				{
					if (self.Block.LightValue == 0)
					{
						
					}
				}
				else if (lightLevel + 1 < neighborLight) //Neighbor is brighter than we are
				{
					Enqueue(neighbor);
				}*/

				Test(level, coord, coord.BlockUp(), lightBfsQueue, lightLevel);
				Test(level, coord, coord.BlockDown(), lightBfsQueue, lightLevel);
				Test(level, coord, coord.BlockWest(), lightBfsQueue, lightLevel);
				Test(level, coord, coord.BlockEast(), lightBfsQueue, lightLevel);
				Test(level, coord, coord.BlockSouth(), lightBfsQueue, lightLevel);
				Test(level, coord, coord.BlockNorth(), lightBfsQueue, lightLevel);

				//SetLightLevel(level, lightBfsQueue, level.GetBlockId(coord + BlockCoordinates.Down, chunk), lightLevel);
				//SetLightLevel(level, lightBfsQueue, level.GetBlockId(coord + BlockCoordinates.West, chunk), lightLevel);
				//SetLightLevel(level, lightBfsQueue, level.GetBlockId(coord + BlockCoordinates.East, chunk), lightLevel);
				//SetLightLevel(level, lightBfsQueue, level.GetBlockId(coord + BlockCoordinates.South, chunk), lightLevel);
				//SetLightLevel(level, lightBfsQueue, level.GetBlockId(coord + BlockCoordinates.North, chunk), lightLevel);
			}
		}

		private ChunkColumn GetChunk(World level, BlockCoordinates blockCoordinates)
		{
			return level.GetChunk(blockCoordinates);
		}

		private void Test(World level, BlockCoordinates sourceBlock, BlockCoordinates target, ConcurrentQueue<BlockCoordinates> lightBfsQueue, int lightLevel)
		{
			var chunkCoord = new ChunkCoordinates(sourceBlock);
			//Interlocked.Add(ref touches, 1);

			bool isOtherChunk = false;
			var newChunkCoord = (ChunkCoordinates) target;
			if (chunkCoord.X != newChunkCoord.X || chunkCoord.Z != newChunkCoord.Z)
			{
				//chunk = GetChunk(level, newCoord);
				lightBfsQueue =
					ChunkQueues.GetOrAdd(newChunkCoord, coordinates => new ConcurrentQueue<BlockCoordinates>());
					
				isOtherChunk = true;
			}

			if (isOtherChunk)
			{
				if (!World.TryGetBlockLight(target, out _))
				{
					Enqueue(target);

					return;
				}
				else
				{
					
				}
				/*Queue.Enqueue((newCoord, () =>
				{
					if (ChunkQueues.TryGetValue((ChunkCoordinates) newCoord, out var queue))
					{
						if (!level.TryGetBlockLight(coord, out var ll))
						{
							return false;
						}
						//var cc = GetChunk(level, newCoord);
						//if (cc == null)
						//	return false;

						//var ll = level.GetBlockLight(coord);
						
						DoPass(level, newCoord, queue, ll);
						
						Enqueue(coord);
						Enqueue(newCoord);

						return true;
					}

					return false;
				}));*/
				
				//return;
			}
			
			/*if (lightLevel > 0)
			{
				GetHeighestNeighbor(level, sourceBlock, out var neighbor, out var neighborLight);
				
				//if (neighbor != coord)
				{
					if (neighborLight <= lightLevel)
					{
						var self              = level.GetBlockState(sourceBlock);
						var selfIsLightSource = self.Block.LightValue > 0;

						if (!selfIsLightSource)
						{
							World.SetBlockLight(sourceBlock, (byte) Math.Max(neighborLight - 1, 0));

							if (neighbor != sourceBlock && neighborLight - 1 > 0)
							{
								Enqueue(neighbor);
							}

							return;
						}
					}
				}
			}*/

			DoPass(level, target, lightBfsQueue, lightLevel);
		}

		private void DoPass(World level, BlockCoordinates target,
			ConcurrentQueue<BlockCoordinates> lightBfsQueue, int lightLevel)
		{
			var block = level.GetBlockState(target).Block;// chunk.GetBlockState(newCoord.X & 0x0f, newCoord.Y & 0xff, newCoord.Z & 0x0f).Block;

			if (!block.Renderable || block.BlockMaterial == Material.Air) 
			{
				SetLightLevel(lightBfsQueue, target, lightLevel);
			}
			else
			{
				SetLightLevel(level, lightBfsQueue, target, level.GetBlockLight(target),
					(Block) block, lightLevel);
			}
		}

		private void UpdateNeighbors(World world, BlockCoordinates coordinates)
		{
			//var source = new BlockCoordinates(x, y, z);

			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X + 1, coordinates.Y, coordinates.Z));
			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X - 1, coordinates.Y, coordinates.Z));

			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X, coordinates.Y, coordinates.Z + 1));
			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X, coordinates.Y, coordinates.Z - 1));

			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X, coordinates.Y + 1, coordinates.Z));
			world.ScheduleBlockUpdate(new BlockCoordinates(coordinates.X, coordinates.Y - 1, coordinates.Z));
		}
		
		private void SetLightLevel(World world, ConcurrentQueue<BlockCoordinates> lightBfsQueue, BlockCoordinates coordinates, int currentLightLevel, Block block, int lightLevel)
		{
			if (currentLightLevel > 0)
			{
				if (currentLightLevel >= lightLevel)
				{
					return;
				}
				
				world.SetBlockLight(coordinates, (byte) Math.Max(currentLightLevel, lightLevel - 1));
				
				UpdateNeighbors(world, coordinates);
				
				return;
			}

			if ((!block.Solid || block.Transparent) && currentLightLevel + 2 <= lightLevel)
			{
				world.SetBlockLight(coordinates,  (byte) (lightLevel - 1));
				
				UpdateNeighbors(world, coordinates);
				
				//Enqueue(coordinates);
				if (!lightBfsQueue.Contains(coordinates))
				{
					lightBfsQueue.Enqueue(coordinates);
				}
			}
		}

		private void SetLightLevel(ConcurrentQueue<BlockCoordinates> lightBfsQueue, BlockCoordinates coord, int lightLevel)
		{
			var val = World.GetBlockLight(coord);
			if (val + 2 <= lightLevel)
			{
				//chunk.SetBlocklight(coord.X & 0x0f, coord.Y & 0xff, coord.Z & 0x0f, (byte) (lightLevel - 1));
				World.SetBlockLight(coord, (byte) (lightLevel - 1));
				
				UpdateNeighbors(World, coord);
				
				//Enqueue(coord);
				if (!lightBfsQueue.Contains(coord))
				{
					lightBfsQueue.Enqueue(coord);
				}
			}else if (lightLevel < val)
			{
				if (!lightBfsQueue.Contains(coord))
				{
					lightBfsQueue.Enqueue(coord);
				}
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			//Queue.Clear();
			ChunkQueues.Clear();
		}
    }
}