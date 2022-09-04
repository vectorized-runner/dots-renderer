using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	[UpdateAfter(typeof(CalculateCameraFrustumPlanesSystem))]
	[UpdateBefore(typeof(SphereRenderingSystem))]
	public partial class SphereFrustumCullingSystem : SystemBase
	{
		public JobHandle CullingJobHandle;
		public NativeList<Matrix4x4> SphereMatricesToRender;

		protected override void OnCreate()
		{
			SphereMatricesToRender = new NativeList<Matrix4x4>(Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			SphereMatricesToRender.Dispose();
		}

		// TODO: We can ScheduleParallel this for extra optimization.
		protected override void OnUpdate()
		{
			var spherePositions = new NativeList<float3>(Allocator.TempJob);
			var frustumPlanes = World.GetExistingSystem<CalculateCameraFrustumPlanesSystem>().NativeFrustumPlanes;

			Entities
				.ForEach((in CullingSphere cullingSphere) =>
				{
					if(
						IsOnForwardOrOnThePlane(frustumPlanes[0], cullingSphere) &&
						IsOnForwardOrOnThePlane(frustumPlanes[1], cullingSphere) &&
						IsOnForwardOrOnThePlane(frustumPlanes[2], cullingSphere) &&
						IsOnForwardOrOnThePlane(frustumPlanes[3], cullingSphere) &&
						IsOnForwardOrOnThePlane(frustumPlanes[4], cullingSphere) &&
						IsOnForwardOrOnThePlane(frustumPlanes[5], cullingSphere))
					{
						spherePositions.Add(cullingSphere.Center);
					}
				})
				.Schedule();

			var matricesToRender = SphereMatricesToRender;

			Job.WithCode(() =>
			   {
				   matricesToRender.Clear();

				   if(matricesToRender.Capacity < spherePositions.Capacity)
				   {
					   matricesToRender.SetCapacity(spherePositions.Length);
				   }

				   for(int i = 0; i < spherePositions.Length; i++)
				   {
					   matricesToRender.Add(float4x4.TRS(spherePositions[i], quaternion.identity, new float3(1, 1, 1)));
				   }
			   })
			   .WithDisposeOnCompletion(spherePositions)
			   .Schedule();

			CullingJobHandle = Dependency;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool IsOnForwardOrOnThePlane(Plane plane, CullingSphere cullingSphere)
		{
			return RMath.GetPlaneSignedDistanceToPoint(plane, cullingSphere.Center) > -cullingSphere.Radius;
		}


	}
}