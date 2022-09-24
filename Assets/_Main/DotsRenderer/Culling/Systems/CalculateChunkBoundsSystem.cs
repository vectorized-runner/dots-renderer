using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DotsRenderer
{
	[BurstCompile]
	public struct CalculateChunkBoundsJob : IJobChunk
	{
		public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var worldRenderBoundsArray = chunk.GetNativeArray(WorldRenderBoundsHandle);
			var resultAABB = new AABB();

			// TODO-Renderer: This can be optimized by unrolling and removing 'resultAABB' data dependency?
			// Instead check indices 0-1, 2-3, 4-5 etc...
			for(int i = 0; i < worldRenderBoundsArray.Length; i++)
			{
				resultAABB = Encapsulate(resultAABB, worldRenderBoundsArray[i].AABB);
			}

			chunk.SetChunkComponentData(ChunkWorldRenderBoundsHandle, new ChunkWorldRenderBounds { AABB = resultAABB });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		AABB Encapsulate(AABB first, AABB second)
		{
			var newMin = math.min(first.Min, second.Min);
			var newMax = math.max(first.Max, second.Max);
			return AABB.FromMinMax(newMin, newMax);
		}
	}

	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[UpdateAfter(typeof(AttachChunkBoundsSystem))]
	public partial class CalculateChunkBoundsSystem : SystemBase
	{
		EntityQuery ChangedChunksQuery;

		protected override void OnCreate()
		{
			ChangedChunksQuery = GetEntityQuery(
				ComponentType.ChunkComponent<ChunkWorldRenderBounds>(),
				ComponentType.ReadOnly<WorldRenderBounds>());
			// We only need to recalculate ChunkWorldRenderBounds if any of the 'WorldRenderBounds' of Entities is changed
			ChangedChunksQuery.SetChangedVersionFilter(ComponentType.ReadOnly<WorldRenderBounds>());
		}

		protected override void OnUpdate()
		{
			Dependency = new CalculateChunkBoundsJob
			{
				ChunkWorldRenderBoundsHandle = GetComponentTypeHandle<ChunkWorldRenderBounds>(),
				WorldRenderBoundsHandle = GetComponentTypeHandle<WorldRenderBounds>(),
			}.Schedule(ChangedChunksQuery, Dependency);
		}
	}
}