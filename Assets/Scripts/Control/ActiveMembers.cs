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
		protected virtual void OnEnable() => IRCParser.OnPRIVMSG += OnMessage;
		protected virtual void OnDisable() => IRCParser.OnPRIVMSG += OnMessage;
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
			if (message.Contains(commands.GetJoinCommand()))
			{
				var am = FindByUsername(sender);
				if (am == null)
				{
					MemberJoin(sender);
				}
			}
			else if (message.Contains(commands.GetLeaveCommand()))
			{
				if (activeMembers.Count == 0) return;
				var am = FindByUsername(sender);
				if (am == null) return;
				RemoveMember(am);
			}

			var amm = FindByUsername(sender);
			if (amm != null) amm.joinTime = Time.time;
		}

		private static ActiveMember FindByUsername(string sender) => activeMembers.Find(x => x.userName == sender);
		public static bool IsActiveMember(Character character) => FindByUsername(character.GetUserName()) != null;

		private void MemberJoin(string sender)
		{
			if (activeMembers.Count >= maxPlayers)
			{
				TwitchCore.Instance.PRIVMSGTToTwitch("This game is currently full, please wait and try again");
				return;
			}
			if (IsBanned(sender)) return;
			activeMembers.Add(new ActiveMember(sender, Time.time));
			OnMemberJoin?.Invoke(sender);
		}

		private bool IsBanned(string sender) => sender.ToLower() == "nightbot";


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
				this.userName = userName;
				this.joinTime = joinTime;
			}
		}
	}
}