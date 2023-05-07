using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[UpdateAfter(typeof(CalculateWorldRenderBoundsSystem))]
	public partial class CullingSystem : SystemBase
	{
		public NativeList<UnsafeList<LocalToWorld>> MatricesByRenderMeshIndex;

		protected override void OnCreate()
		{
			// TODO: We need to Initialize and Grow this when required!
			MatricesByRenderMeshIndex = new NativeList<UnsafeList<LocalToWorld>>(Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			for(int i = 0; i < MatricesByRenderMeshIndex.Length; i++)
			{
				MatricesByRenderMeshIndex[i].Dispose();
			}

			MatricesByRenderMeshIndex.Dispose();
		}

		protected override void OnUpdate()
		{
			var frustumPlanes = World.GetExistingSystem<CalculateCameraFrustumPlanesSystem>().NativeFrustumPlanes;
			var matricesByRenderMeshIndex = MatricesByRenderMeshIndex;

			// Clear previous frame lists
			Job.WithCode(() =>
			   {
				   for(int i = 0; i < matricesByRenderMeshIndex.Length; i++)
				   {
					   matricesByRenderMeshIndex[i].Clear();
				   }
			   })
			   .Schedule();
			
			Entities.ForEach((in WorldRenderBounds worldRenderBounds,
			                  in RenderMeshIndex renderMeshIndex,
			                  in LocalToWorld localToWorld) =>
			        {
				        if(RMath.IsVisibleByCameraFrustum(frustumPlanes, worldRenderBounds.AABB))
				        {
					        ref var matrices = ref matricesByRenderMeshIndex.ElementAsRef(renderMeshIndex.Value);
					        // TODO-Optimization: This addition is not thread-safe, we need to figure out another way if we want to process in parallel.
					        matrices.Add(localToWorld);
				        }
			        })
			        .Schedule();
		}
	}
}