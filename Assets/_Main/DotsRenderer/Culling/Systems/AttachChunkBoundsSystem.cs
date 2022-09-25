using Unity.Entities;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(CullingGroup))]
	[UpdateAfter(typeof(CalculateWorldRenderBoundsSystem))]
	public partial class AttachChunkBoundsSystem : SystemBase
	{
		EntityQuery RequireChunkBoundsQuery;

		protected override void OnCreate()
		{
			// Add to Chunks with WorldRenderBounds, but no ChunkWorldRenderBounds
			RequireChunkBoundsQuery =
				GetEntityQuery(
					ComponentType.ReadOnly<WorldRenderBounds>(),
					ComponentType.ChunkComponentExclude<ChunkWorldRenderBounds>());
		}

		protected override void OnUpdate()
		{
			if(RequireChunkBoundsQuery.CalculateEntityCount() == 0)
				return;

			EntityManager.AddChunkComponentData(RequireChunkBoundsQuery, new ChunkWorldRenderBounds());
		}
	}
}