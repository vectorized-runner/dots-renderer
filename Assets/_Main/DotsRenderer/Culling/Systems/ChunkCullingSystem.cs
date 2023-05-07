﻿using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
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
		public SharedComponentTypeHandle<RenderMesh> RenderMeshHandle;
		[ReadOnly]
		public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;
		[ReadOnly]
		public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;
		[ReadOnly]
		public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;
		[ReadOnly]
		public NativeArray<Plane> FrustumPlanes;

		public NativeArray<UnsafeStream> MatricesByRenderMeshIndex;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var chunkWorldRenderBounds = chunk.GetChunkComponentData(ChunkWorldRenderBoundsHandle);

			// Early exit, if Chunk doesn't have it no need to check Entities
			if(!RMath.IsVisibleByCameraFrustum(FrustumPlanes, chunkWorldRenderBounds.AABB))
				return;

			var entityCount = chunk.ChunkEntityCount;
			if(entityCount == 0)
				return;

			var worldRenderBoundsArray = chunk.GetNativeArray(WorldRenderBoundsHandle);
			Debug.Assert(entityCount == worldRenderBoundsArray.Length);

			var sharedComponentIndex = chunk.GetSharedComponentIndex(RenderMeshHandle);
			var localToWorldArray = chunk.GetNativeArray(LocalToWorldHandle);

			ref var matrices = ref MatricesByRenderMeshIndex.ElementAsRef(sharedComponentIndex);
			var matrixWriter = matrices.AsWriter();

			matrixWriter.BeginForEachIndex(ThreadIndex);
			{
				for(int i = 0; i < entityCount; i++)
				{
					if(RMath.IsVisibleByCameraFrustum(FrustumPlanes, worldRenderBoundsArray[i].AABB))
					{
						// Entity is visible, write its Matrix for rendering
						matrixWriter.Write(localToWorldArray[i]);
					}
				}
			}
			matrixWriter.EndForEachIndex();
		}
	}

	[BurstCompile]
	public unsafe struct ConvertStreamDataToArrayJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeList<UnsafeStream> Input;

		[WriteOnly]
		public NativeArray<UnsafeArray<float4x4>> Output;

		public void Execute(int index)
		{
			var allocator = Allocator.TempJob;
			var nativeArray = Input[index].ToNativeArray<float4x4>(allocator);
			Output[index] = new UnsafeArray<float4x4>(nativeArray.GetTypedPtr(), nativeArray.Length, allocator);
		}
	}

	public partial class ChunkCullingSystem : SystemBase
	{
		public NativeArray<UnsafeArray<float4x4>> MatrixArrayByRenderMeshIndex;
		public JobHandle FinalJobHandle { get; private set; }
		public List<RenderMesh> RenderMeshes { get; private set; }

		EntityQuery ChunkCullingQuery;
		CalculateCameraFrustumPlanesSystem FrustumSystem;
		NativeList<UnsafeStream> MatrixStreamByRenderMeshIndex;

		protected override void OnCreate()
		{
			RenderMeshes = new List<RenderMesh>();
			FrustumSystem = World.GetExistingSystem<CalculateCameraFrustumPlanesSystem>();

			ChunkCullingQuery = GetEntityQuery(
				ComponentType.ReadOnly<WorldRenderBounds>(),
				ComponentType.ReadOnly<LocalToWorld>(),
				ComponentType.ReadOnly(typeof(RenderMesh)),
				ComponentType.ChunkComponentReadOnly(typeof(ChunkWorldRenderBounds)));
		}

		protected override void OnUpdate()
		{
			// TODO-Renderer: Ensure no RenderMeshes are created between update of this system and end of update of the Renderer
			var frustumPlanes = FrustumSystem.NativeFrustumPlanes;
			var matrixStreamByRenderMeshIndex = MatrixStreamByRenderMeshIndex;

			RenderMeshes.Clear();
			EntityManager.GetAllUniqueSharedComponentData(RenderMeshes);
			var renderMeshCount = RenderMeshes.Count;

			Job.WithCode(() =>
			   {
				   // Dispose previous frame Streams and Recreate them
				   for(int i = 0; i < matrixStreamByRenderMeshIndex.Length; i++)
				   {
					   ref var matrices = ref matrixStreamByRenderMeshIndex.ElementAsRef(i);
					   matrices.Dispose();
					   matrices = new UnsafeStream(JobsUtility.MaxJobThreadCount, Allocator.TempJob);
				   }

				   // Add new streams to match the RenderMesh count
				   while(matrixStreamByRenderMeshIndex.Length < renderMeshCount)
				   {
					   matrixStreamByRenderMeshIndex.Add(new UnsafeStream(JobsUtility.MaxJobThreadCount,
						   Allocator.TempJob));
				   }
			   })
			   .Schedule();

			var chunkCullingHandle = new ChunkCullingJob
			{
				ChunkWorldRenderBoundsHandle = GetComponentTypeHandle<ChunkWorldRenderBounds>(),
				LocalToWorldHandle = GetComponentTypeHandle<LocalToWorld>(),
				RenderMeshHandle = GetSharedComponentTypeHandle<RenderMesh>(),
				WorldRenderBoundsHandle = GetComponentTypeHandle<WorldRenderBounds>(),
				FrustumPlanes = frustumPlanes,
				MatricesByRenderMeshIndex = matrixStreamByRenderMeshIndex,
			}.ScheduleParallel(ChunkCullingQuery);

			var matrixArrayByRenderMeshIndex =
				new NativeArray<UnsafeArray<float4x4>>(renderMeshCount, Allocator.TempJob);
			MatrixArrayByRenderMeshIndex = matrixArrayByRenderMeshIndex;

			var convertStreamJobHandle = new ConvertStreamDataToArrayJob
			{
				Input = matrixStreamByRenderMeshIndex,
				Output = matrixArrayByRenderMeshIndex,
			}.Schedule(renderMeshCount, 16, chunkCullingHandle);

			FinalJobHandle = convertStreamJobHandle;
		}
	}
}