//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
	[SerializeField] private GameObject characterPrefab;
	[SerializeField] private List<Character> characters = new List<Character>();
	[SerializeField] private Commands commands;

	private void OnEnable()
	{
		ActiveMembers.OnMemberJoin += MemberJoin;
		ActiveMembers.OnMemberLeave += MemberLeave;
		IRCParser.OnPRIVMSG += ParseMessage;
	}

	private void OnDisable()
	{
		ActiveMembers.OnMemberJoin -= MemberJoin;
		ActiveMembers.OnMemberLeave -= MemberLeave;
		IRCParser.OnPRIVMSG += ParseMessage;
	}

	private void ParseMessage(string sender, string message)
	{
		if (message.Contains(commands.GetMoveCommand()))
		{
			foreach (var c in characters.Where(c => c.GetUserName() == sender))
			{
				c.RequestMove();
				Debug.Log("moving at request");
				return;
			}
		}
	}

	private void MemberJoin(string username)
	{
		foreach (var character in characters.Where(character => character.GetUserName() == username))
		{
			return;
		}

		Character c = Instantiate(characterPrefab, transform).GetComponent<Character>();
		characters.Add(c);
		c.Init(this, username);
	}

	private void MemberLeave(string username)
	{
		if (characters.Count == 0) return;
		foreach (var character in characters.Where(character => character.GetUserName() == username))
		{
			characters.Remove(character);
			Destroy(character.gameObject);
			return;
		}
	}
}