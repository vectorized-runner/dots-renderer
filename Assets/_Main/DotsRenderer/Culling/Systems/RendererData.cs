using System.Collections.Generic;
using UnityEngine;

namespace DotsRenderer
{
	public static class RendererData
	{
		public static List<RenderMesh> RenderMeshList;
		static HashSet<RenderMesh> RenderMeshes;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Initialize()
		{
			RenderMeshList = new List<RenderMesh>();
			RenderMeshes = new HashSet<RenderMesh>();
		}

		public static RenderMeshIndex RegisterRenderMeshAndGetIndex(in RenderMesh renderMesh)
		{
			if(RenderMeshes.Add(renderMesh))
			{
				var index = RenderMeshList.Count;
				RenderMeshList.Add(renderMesh);
				return new RenderMeshIndex(index);
			}

			// TODO-Optimize IndexOf here if required. 
			return new RenderMeshIndex(RenderMeshList.IndexOf(renderMesh));
		}
	}
}