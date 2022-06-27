using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class LootBox : MonoBehaviour
{
	private float timer = 0;
	private float lootBoxDuration = 3f;
	private LootBoxSpawner spawner;

	private void Update()
	{
		timer += Time.deltaTime;
		if (!(timer > lootBoxDuration)) return;
		CheckTargets();
		Destroy(gameObject);
	}

	private void CheckTargets()
	{
		var targets = Physics.OverlapSphere(transform.position, 2f);
		var characters = new List<Character>();
		foreach (var target in targets)
		{
			if (TryGetComponent<Character>(out var c))
			{
				characters.Add(c);
			}
		}

		spawner.AwardLoot(characters);
	}

	public void Init(LootBoxSpawner spawner, float lootBoxDuration)
	{
		this.lootBoxDuration = lootBoxDuration;
		this.spawner = spawner;
	}
}