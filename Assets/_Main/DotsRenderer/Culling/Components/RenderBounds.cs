using System;
using Unity.Entities;

namespace DotsRenderer
{
	[Serializable]
	public struct RenderBounds : IComponentData
	{
		public AABB AABB;
	}
}