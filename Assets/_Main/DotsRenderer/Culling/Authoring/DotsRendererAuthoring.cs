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
			var localBounds = MeshRenderer.localBounds;
			Debug.Log($"Center: {localBounds.center}, Extents: {localBounds.extents}");

			dstManager.AddComponentData(entity, new RenderBounds
			{
				AABB = new AABB
				{
					Center = localBounds.center,
					Extents = localBounds.extents,
				}
			});
			dstManager.AddComponentData(entity, new WorldRenderBounds());

			if(IsStatic)
			{
				dstManager.AddComponent<StaticRenderTag>(entity);
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