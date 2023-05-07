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
			
			for(int i = 0; i < spawnedEntities.Length; i++)
			{
				var iX = spawnCount / countX;
				var iZ = spawnCount % countX;
				var translation = new Translation { Value = new float3(iX, 0f, iZ) };
				EntityManager.SetComponentData(spawnedEntities[i], translation);
			}
			
			EntityManager.DestroyEntity(GetSingletonEntity<SpawnData>());
		}
	}
}