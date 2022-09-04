using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DotsRenderer.PerfTesting
{
	public partial class SpawnerSystem : SystemBase
	{
		protected override void OnCreate()
		{
			RequireSingletonForUpdate<SpawnData>();
		}

		protected override void OnUpdate()
		{
			var spawnData = GetSingleton<SpawnData>();
			var spawnCount = spawnData.CountX * spawnData.CountZ;
			var spawnedEntities = EntityManager.Instantiate(spawnData.Entity, spawnCount, Allocator.TempJob);
			var countX = spawnData.CountX;

			for(int index = 0; index < spawnedEntities.Length; index++)
			{
				var iX = index / countX;
				var iZ = index % countX;
				var l2w = EntityManager.GetComponentData<LocalToWorld>(spawnedEntities[index]);
				var position = new float3(iX, 0f, iZ) * spawnData.Spacing;
				l2w.Value.c3.xyz = position;
				EntityManager.SetComponentData(spawnedEntities[index], l2w);
			}

			EntityManager.DestroyEntity(GetSingletonEntity<SpawnData>());
		}
	}
}