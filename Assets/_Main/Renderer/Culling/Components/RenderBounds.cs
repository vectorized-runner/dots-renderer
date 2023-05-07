using System;
using Unity.Entities;

namespace DotsLibrary.Rendering
{
	[Serializable]
	public struct RenderBounds : IComponentData
	{
		public AABB AABB;
	}
}