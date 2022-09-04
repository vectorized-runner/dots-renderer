using System;
using Unity.Mathematics;

namespace DotsRenderer
{
	[Serializable]
	public struct AABB
	{
		public float3 Center;
		public float3 Extents;
	}
}