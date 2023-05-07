using System;
using Unity.Entities;

namespace DotsLibrary.Rendering
{
	[Serializable]
	public struct WorldRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}