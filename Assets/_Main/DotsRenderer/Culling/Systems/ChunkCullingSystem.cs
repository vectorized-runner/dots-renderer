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

	
	public partial class ChunkCullingSystem : SystemBase
	{
		EntityQuery ChunkCullingQuery;
		CalculateCameraFrustumPlanesSystem FrustumSystem;

		protected override void OnCreate()
		{
			FrustumSystem = World.GetExistingSystem<CalculateCameraFrustumPlanesSystem>();
			
			ChunkCullingQuery = GetEntityQuery(
				ComponentType.ReadOnly<WorldRenderBounds>(),
				ComponentType.ReadOnly<LocalToWorld>(),
				ComponentType.ChunkComponentReadOnly(typeof(RenderMeshIndex)),
				ComponentType.ChunkComponentReadOnly(typeof(ChunkWorldRenderBounds)));
		}

		protected override void OnUpdate()
		{
			var frustumPlanes = FrustumSystem.NativeFrustumPlanes;
			
			new ChunkCullingJob
			{
				ChunkWorldRenderBoundsHandle = GetComponentTypeHandle<ChunkWorldRenderBounds>(),
				LocalToWorldHandle = GetComponentTypeHandle<LocalToWorld>(),
				RenderMeshIndexHandle = GetComponentTypeHandle<RenderMeshIndex>(),
				WorldRenderBoundsHandle = GetComponentTypeHandle<WorldRenderBounds>(),
				FrustumPlanes = frustumPlanes,
				RenderMatricesByRenderMeshIndex = ,
			}.ScheduleParallel(ChunkCullingQuery);
		}
	}
}