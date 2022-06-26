using System;
using System.Collections;
using Characters;
using StuartHeathTools;
using UnityEngine;

namespace Control
{
	public class FightController : MonoBehaviour
	{
		[SerializeField] private Transform fightLocation1;
		[SerializeField] private Transform fightLocation2;
		[SerializeField] private int minDamage = 5;
		[SerializeField] private int maxDamage = 20;
		private Character fighter1;
		private Character fighter2;
		private bool f1ReachedDestination;
		private bool f2ReachedDestination;
		private bool fightUnderWay;
		private bool fightEventActive;
		public static event Action<Character, Character> OnFightOver;
		private Coroutine cor;
		private bool fightOver = false;
		private void OnEnable() => CharacterManager.OnFightRequested += InitiateFight;
		private void OnDisable() => CharacterManager.OnFightRequested -= InitiateFight;

		private void InitiateFight(Character fighter1, Character fighter2)
		{
			if (fightLocation1 == null || fightLocation2 == null || fighter1 == null || fighter2 == null)
			{
				Debug.LogError("Missing data");
				return;
			}

			if (fightEventActive)
			{
				Debug.LogWarning("fight already underway");
			}

			fighter1.OnReachedDestination += ReachedDestination;
			fighter2.OnReachedDestination += ReachedDestination;
			fighter1.OnDeath += OnDeath;
			fighter2.OnDeath += OnDeath;
			this.fighter1 = fighter1;
			this.fighter2 = fighter2;
			fighter1.StartFight(fightLocation1.transform.position);
			fighter2.StartFight(fightLocation2.transform.position);
			fightEventActive = true;
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
			if (fighter1 != null)
			{
				fighter1.OnDeath -= OnDeath;
				fighter1.OnReachedDestination -= ReachedDestination;
				fighter1.ResetAfterFight();
			}

			if (fighter2 != null)
			{
				fighter2.OnDeath -= OnDeath;
				fighter2.OnReachedDestination -= ReachedDestination;
				fighter2.ResetAfterFight();
			}

			f1ReachedDestination = false;
			f2ReachedDestination = false;
			fighter1 = null;
			fighter2 = null;
			fightUnderWay = false;
			fightEventActive = false;
			fightOver = false;
			cor = null;
		}


		private void FightersAtDestination()
		{
			if (fightUnderWay) return;
			fightUnderWay = true;
			cor ??= StartCoroutine(Fight());
		}

		private IEnumerator Fight()
		{
			fighter1.OnDeath += OnDeath;
			fighter2.OnDeath += OnDeath;
			var isF1Turn = UtilityRandom.RandomBool();
			while (fightUnderWay)
			{
				//attack
				yield return new WaitForSeconds(1f);
				isF1Turn = !isF1Turn;
				if (isF1Turn) fighter1.Attack(fighter2);
				else fighter2.Attack(fighter1);

				//damage
				yield return new WaitForSeconds(0.2f);
				if (isF1Turn) fighter2.TakeDamage(UnityEngine.Random.Range(minDamage, maxDamage));
				else fighter1.TakeDamage(UnityEngine.Random.Range(minDamage, maxDamage));
				yield return null;
			}
		}

		private void OnDeath(Character c)
		{
			if (fightOver) return;
			fightOver = true;

			if (cor != null)
			{
				StopCoroutine(cor);
			}

			if (c == fighter1)
			{
				Debug.Log("fighter 2 wins");
				OnFightOver?.Invoke(fighter2, fighter1);
			}

			else if (c == fighter2)
			{
				Debug.Log("fighter 1 wins");
				OnFightOver?.Invoke(fighter1, fighter2);
			}

			fightUnderWay = false;
			Invoke(nameof(FightOver), 4f);
		}
	}
}