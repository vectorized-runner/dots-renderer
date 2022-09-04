using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	// TODO: Remove this System.
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[UpdateAfter(typeof(CalculateFrustumPlanesSystem))]
	[UpdateAfter(typeof(RecalculateDynamicEntityAABBSystem))]
	public partial class AABBFrustumCullingSystem : SystemBase
	{
		public JobHandle CullingJobHandle;
		public NativeList<Matrix4x4> MatricesToRender;

		protected override void OnCreate()
		{
			MatricesToRender = new NativeList<Matrix4x4>(Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			MatricesToRender.Dispose();
		}

		// TODO: We can ScheduleParallel this for extra optimization.
		// TODO: We need to check if this works only for static objects
		// (Objects that change Position, Rotation, Scale also change their AABB
		protected override void OnUpdate()
		{
			var renderPositions = new NativeList<float3>(Allocator.TempJob);
			var frustumPlanes = World.GetExistingSystem<CalculateFrustumPlanesSystem>().NativeFrustumPlanes;

			Entities
				.ForEach((in RenderBounds renderBounds, in Translation translation) =>
				{
					if(RMath.IsVisibleByCameraFrustum(frustumPlanes, renderBounds.AABB))
					{
						renderPositions.Add(translation.Value);
					}
				})
				.Schedule();


			var matricesToRender = MatricesToRender;

			Job.WithCode(() =>
			   {
				   matricesToRender.Clear();

				   if(matricesToRender.Capacity < renderPositions.Capacity)
				   {
					   matricesToRender.SetCapacity(renderPositions.Length);
				   }

				   for(int i = 0; i < renderPositions.Length; i++)
				   {
					   matricesToRender.Add(float4x4.TRS(renderPositions[i], quaternion.identity, new float3(1, 1, 1)));
				   }
			   })
			   .WithDisposeOnCompletion(renderPositions)
			   .Schedule();

			CullingJobHandle = Dependency;
		}


	}
}