using Unity.Collections;
using Unity.Entities;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class CollectRenderBoundsSystem : SystemBase
	{
		EntityQuery RenderBoundsQuery;

		public NativeList<RenderBounds> RenderBoundsOfEntities;

		protected override void OnCreate()
		{
			RenderBoundsOfEntities = new NativeList<RenderBounds>(Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			RenderBoundsOfEntities.Dispose();
		}

		public void FinishJob()
		{
			Dependency.Complete();
		}

		protected override void OnUpdate()
		{
			// Is there any point trying to parallel write to this? Don't we cause false sharing?
			var entityCount = RenderBoundsQuery.CalculateEntityCount();

			if(RenderBoundsOfEntities.Length < entityCount)
			{
				RenderBoundsOfEntities.ResizeUninitialized(entityCount);
			}

			var renderBoundsOfEntities = RenderBoundsOfEntities;
			renderBoundsOfEntities.Clear();

			Entities
				.WithStoreEntityQueryInField(ref RenderBoundsQuery)
				.ForEach((in RenderBounds renderBounds) => { renderBoundsOfEntities.AddNoResize(renderBounds); })
				.Schedule();
		}
	}
}