using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DotsRenderer
{
	// TODO-Handle setup for multi-material models.
	public class DotsRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public MeshRenderer MeshRenderer;
		public MeshFilter MeshFilter;
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

			var materials = new List<Material>();
			MeshRenderer.GetSharedMaterials(materials);

			if(materials.Count != 1)
				throw new NotSupportedException("Multiple materials isn't supported.");

			var mesh = MeshFilter.sharedMesh;
			// TODO: Get SubMeshIndex properly
			var renderMesh = new RenderMesh(mesh, materials[0], 0);
			dstManager.AddSharedComponentData(entity, renderMesh);

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