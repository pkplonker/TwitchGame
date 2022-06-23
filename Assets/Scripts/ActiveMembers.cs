//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using UnityEngine;

public class ActiveMembers : MonoBehaviour
{
	[SerializeField] private List<string> activeMembers = new List<string>();

	protected virtual void OnEnable()
	{
		IRCParser.OnActiveMemberChange += OnMemberChange;
		IRCParser.OnPRIVMSG += OnMessage;
	}

	private void OnMessage(string sender, string message) => OnMemberChange(sender, true); // if user sends message then they are in the channel and can be added to list
	
	protected virtual void OnDisable()
	{
		IRCParser.OnActiveMemberChange -= OnMemberChange;
		IRCParser.OnPRIVMSG += OnMessage;
	}

	private void OnMemberChange(string username, bool isJoiner)
	{
		if (string.IsNullOrWhiteSpace(username)) return;
		if (activeMembers.Contains(username))
		{
			if (isJoiner) return;
			activeMembers.Remove(username);
			Debug.LogWarning("removing: " +username);
		}
		else if (isJoiner) activeMembers.Add(username);
	}
}