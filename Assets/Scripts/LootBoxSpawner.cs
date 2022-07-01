//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Control;
using UnityEngine;

/// <summary>
///LootBoxSpawner full description
/// </summary>
public class LootBoxSpawner : MonoBehaviour
{
	[SerializeField] private List<Transform> lootBoxSpawnPositions;
	[SerializeField] private LootBox lootBoxPrefab;
	[SerializeField] private ExpHandler xExpHandler;
	[SerializeField] private float lootBoxDuration = 12f;
	[SerializeField] private long lootBoxXP = 200L;

	[SerializeField] private float lootSpawnMin = 10f;
	[SerializeField] private float lootSpawnMax = 30f;
	private float currentTargetTime;
	private float timer;
	private LootBox currentLootBox;

	private void Awake() => lootBoxSpawnPositions = GetComponentsInChildren<Transform>().ToList();
	private void Start() => SetNewSpawnTargetTime();
	private void SetNewSpawnTargetTime() => currentTargetTime = UnityEngine.Random.Range(lootSpawnMin*60, lootSpawnMax*60);


	private void Update()
	{
		timer += Time.deltaTime;
		if (!(timer > currentTargetTime)) return;
		if (currentLootBox != null)
		{
			timer = 0;
			SetNewSpawnTargetTime();
			return;
		}
		SpawnLootBox();
	}

	private void SpawnLootBox()
	{
		currentLootBox = Instantiate(lootBoxPrefab,
			StuartHeathTools.UtilityRandom.GetRandomFromList(lootBoxSpawnPositions).position, Quaternion.identity,
			transform).GetComponent<LootBox>();
		currentLootBox.Init(this, lootBoxDuration);
		timer = 0;
		TwitchIntegration.TwitchCore.Instance.PRIVMSGTToTwitch("----Juicy loot just dropped, can you get there in time?----");
	}

	public void AwardLoot(List<Character> characters)
	{
		foreach (var c in characters) xExpHandler.AllocateXP(c, lootBoxXP);
	}
}