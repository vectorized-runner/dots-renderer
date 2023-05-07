using System;
using Unity.Entities;

namespace DotsLibrary.Rendering
{
	[Serializable]
	public struct CWorldRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}