using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Control;
using TwitchIntegration;
using UnityEngine;

public class AdminController : MonoBehaviour
{
	[SerializeField] private ExpHandler expHandler;
	private void OnEnable() => IRCParser.OnPRIVMSG += ParseMessage;
	[SerializeField] private List<string> admins;

	private void OnDisable() => IRCParser.OnPRIVMSG -= ParseMessage;


	private void ParseMessage(string sender, string message)
	{
		if (message.Contains("!setlevel") && IsValidAdmin(sender)) SetLevel(sender, message);
	}

	private void SetLevel(string sender, string message)
	{
		var level = message.Replace("!setlevel", "");
		if (int.TryParse(level, out var intValue))
		{
			var character = CharacterManager.GetCharacterByUserName(sender.ToLower());
			if (character == null) Debug.LogWarning("Unable to find character");
			else
			{
				Debug.Log("");
				expHandler.SetLevel(character, intValue);
			}
		}
		else
		{
			Debug.LogWarning("unable to parse level change request");
		}
	}

	private bool IsValidAdmin(string sender)
	{
		sender = sender.ToLower();
		if (admins.Any(a => a == sender)) return true;
		TwitchCore.Instance.PRIVMSGTToTwitch("This is an admin only command, nice try.");
		return false;
	}
}