using System;
using Unity.Mathematics;

namespace DotsLibrary.Rendering
{
	[Serializable]
	public struct AABB
	{
		public float3 Center;
		public float3 Extents;
	}
}