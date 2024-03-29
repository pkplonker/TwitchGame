//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using TwitchIntegration;
using UnityEngine;

namespace Control
{
	public class ActiveMembers : MonoBehaviour
	{
		private static List<ActiveMember> activeMembers = new List<ActiveMember>();
		[SerializeField] private Commands commands;
		[SerializeField] private float timeOutMinutes;
		public static event Action<string> OnMemberJoin;
		public static event Action<string> OnMemberLeave;
		[SerializeField] [Range(5,150)] private uint maxPlayers=50;

		private void Start() => StartCoroutine(CheckTimeouts());

		protected virtual void OnEnable()
		{
			IRCParser.OnPRIVMSG += OnMessage;
			IRCParser.OnActiveMemberChange += OnActiveMemberChange;
		}

		protected virtual void OnDisable()
		{
			IRCParser.OnPRIVMSG += OnMessage;
			IRCParser.OnActiveMemberChange -= OnActiveMemberChange;

		}

		private void OnActiveMemberChange(string player, bool join)
		{
			
			if(join) JoinPlayer(player);
			else RemovePlayer(player);
		}

		public List<ActiveMember> GetActiveMembers() => activeMembers;

		private IEnumerator CheckTimeouts()
		{
			yield return new WaitForSeconds(30f);

			for (var i = activeMembers.Count - 1; i >= 0; i--)
			{
				if (activeMembers[i].joinTime + (timeOutMinutes * 60) < Time.time)
				{
					activeMembers.Remove(activeMembers[i]);
				}
			}

			StartCoroutine(CheckTimeouts());
		}

		private void OnMessage(string sender, string message)
		{
			sender = sender.ToLower();
			if (message.Contains(commands.GetJoinCommand()))
			{
				JoinPlayer(sender);
			}
			else if (message.Contains(commands.GetLeaveCommand()))
			{
				if (RemovePlayer(sender)) return;
			}

			var amm = FindByUsername(sender);
			if (amm != null) amm.joinTime = Time.time;
		}

		private bool RemovePlayer(string sender)
		{
			if (activeMembers.Count == 0) return true;
			var am = FindByUsername(sender.ToLower());
			if (am == null) return true;
			RemoveMember(am);
			TwitchCore.Instance.PRIVMSGTToTwitch("@" + sender + " has left the game.");
			return false;
		}

		private void JoinPlayer(string sender)
		{
			var am = FindByUsername(sender);
			if (am == null)
			{
				MemberJoin(sender);
				TwitchCore.Instance.PRIVMSGTToTwitch("@" + sender + " has joined the game.");
			}
		}

		private static ActiveMember FindByUsername(string sender) => activeMembers.Find(x => x.userName.ToLower() == sender.ToLower());
		public static bool IsActiveMember(Character character) => FindByUsername(character.GetUserName().ToLower()) != null;

		private void MemberJoin(string sender)
		{
			sender = sender.ToLower();
			if (activeMembers.Count >= maxPlayers)
			{
				TwitchCore.Instance.PRIVMSGTToTwitch($"This game is currently full, please wait and try again {activeMembers.Count}/{maxPlayers}");
				return;
			}
			if (IsBanned(sender)) return;
			activeMembers.Add(new ActiveMember(sender, Time.time));
			OnMemberJoin?.Invoke(sender);
		}

		private bool IsBanned(string sender) => sender.ToLower() == "nightbot".ToLower();


		private void RemoveMember(ActiveMember am)
		{
			activeMembers.Remove(am);
			OnMemberLeave?.Invoke(am.userName);
		}


		[Serializable]
		public class ActiveMember
		{
			public string userName;
			public float joinTime;

			public ActiveMember(string userName, float joinTime)
			{
				this.userName = userName.ToLower();
				this.joinTime = joinTime;
			}
		}
	}
}