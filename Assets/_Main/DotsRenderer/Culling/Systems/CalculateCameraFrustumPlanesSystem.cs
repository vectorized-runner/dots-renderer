using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
	public partial class CalculateCameraFrustumPlanesSystem : SystemBase
	{
		public NativeArray<Plane> NativeFrustumPlanes;

		Camera Camera;
		Plane[] FrustumPlanes;

		protected override void OnCreate()
		{
			Camera = Camera.main;
			FrustumPlanes = new Plane[6];
			NativeFrustumPlanes = new NativeArray<Plane>(6, Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			NativeFrustumPlanes.Dispose();
		}

		protected override void OnUpdate()
		{
			GeometryUtility.CalculateFrustumPlanes(Camera, FrustumPlanes);
			NativeFrustumPlanes.CopyFrom(FrustumPlanes);
		}
		
		public void DebugDrawCameraFrustum()
		{
			var corners = new Vector3[4];
			Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), Camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
			for(int i = 0; i < corners.Length; i++)
			{
				var worldSpaceCorner = Camera.transform.TransformVector(corners[i]);
				Debug.DrawRay(Camera.transform.position, worldSpaceCorner, Color.blue);
			}
		}
	}
}