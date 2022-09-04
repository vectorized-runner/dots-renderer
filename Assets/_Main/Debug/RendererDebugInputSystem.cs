using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DotsLibrary
{
	public partial class RendererDebugInputSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var up = Input.GetKey(KeyCode.W) ? 1 : 0;
			var down = Input.GetKey(KeyCode.S) ? 1 : 0;
			var left = Input.GetKey(KeyCode.A) ? 1 : 0;
			var right = Input.GetKey(KeyCode.D) ? 1 : 0;
			var rotateX = Input.GetKey(KeyCode.Z) ? 1 : 0;
			var rotateY = Input.GetKey(KeyCode.X) ? 1 : 0;
			var rotateZ = Input.GetKey(KeyCode.C) ? 1 : 0;
			var deltaTime = Time.DeltaTime;

			Entities
				.WithAll<RendererDebugInput>()
				.ForEach((ref Translation translation, ref Rotation rotation) =>
				{
					var moveAmount = (up - down) * math.up() + (right - left) * math.right();
					translation.Value += moveAmount * deltaTime;

					// const float rotationPerSecond = 15f;
					// var rotX = quaternion.RotateX(rotateX * rotationPerSecond);
					// var rotY = quaternion.RotateY(rotateY * rotationPerSecond);
					// var rotZ = quaternion.RotateZ(rotateZ * rotationPerSecond);
					//
					// var first = math.mul(rotation.Value, rotX);
					// var second = math.mul(first, rotY);
					// var third = math.mul(second, rotZ);
					// rotation.Value = third;
					
					rotation.Value = math.mul(
						math.normalize(rotation.Value), 
						// Rotate 1 rad/s
						quaternion.RotateX(deltaTime));

					// Rotate y in 15 degrees per second?
					// rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.RotateY(15f * deltaTime));

				})
				.Run();
		}
	}
}