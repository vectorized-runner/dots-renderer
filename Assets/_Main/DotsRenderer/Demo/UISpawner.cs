﻿using DotsRenderer.PerfTesting;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DotsRenderer.Demo
{
	public class UISpawner : MonoBehaviour
	{
		public TextMeshProUGUI CountText;
		public TMP_InputField InputField;
		public Button SpawnButton;
		public float Spacing;

		EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

		void Start()
		{
			SpawnButton.onClick.AddListener(Spawn);
		}

		void OnDestroy()
		{
			SpawnButton.onClick.RemoveListener(Spawn);
		}

		void LateUpdate()
		{
			var count = EntityManager.CreateEntityQuery(typeof(RenderEntityTag)).CalculateEntityCount();
			CountText.text = $"RenderCount: {count.ToString()}";
		}

		void Spawn()
		{
			var text = InputField.text;
			if(!int.TryParse(text, out var spawnCount))
			{
				Debug.LogError($"Can't parse int from text: '{text}'");
				return;
			}

			var entityManager = EntityManager;
			var renderQuery = entityManager.CreateEntityQuery(typeof(RenderEntityTag));

			// Destroy existing objects first
			entityManager.DestroyEntity(renderQuery);

			var countSqrt = (int)math.ceil(math.sqrt(spawnCount));
			var query = entityManager.CreateEntityQuery(typeof(EntityToSpawn));
			var singleton = query.GetSingletonEntity();
			var entityToSpawn = entityManager.GetComponentData<EntityToSpawn>(singleton).Value;

			var entity = entityManager.CreateEntity();
			entityManager.AddComponentData(entity, new SpawnData
			{
				CountX = countSqrt,
				CountZ = countSqrt,
				Entity = entityToSpawn,
				Spacing = Spacing,
			});

			Debug.Log("Spawn Entities!");
		}
	}
}