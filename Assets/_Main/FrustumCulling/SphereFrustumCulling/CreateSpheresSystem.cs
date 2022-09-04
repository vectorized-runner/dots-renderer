using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

namespace DotsLibrary.Rendering
{
	public partial class CreateSpheresSystem : SystemBase
	{
		EntityQuery CreateSpheresQuery;
		EntityArchetype SphereArchetype;

		protected override void OnCreate()
		{
			SphereArchetype = EntityManager.CreateArchetype(
				typeof(CullingSphere));
		}

		protected override void OnUpdate()
		{
			Entities
				.WithStoreEntityQueryInField(ref CreateSpheresQuery)
				.ForEach((ref CreateSpheresData createSpheres) =>
				{
					var random = new Random((uint)Stopwatch.GetTimestamp());
					var createdEntities =
						EntityManager.CreateEntity(SphereArchetype, createSpheres.Count, Allocator.Temp);

					for(int i = 0; i < createSpheres.Count; i++)
					{
						EntityManager.SetComponentData(createdEntities[i], new CullingSphere
						{
							Center = random.NextFloat3() * createSpheres.SpawnRadius,
							Radius = 0.5f,
						});
					}

					Debug.Log($"Created {createSpheres.Count} Frustum Culling entities.");
				})
				.WithStructuralChanges()
				.WithoutBurst()
				.Run();

			EntityManager.DestroyEntity(CreateSpheresQuery);
		}
	}
}