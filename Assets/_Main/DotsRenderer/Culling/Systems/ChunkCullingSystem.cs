using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DotsRenderer
{
	[BurstCompile]
	public struct ChunkCullingJob : IJobChunk
	{
		[NativeSetThreadIndex]
		public int ThreadIndex;
		[ReadOnly]
		public NativeArray<UnsafeStream> RenderMatricesByRenderMeshIndex;
		[ReadOnly]
		public ComponentTypeHandle<RenderMeshIndex> RenderMeshIndexHandle;
		[ReadOnly]
		public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;
		[ReadOnly]
		public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;
		[ReadOnly]
		public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;
		[ReadOnly]
		public NativeArray<Plane> FrustumPlanes;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var chunkWorldRenderBounds = chunk.GetChunkComponentData(ChunkWorldRenderBoundsHandle);

			// Early exit, if Chunk doesn't have it no need to check Entities
			if(!RMath.IsVisibleByCameraFrustum(FrustumPlanes, chunkWorldRenderBounds.AABB))
				return;

			var count = chunk.ChunkEntityCount;
			if(count == 0)
				return;
			
			var worldRenderBoundsArray = chunk.GetNativeArray(WorldRenderBoundsHandle);
			// These should be the same?
			Debug.Assert(count == worldRenderBoundsArray.Length);
			
			var chunkRenderMeshIndex = chunk.GetChunkComponentData(RenderMeshIndexHandle);
			var localToWorldArray = chunk.GetNativeArray(LocalToWorldHandle);
			
			ref var stream = ref RenderMatricesByRenderMeshIndex.ElementAsRef(chunkRenderMeshIndex.Value);
			var writer = stream.AsWriter();
			
			writer.BeginForEachIndex(ThreadIndex);
			{
				for(int i = 0; i < count; i++)
				{
					if(RMath.IsVisibleByCameraFrustum(FrustumPlanes, worldRenderBoundsArray[i].AABB))
					{
						writer.Write(localToWorldArray[i]);
					}
				}
			}
			writer.EndForEachIndex();
		}
	}
	
	// What am I trying to achieve?
	// For each chunk, first check the chunk to see if we should render any of its entities.
	// Then check the Entities themselves, and for the ones we should render,
	// Collect their data, but the code needs to run in parallel.

	public partial class ChunkCullingSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			throw new System.NotImplementedException();
		}
	}
}