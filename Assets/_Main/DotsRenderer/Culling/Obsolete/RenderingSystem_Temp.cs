using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DotsRenderer
{
	// TODO-Renderer: Move Jobs to here, easier to see flow of logic, name the Jobs properly
	// TODO-Renderer: Use Static Gizmo Drawer
	// TODO-Renderer: SIMD Plane-AABB Check
	// TODO-Renderer: Continue from here.
	// In order for this to work, we need the remaining things:
	// RenderBounds should be static, WorldRenderBounds for entities needs to be calculated
	// using for each WorldRenderBounds in the Chunk, ChunkWorldRenderBounds needs to be calculated
	// When a Chunk is added/changed, if it has RenderMesh shared component, we need to add ChunkWorldRenderBounds to it
	// When a Chunk is added/changed, if it has RenderMesh shared component, we need to add/remove ChunkRenderMeshIndex component to it
	// We need to know the amount of Unique Render Meshes, so we can calculate the RenderMesh -> ChunkRenderMeshIndex mapping
	// This job is still very inefficient:
	// for each entity we want to render we're doing a HashMap addition, it should be list/array addition
	// We should remove the HasComponent check for ChunkWorldRenderBounds, the scheduler needs to provide the correct query
	// How can we sort the RenderBatches? (NativeArray<Chunk>, NativeList<int>, input is the Chunks with same RenderMesh, output is Render index list)
	[BurstCompile]
	public struct CullingJob : IJobChunk
	{
		[ReadOnly]
		public NativeArray<Plane> CameraFrustumPlanes;

		[ReadOnly]
		public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;

		[ReadOnly]
		public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;

		[ReadOnly]
		public ComponentTypeHandle<int> ChunkRenderMeshIndexHandle;
		// public ComponentTypeHandle<ChunkRenderMeshIndex> ChunkRenderMeshIndexHandle;

		[WriteOnly]
		public NativeParallelMultiHashMap<int, Matrix4x4> RenderMatricesByRenderMeshIndex;
		// public NativeParallelMultiHashMap<ChunkRenderMeshIndex, Matrix4x4> RenderMatricesByRenderMeshIndex;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			if(chunk.HasChunkComponent(ChunkWorldRenderBoundsHandle))
			{
				var worldRenderBounds = chunk.GetChunkComponentData(ChunkWorldRenderBoundsHandle);
				
				if(RMath.IsVisibleByCameraFrustum(CameraFrustumPlanes, worldRenderBounds.AABB))
				{
					var worldRenderBoundsArray = chunk.GetNativeArray(WorldRenderBoundsHandle);
					var localToWorldArray = chunk.GetNativeArray(LocalToWorldHandle);
					var renderMeshIndex = chunk.GetChunkComponentData(ChunkRenderMeshIndexHandle);

					for(int i = 0; i < chunk.Count; i++)
					{
						if(RMath.IsVisibleByCameraFrustum(CameraFrustumPlanes, worldRenderBoundsArray[i].AABB))
						{
							RenderMatricesByRenderMeshIndex.Add(renderMeshIndex, localToWorldArray[i].Value);
						}
					}
				}
			}
		}
	}

	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class RenderingSystem_Temp : SystemBase
	{
		EntityQuery Query;
		
		protected override void OnCreate()
		{
			Query = GetEntityQuery(
				ComponentType.ChunkComponent<ChunkWorldRenderBounds>());
			
			base.OnCreate();
		}

		protected override void OnUpdate()
		{
			// How do I sort chunks by their shared component?
			// Graphics.DrawMeshInstanced();
		}
	}
}