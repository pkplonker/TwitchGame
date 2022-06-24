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

	private void OnEnable()
	{
		ActiveMembers.OnMemberJoin += MemberJoin;
		ActiveMembers.OnMemberLeave += MemberLeave;
	}

	private void OnDisable()
	{
		ActiveMembers.OnMemberJoin -= MemberJoin;
		ActiveMembers.OnMemberLeave -= MemberLeave;
	}

	private void MemberJoin(string username)
	{
		foreach (var character in characters.Where(character => character.GetUserName() == username))
		{
			return;
		}

		Character c = Instantiate(characterPrefab,transform).GetComponent<Character>();
		characters.Add(c);
		c.Init(this,username);
	}

	private void MemberLeave(string username)
	{
		if (characters.Count == 0) return;
		foreach (var character in characters.Where(character => character.GetUserName() == username))
		{
			characters.Remove(character);
			return;
		}
	}
}