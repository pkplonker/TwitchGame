using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Control;
using UnityEngine;

public class EngagementRewarder : MonoBehaviour
{
	[SerializeField] private float rewardFrequency = 60 * 8;
	private float timer = 0f;
	[SerializeField] private long expReward = 20;
	[SerializeField] private ActiveMembers activeMembers;
	[SerializeField] private ExpHandler expHandler;
	private void Update()
	{
		timer += Time.deltaTime;
		if (timer < rewardFrequency) return;
		Award();
		timer = 0;
	}

	private void Award()
	{
		foreach (var c in activeMembers.GetActiveMembers().Select(am => CharacterManager.GetCharacterByUserName(am.userName)))
		{
			if (c == null) continue;
			expHandler.AllocateXP(c,expReward);
		}
	}
}