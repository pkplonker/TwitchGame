using System.Collections;
using System.Collections.Generic;
using System.Text;
using Characters;
using TwitchIntegration;
using UnityEngine;

public class StatCommand : MonoBehaviour
{
	[SerializeField] private CharacterManager characterManager;
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
		var stats = characterManager.GetOfflineCharacterByUserName(sender);
		if (stats == null)
		{
			TwitchCore.Instance.PRIVMSGTToTwitch(
				"You need to !join first to be able to see your stats");
			return;
		}

		var sb = new StringBuilder();
		sb.Append("@" + stats.userName);
		sb.Append(" You are a level " + stats.currentLevel + " " + stats.characterClass.GetClassName());
		sb.Append(" W/L: " + stats.wins + "/" + stats.loses);
		var nextLevel = stats.GetNextLevel();
		if (nextLevel == -1)
			sb.Append(". You are the max level achievable. https://bit.ly/3u4wvSD");
		else
			sb.Append(" You need " + stats.ExperienceRequiredForNextLevel() + " more exp to reach level " +
			          (nextLevel+1));


		TwitchCore.Instance.PRIVMSGTToTwitch(
			sb.ToString());
	}
}