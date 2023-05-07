using Unity.Entities;
using Unity.Transforms;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(CullingGroup))]
	public partial class AttachWorldBoundsToStaticEntitySystem : SystemBase
	{
		EntityCommandBufferSystem ECBSystem;

		protected override void OnCreate()
		{
			ECBSystem = World.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ECBSystem.CreateCommandBuffer();

			Entities
				.WithAll<StaticRenderTag, StaticRenderInitTag>()
				.ForEach((Entity entity, in RenderBounds renderBounds, in LocalToWorld localToWorld) =>
				{
					var worldBounds = CalculateWorldRenderBoundsSystem.CalculateWorldBounds(renderBounds, localToWorld);
					commandBuffer.AddComponent(entity, worldBounds);
					commandBuffer.RemoveComponent<RenderBounds>(entity);
					commandBuffer.RemoveComponent<StaticRenderInitTag>(entity);
				})
				.Schedule();

			ECBSystem.AddJobHandleForProducer(Dependency);
		}
	}
}