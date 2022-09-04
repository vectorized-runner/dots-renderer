using Unity.Entities;
using UnityEngine;

namespace DotsRenderer
{
	public class DotsRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public MeshRenderer MeshRenderer;
		public bool IsStatic;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var rendererBounds = MeshRenderer.bounds;
			dstManager.AddComponentData(entity, new WorldRenderBounds
			{
				AABB = new AABB
				{
					Center = rendererBounds.center,
					Extents = rendererBounds.extents
				}
			});

			if(IsStatic)
			{
				dstManager.AddComponent<StaticRenderTag>(entity);
			}
			else
			{
				var localBounds = MeshRenderer.localBounds;
				dstManager.AddComponentData(entity, new RenderBounds
				{
					AABB = new AABB
					{
						Center = localBounds.center,
						Extents = localBounds.extents,
					}
				});
			}
		}

		void OnDrawGizmos()
		{
			if(MeshRenderer == null)
				return;

			Gizmos.DrawWireCube(MeshRenderer.bounds.center, MeshRenderer.bounds.extents * 2f);
		}
	}
}