using System;
using Unity.Entities;

namespace DotsRenderer
{
	[Serializable]
	public struct WorldRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}