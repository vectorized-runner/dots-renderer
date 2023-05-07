﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	// TODO: Move Jobs to here, easier to see flow of logic, name the Jobs properly
	// TODO: Use Static Gizmo Drawer
	// TODO: SIMD Plane-AABB Check
	// TODO: Continue from here.
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
	public struct CollectVisibleRenderMatricesJob : IJobChunk
	{
		[ReadOnly]
		public NativeArray<Plane> CameraFrustumPlanes;

		[ReadOnly]
		public ComponentTypeHandle<CChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;

		[ReadOnly]
		public ComponentTypeHandle<CWorldRenderBounds> WorldRenderBoundsHandle;

		[ReadOnly]
		public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;

		[ReadOnly]
		public ComponentTypeHandle<ChunkRenderMeshIndex> ChunkRenderMeshIndexHandle;

		[WriteOnly]
		public NativeParallelMultiHashMap<ChunkRenderMeshIndex, Matrix4x4> RenderMatricesByRenderMeshIndex;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			if(chunk.HasChunkComponent(ChunkWorldRenderBoundsHandle))
			{
				var worldRenderBounds = chunk.GetChunkComponentData(ChunkWorldRenderBoundsHandle);
				
				if(RMath.IsVisibleByCameraFrustum(CameraFrustumPlanes, worldRenderBounds.Value))
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
	public partial class RenderingSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			// How do I sort chunks by their shared component?

			// Graphics.DrawMeshInstanced();
		}
	}
}