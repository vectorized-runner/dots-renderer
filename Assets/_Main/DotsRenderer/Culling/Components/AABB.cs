using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace DotsRenderer
{
	[Serializable]
	public struct AABB
	{
		public float3 Center;
		public float3 Extents;
		
		public float3 Min => Center - Extents;
		public float3 Max => Center + Extents;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AABB FromMinMax(float3 min, float3 max)
		{
			return new AABB
			{
				Center = (min + max) * 0.5f,
				Extents = (max - min) * 0.5f,
			};
		}
	}
}