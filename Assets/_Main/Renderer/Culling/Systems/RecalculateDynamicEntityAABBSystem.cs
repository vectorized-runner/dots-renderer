using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DotsLibrary.Rendering
{
	public partial class RecalculateDynamicEntityAABBSystem : SystemBase
	{
		/// <summary>
		/// https://learnopengl.com/Guest-Articles/2021/Scene/Frustum-Culling
		/// </summary>
		protected override void OnUpdate()
		{
			// We can't use WithChangeFilter Translation, Rotation, Scale (allows up to 2 components)
			Entities
				.WithNone<StaticOptimizeRenderingTag>()
				.WithChangeFilter<LocalToWorld>()
				.ForEach((ref RenderBounds renderBounds, in LocalToWorld localToWorld) =>
				{
					var aabb = renderBounds.AABB;
					// Scaled orientation (?)
					var localRight = localToWorld.Right * aabb.Extents.x;
					var localUp = localToWorld.Up * aabb.Extents.y;
					var localForward = localToWorld.Forward * aabb.Extents.z;
					var right = math.right();
					var up = math.up();
					var forward = math.forward();

					var newIi =
						math.abs(math.dot(right, localRight)) +
						math.abs(math.dot(right, localUp)) +
						math.abs(math.dot(right, localForward));

					var newIj =
						math.abs(math.dot(up, localRight)) +
						math.abs(math.dot(up, localUp)) +
						math.abs(math.dot(up, localForward));

					var newIk =
						math.abs(math.dot(forward, localRight)) +
						math.abs(math.dot(forward, localUp)) +
						math.abs(math.dot(forward, localForward));

					var newExtents = new float3(newIi, newIj, newIk);

					renderBounds = new RenderBounds
					{
						AABB = new AABB
						{
							Center = localToWorld.Position,
							Extents = newExtents
						}
					};
				})
				.ScheduleParallel();
		}
	}
}