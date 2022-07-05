//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Control;
using UnityEngine;

namespace Characters
{
	/// <summary>
	///CharacterCombat full description
	/// </summary>
	public class CharacterCombat : MonoBehaviour
	{
		private CharacterHealth characterHealth;
		private CharacterMovement characterMovement;

		private bool isFighting;
		private Animator animator;
		private static readonly int DoAttack = Animator.StringToHash("doAttack");
		private Character character;

		private void Start()
		{
			characterHealth = GetComponent<CharacterHealth>();
			characterMovement = GetComponent<CharacterMovement>();
			character = GetComponent<Character>();
			animator = GetComponentInChildren<Animator>();
		}

		private void OnEnable() => FightController.OnFightOver += FightOver;
		private void OnDisable() => FightController.OnFightOver -= FightOver;


		private void FightOver(Character arg1, Character arg2)
		{
			if (arg1 == character || arg2 == character)
			{
				isFighting = false;
			}
		}


		public void StartFight(Vector3 position)
		{
			position = new Vector3(position.x, transform.parent.transform.position.y, position.z);
			if (characterMovement == null) characterMovement = GetComponent<CharacterMovement>();
			characterMovement.RequestMove(position);
			isFighting = true;
		}

		public bool GetIsFighting() => isFighting;

		public void Attack(Character opp)
		{
			if (characterHealth.GetIsDead()) return;
			animator.SetTrigger(DoAttack);
		}
	}
}