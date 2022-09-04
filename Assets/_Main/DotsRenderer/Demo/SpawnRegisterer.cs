using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DotsRenderer.Demo
{
	[GenerateAuthoringComponent]
	public struct EntityToSpawn : IComponentData
	{
		public Entity Value;
	}

	public class SpawnRegisterer : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
	{
		public GameObject Prefab;

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(Prefab);
		}

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var primaryEntity = conversionSystem.GetPrimaryEntity(Prefab);
			dstManager.AddComponentData(entity, new EntityToSpawn
			{
				Value = primaryEntity
			});
		}
	}
}