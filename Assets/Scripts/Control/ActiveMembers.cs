//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using TwitchIntegration;
using UnityEngine;

namespace Control
{
	public class ActiveMembers : MonoBehaviour
	{
		[SerializeField] private List<ActiveMember> activeMembers = new List<ActiveMember>();
		[SerializeField] private Commands commands;
		[SerializeField] private float timeOutMinutes;
		public static event Action<string> OnMemberJoin;
		public static event Action<string> OnMemberLeave;

		private void Start() => StartCoroutine(CheckTimeouts());
		protected virtual void OnEnable() => IRCParser.OnPRIVMSG += OnMessage;
		protected virtual void OnDisable() => IRCParser.OnPRIVMSG += OnMessage;

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

		private ActiveMember FindByUsername(string sender) => activeMembers.Find(x => x.userName == sender);


		private void MemberJoin(string sender)
		{
			if (IsBanned(sender)) return;
			activeMembers.Add(new ActiveMember(sender, Time.time));
			OnMemberJoin?.Invoke(sender);
		}

		private bool IsBanned(string sender)
		{
			return sender.ToLower() == "nightbot";
		}

		private void RemoveMember(ActiveMember am)
		{
			activeMembers.Remove(am);
			OnMemberLeave?.Invoke(am.userName);
		}


		[Serializable]
		private class ActiveMember
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