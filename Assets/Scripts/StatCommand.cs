using System.Collections;
using System.Collections.Generic;
using System.Text;
using Characters;
using TwitchIntegration;
using UnityEngine;

public class StatCommand : MonoBehaviour
{
	private void OnEnable() => IRCParser.OnPRIVMSG += ParseMessage;

	private void OnDisable() => IRCParser.OnPRIVMSG -= ParseMessage;


	private void ParseMessage(string sender, string message)
	{
		if (message.Contains("!stats"))
		{
			OutputStats(sender);
		}
	}

	private void OutputStats(string sender)
	{
		var character = CharacterManager.GetCharacterByUserName(sender);
		if (character == null) return;
		var stats = character.GetCharacterStats();
		if (stats == null) return;
		StringBuilder sb = new StringBuilder();
		sb.Append("@" + character.GetUserName());
		sb.Append(" You are level: " + stats.currentLevel);
		sb.Append(" W/L: " + stats.wins + "/" + stats.loses);

		TwitchCore.Instance.PRIVMSGTToTwitch(
			sb.ToString());
	}
}