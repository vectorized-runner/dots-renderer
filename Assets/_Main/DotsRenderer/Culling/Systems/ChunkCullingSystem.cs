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
		public ComponentTypeHandle<RenderMeshIndex> RenderMeshIndexHandle;
		[ReadOnly]
		public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;
		[ReadOnly]
		public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;
		[ReadOnly]
		public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;
		[ReadOnly]
		public NativeArray<Plane> FrustumPlanes;

		public NativeArray<UnsafeStream> MatrixStreamByRenderMeshIndex;
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

			ref var stream = ref MatrixStreamByRenderMeshIndex.ElementAsRef(chunkRenderMeshIndex.Value);
			var writer = stream.AsWriter();

			writer.BeginForEachIndex(ThreadIndex);
			{
				for(int i = 0; i < count; i++)
				{
					if(RMath.IsVisibleByCameraFrustum(FrustumPlanes, worldRenderBoundsArray[i].AABB))
					{
						// Entity is visible, write its Matrix for rendering
						writer.Write(localToWorldArray[i]);
					}
				}
			}
			writer.EndForEachIndex();
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

	public unsafe partial class ChunkCullingSystem : SystemBase
	{
		EntityQuery ChunkCullingQuery;
		CalculateCameraFrustumPlanesSystem FrustumSystem;
		NativeList<UnsafeStream> MatrixStreamByRenderMeshIndex;

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
			var matrixStreamByRenderMeshIndex = MatrixStreamByRenderMeshIndex;
			var renderMeshCount = RendererData.RenderMeshList.Count;

			// TODO: See if we can Clear instead of disposing?
			Job.WithCode(() =>
			   {
				   // Dispose previous frame Streams
				   for(int i = 0; i < matrixStreamByRenderMeshIndex.Length; i++)
				   {
					   ref var matrices = ref matrixStreamByRenderMeshIndex.ElementAsRef(i);
					   matrices.Dispose();
					   matrices = new UnsafeStream(JobsUtility.MaxJobThreadCount, Allocator.TempJob);
				   }
				   
				   // Add new streams to match the RenderMesh count
				   while(matrixStreamByRenderMeshIndex.Length != renderMeshCount)
				   {
					   Debug.Assert(renderMeshCount > matrixStreamByRenderMeshIndex.Length);
					   matrixStreamByRenderMeshIndex.Add(new UnsafeStream(JobsUtility.MaxJobThreadCount, Allocator.TempJob));
				   }
			   })
			   .Schedule();

			var chunkCullingHandle = new ChunkCullingJob
			{
				ChunkWorldRenderBoundsHandle = GetComponentTypeHandle<ChunkWorldRenderBounds>(),
				LocalToWorldHandle = GetComponentTypeHandle<LocalToWorld>(),
				RenderMeshIndexHandle = GetComponentTypeHandle<RenderMeshIndex>(),
				WorldRenderBoundsHandle = GetComponentTypeHandle<WorldRenderBounds>(),
				FrustumPlanes = frustumPlanes,
				MatrixStreamByRenderMeshIndex = matrixStreamByRenderMeshIndex,
			}.ScheduleParallel(ChunkCullingQuery);
			
			var matrixArrayByRenderMeshIndex = new NativeArray<UnsafeArray<float4x4>>(renderMeshCount, Allocator.TempJob);

			var convertStreamJobHandle = new ConvertStreamDataToArrayJob
			{
				Input = matrixStreamByRenderMeshIndex,
				Output = matrixArrayByRenderMeshIndex,
			}.Schedule(renderMeshCount, 4, chunkCullingHandle);

			Dependency = convertStreamJobHandle;

			// TODO: Call ToArray on All Streams, Merge them
			// TODO: Do the Rendering logic
			// TODO: Dispose the streams
			// TODO: Make RenderMeshIndex a SharedComponent, update Queries etc.
			// TODO: Add ChunkWorldRenderBounds automatically, spatial division
			// TODO: Complete this demo! Check for performance!
		}
	}
}