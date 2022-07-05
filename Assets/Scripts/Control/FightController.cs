using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using StuartHeathTools;
using TwitchIntegration;
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
		public bool GetIsFightEvent() => fightEventActive;
		public Tuple<Character, Character> GetFighters() => new Tuple<Character, Character>(fighter1, fighter2);
		public static event Action<Character, Character> OnFightOver;
		public static event Action OnFightStart;

		private Coroutine cor;
		private bool fightOver ;
		private void OnEnable() => CharacterManager.OnFightRequested += InitiateFight;
		private void OnDisable() => CharacterManager.OnFightRequested -= InitiateFight;
		private Queue<Tuple<Character, Character>> outstandingFights = new Queue<Tuple<Character, Character>>();

		private void InitiateFight(Character fighter1, Character fighter2)
		{
			if (fightLocation1 == null || fightLocation2 == null || fighter1 == null || fighter2 == null) return;

			if (fightEventActive)
			{
				if (CheckIfAlreadyInQueue(fighter1)) TwitchCore.Instance.PRIVMSGTToTwitch("@" + fighter1.GetUserName() +
					                                     " You are already in the queue, Let someone else fight!");
				else
				{
					TwitchCore.Instance.PRIVMSGTToTwitch("@" + fighter1.GetUserName() +
					                                     " Fight Already Underway, You've been added to the queue");
					outstandingFights.Enqueue(new Tuple<Character, Character>(fighter1, fighter2));
				}
				return;
			}

			fighter1.OnReachedDestination += ReachedDestination;
			fighter2.OnReachedDestination += ReachedDestination;
			fighter1.GetComponent<CharacterHealth>().OnDeath += OnDeath;
			fighter2.GetComponent<CharacterHealth>().OnDeath += OnDeath;
			this.fighter1 = fighter1;
			this.fighter2 = fighter2;
			fighter1.StartFight(fightLocation1.transform.position);
			fighter2.StartFight(fightLocation2.transform.position);
			fightEventActive = true;
			OnFightStart?.Invoke();
		}

		private bool CheckIfAlreadyInQueue(Character requestor) =>
			outstandingFights.Any(f => f.Item1 == requestor || f.Item2 == requestor);


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
				fighter1.GetComponent<CharacterHealth>().OnDeath -= OnDeath;
				fighter1.OnReachedDestination -= ReachedDestination;
				fighter1.ResetAfterFight();
			}

			if (fighter2 != null)
			{
				fighter2.GetComponent<CharacterHealth>().OnDeath -= OnDeath;
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

			CheckQueue();
		}

		private void CheckQueue()
		{
			if (outstandingFights.Count == 0) return;
			while (outstandingFights.Count != 0)
			{
				var (item1, item2) = outstandingFights.Dequeue();
				if (!ActiveMembers.IsActiveMember(item1) || !ActiveMembers.IsActiveMember(item2)) continue;
				InitiateFight(item1, item2);
				return;
			}
		}


		private void FightersAtDestination()
		{
			if (fightUnderWay) return;
			fightUnderWay = true;
			cor ??= StartCoroutine(Fight());
		}

		private IEnumerator Fight()
		{

			var f1ch = fighter1.GetComponent<CharacterHealth>();
			var f2ch = fighter2.GetComponent<CharacterHealth>();
			f1ch.OnDeath += OnDeath;
			f2ch.OnDeath -= OnDeath;
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
				if (isF1Turn) f2ch.TakeDamage(UnityEngine.Random.Range(minDamage, maxDamage));
				else f1ch.TakeDamage(UnityEngine.Random.Range(minDamage, maxDamage));
				yield return null;
			}
		}

		private void OnDeath(CharacterHealth ch)
		{
			if (fightOver) return;
			fightOver = true;
			if (cor != null) StopCoroutine(cor);
			if (ch == fighter1.GetComponent<CharacterHealth>()) OnFightOver?.Invoke(fighter2, fighter1);
			else if (ch == fighter2.GetComponent<CharacterHealth>()) OnFightOver?.Invoke(fighter1, fighter2);
			fightUnderWay = false;
			Invoke(nameof(FightOver), 4f);
		}
	}
}