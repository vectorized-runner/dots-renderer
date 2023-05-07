using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DotsRenderer
{
	public static class RMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetPlaneSignedDistanceToPoint(Plane plane, float3 point)
		{
			// From Unity's plane implementation
			// The value returned is positive if the point is on the side of the plane into which the plane's normal is facing, and negative otherwise.
			// public float GetDistanceToPoint(Vector3 point) => Vector3.Dot(this.m_Normal, point) + this.m_Distance;
			return math.dot(plane.normal, point) + plane.distance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsVisibleByCameraFrustum(NativeArray<Plane> frustumPlanes6, AABB aabb)
		{
			return
				IsOnForwardOrOnPlane(frustumPlanes6[0], aabb) &&
				IsOnForwardOrOnPlane(frustumPlanes6[1], aabb) &&
				IsOnForwardOrOnPlane(frustumPlanes6[2], aabb) &&
				IsOnForwardOrOnPlane(frustumPlanes6[3], aabb) &&
				IsOnForwardOrOnPlane(frustumPlanes6[4], aabb) &&
				IsOnForwardOrOnPlane(frustumPlanes6[5], aabb);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOnForwardOrOnPlane(Plane plane, AABB aabb)
		{
			var normalDotExtents = math.dot(aabb.Extents, math.abs(plane.normal));
			var planeDistanceToCenter = GetPlaneSignedDistanceToPoint(plane, aabb.Center);
			return planeDistanceToCenter >= -normalDotExtents;
		}
	}
}