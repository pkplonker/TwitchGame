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
		var ch = CharacterManager.GetCharacterByUserName(sender);
		if (ch == null)
		{
			TwitchCore.Instance.PRIVMSGTToTwitch(
				"You need to !join first to be able to see your stats");
			return;
		}

		var stats = ch.GetCharacterStats();
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
		sb.Append(". Your current win streak is " + stats.currentWinStreak);
		sb.Append(". Your best win streak is " + stats.bestWinStreak);

		var nextLevel = stats.GetNextLevel();
		if (nextLevel == -1)
			sb.Append(". You are the max level achievable. https://bit.ly/3u4wvSD");
		else
			sb.Append(" You need " + stats.ExperienceRequiredForNextLevel() + " more exp to reach level " +
			          (nextLevel + 1));


		TwitchCore.Instance.PRIVMSGTToTwitch(
			sb.ToString());
	}
}