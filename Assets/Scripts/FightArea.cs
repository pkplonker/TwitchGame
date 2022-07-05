//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Characters;
using Control;
using UnityEngine;

/// <summary>
///FightArea full description
/// </summary>
public class FightArea : MonoBehaviour
{
	[SerializeField] private FightController fightController;
	[SerializeField] private Transform fightPoint1;
	[SerializeField] private Transform fightPoint2;

	private void OnTriggerStay2D(Collider2D other)
	{
		CheckForPlayers(other);
	}

	private void CheckForPlayers(Collider2D other)
	{
		var character = other.GetComponent<Character>();
		if (character != null && fightController.GetIsFightEvent())
		{
			var fighters = fightController.GetFighters();
			if (character == fighters.Item1) return;
			if (character == fighters.Item2) return;

			MoveCharacterToSafeArea(character);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)=>CheckForPlayers(other);
			

	private void MoveCharacterToSafeArea(Character character) =>
		MoveChar(!(character.transform.position.x > fightPoint2.transform.position.x), character.GetComponent<CharacterMovement>());


	private void MoveChar(bool moveLeft, CharacterMovement characterMovement)
	{
		characterMovement.RequestMove(moveLeft
			? new Vector3(UnityEngine.Random.Range(characterMovement.GetMinX(), fightPoint1.transform.position.x), 0, 0)
			: new Vector3(UnityEngine.Random.Range(characterMovement.GetMaxX(), fightPoint1.transform.position.x), 0, 0));
	}
	
}