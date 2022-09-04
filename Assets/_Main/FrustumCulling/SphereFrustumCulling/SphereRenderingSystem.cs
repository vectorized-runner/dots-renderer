using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DotsLibrary.Rendering
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class SphereRenderingSystem : SystemBase
	{
		Material SphereMaterial;
		Mesh Sphere;

		SphereFrustumCullingSystem SphereFrustumCullingSystem;

		protected override void OnCreate()
		{
			var spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			SphereMaterial = spherePrimitive.GetComponentInChildren<MeshRenderer>().sharedMaterial;
			Sphere = spherePrimitive.GetComponentInChildren<MeshFilter>().sharedMesh;
			Object.Destroy(spherePrimitive);

			SphereFrustumCullingSystem = World.GetExistingSystem<SphereFrustumCullingSystem>();
		}

		protected override void OnUpdate()
		{
			SphereFrustumCullingSystem.CullingJobHandle.Complete();
			var sphereMatrices = SphereFrustumCullingSystem.SphereMatricesToRender;

			Debug.Log($"There are {sphereMatrices.Length} Spheres in the camera view.");

			for(int i = 0; i < sphereMatrices.Length; i++)
			{
				var matrix = sphereMatrices[i];
				Graphics.DrawMesh(Sphere, matrix, SphereMaterial, 0);
			}
		}
	}
}