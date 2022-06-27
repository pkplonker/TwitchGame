//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TwitchIntegration;
using UnityEngine;

namespace Control
{
	/// <summary>
	///Commands full description
	/// </summary>
	[CreateAssetMenu(fileName = "New Commands", menuName = "Twitch Game/Commands")]
	public class Commands : ScriptableObject
	{
		[SerializeField] private string commandInitialChar;
		[SerializeField] private string joinCommand;
		[SerializeField] private string leaveCommand;
		[SerializeField] private string fightCommand;
		[SerializeField] private string moveCommand;
		[SerializeField] private string commandsCommand;

		private void OnEnable()
		{
			IRCParser.OnPRIVMSG += OnMessage;
		}

		private void OnMessage(string sender, string message)
		{
			if (message.Contains(GetCommandCommand()))
			{
				TwitchCore.Instance.PRIVMSGTToTwitch("https://pkplonker.github.io/TwitchGame/");
			}
		}

		public string GetJoinCommand() => commandInitialChar + joinCommand;
		public string GetLeaveCommand() => commandInitialChar + leaveCommand;
		public string GetFightCommand() => commandInitialChar + fightCommand;
		public string GetMoveCommand() => commandInitialChar + moveCommand;
		public string GetCommandCommand() => commandInitialChar + commandsCommand;
	}
}