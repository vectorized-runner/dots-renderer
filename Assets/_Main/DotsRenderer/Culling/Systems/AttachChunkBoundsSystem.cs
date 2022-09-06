using Unity.Entities;

namespace DotsRenderer
{
	public partial class AttachChunkBoundsSystem : SystemBase
	{
		EntityQuery RequireChunkBoundsQuery;

		protected override void OnCreate()
		{
			RequireChunkBoundsQuery =
				GetEntityQuery(
					ComponentType.ReadOnly<SceneSection>(),
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