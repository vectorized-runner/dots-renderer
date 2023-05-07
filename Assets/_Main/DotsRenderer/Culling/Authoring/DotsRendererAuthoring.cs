using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DotsRenderer
{
	// TODO-Renderer: Handle setup for multi-material models.
	public class DotsRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public MeshRenderer MeshRenderer;
		public MeshFilter MeshFilter;
		public bool IsStatic;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var materials = new List<Material>();
			MeshRenderer.GetSharedMaterials(materials);

			if(materials.Count != 1)
				throw new NotSupportedException("Multiple materials isn't supported.");

			var mesh = MeshFilter.sharedMesh;
			// TODO-Renderer: Get SubMeshIndex properly
			var renderMesh = new RenderMesh(mesh, materials[0], 0);
			dstManager.AddSharedComponentData(entity, renderMesh);

			var localBounds = MeshRenderer.localBounds;
			var renderBounds = new RenderBounds
			{
				AABB = new AABB
				{
					Center = localBounds.center,
					Extents = localBounds.extents,
				}
			};
			dstManager.AddComponentData(entity, renderBounds);

			if(IsStatic)
			{
				dstManager.AddComponent<StaticRenderTag>(entity);
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