using Unity.Entities;
using UnityEngine;

namespace DotsRenderer
{
	public class RenderBoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public MeshRenderer MeshRenderer;

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
		}

		void OnDrawGizmos()
		{
			if(MeshRenderer == null)
				return;

			Gizmos.DrawWireCube(MeshRenderer.bounds.center, MeshRenderer.bounds.extents * 2f);
		}
	}
}