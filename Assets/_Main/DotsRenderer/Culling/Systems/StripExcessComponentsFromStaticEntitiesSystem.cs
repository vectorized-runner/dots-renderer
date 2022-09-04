using Unity.Entities;
using Unity.Transforms;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class StripExcessComponentsFromStaticEntitiesSystem : SystemBase
	{
		EntityQuery StaticTranslationQuery;
		EntityQuery StaticRotationQuery;

		protected override void OnCreate()
		{
			StaticTranslationQuery = GetEntityQuery(typeof(Translation), typeof(StaticRenderTag));
			StaticRotationQuery = GetEntityQuery(typeof(Rotation), typeof(StaticRenderTag));
		}

		protected override void OnUpdate()
		{
			if(
				StaticTranslationQuery.CalculateEntityCount() == 0 && 
				StaticRotationQuery.CalculateEntityCount() == 0)
			{
				return;
			}
			
			EntityManager.RemoveComponent(StaticTranslationQuery, ComponentType.ReadOnly<Translation>());
			EntityManager.RemoveComponent(StaticRotationQuery, ComponentType.ReadOnly<Rotation>());
		}
	}
}