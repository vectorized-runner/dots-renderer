using Unity.Entities;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	public class RendererDebugDrawer : MonoBehaviour
	{
		[SerializeField]
		Color RenderBoundsGizmoColor;

		void OnDrawGizmos()
		{
			// This is a runtime system
			if(!Application.isPlaying)
				return;

			var collectSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<CollectRenderBoundsSystem>();
			collectSystem.FinishJob();

			var renderBounds = collectSystem.RenderBoundsOfEntities;

			Gizmos.color = RenderBoundsGizmoColor;

			for(int i = 0; i < renderBounds.Length; i++)
			{
				Gizmos.DrawWireCube(renderBounds[i].AABB.Center, renderBounds[i].AABB.Extents * 2f);
			}
		}
	}
}