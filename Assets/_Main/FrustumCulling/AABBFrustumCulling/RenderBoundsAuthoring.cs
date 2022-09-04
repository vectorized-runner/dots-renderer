using Unity.Entities;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	public class RenderBoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public CRenderBounds RenderBounds;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, RenderBounds);
		}

		// TODO: This should behave well with Rotation/Scaling etc.
		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(RenderBounds.AABB.Center, RenderBounds.AABB.Extents * 2f);
		}
	}
}