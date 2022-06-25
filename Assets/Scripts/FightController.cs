using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StuartHeathTools;
using UnityEngine;

public class FightController : MonoBehaviour
{
	private bool fightActive = false;
	[SerializeField] private Transform fightLocation1;
	[SerializeField] private Transform fightLocation2;
	private Character fighter1;
	private Character fighter2;
	private bool f1ReachedDestination;
	private bool f2ReachedDestination;
	private void OnEnable() => CharacterManager.OnFightRequested += InitiateFight;
	private void OnDisable() => CharacterManager.OnFightRequested -= InitiateFight;

	private void Reset()
	{
		f1ReachedDestination = false;
		f1ReachedDestination = true;
		fighter1 = null;
		fighter2 = null;
	}

	private void InitiateFight(Character fighter1, Character fighter2)
	{
		if (fightLocation1 == null || fightLocation2 == null || fighter1 == null || fighter2 == null)
		{
			Debug.LogError("Missing data");
			return;
		}

		fighter1.OnReachedDestination += ReachedDestination;
		fighter2.OnReachedDestination += ReachedDestination;
		this.fighter1 = fighter1;
		this.fighter2 = fighter2;
		fighter1.StartFight(fightLocation1.transform.position);
		fighter2.StartFight(fightLocation2.transform.position);
	}

	private void ReachedDestination(Character c)
	{
		if (c == fighter1)
		{
			c.Flip(true);
			f1ReachedDestination = true;
		}
		else if (c == fighter2)
		{
			c.Flip(false);
			f2ReachedDestination = true;
		}

		if (f1ReachedDestination && f2ReachedDestination) FightersAtDestination();
	}

	private void FightOver()
	{
		fighter1.OnReachedDestination -= ReachedDestination;
		fighter2.OnReachedDestination -= ReachedDestination;
		Reset();
	}

	private void FightersAtDestination()
	{
		Debug.Log(("At Fight point " + fighter1 + " & " + fighter2).WithColor(Color.red));
	}
}