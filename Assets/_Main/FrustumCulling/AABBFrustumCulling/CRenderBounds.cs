using System;
using Unity.Entities;

namespace DotsLibrary.Rendering
{
	[Serializable]
	public struct CRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}