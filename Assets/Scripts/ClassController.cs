using System.Text;
using Characters;
using Control;
using TwitchIntegration;
using UnityEngine;

public class ClassController : MonoBehaviour
{
	[SerializeField] private CharacterClassContainer characterClassContainer;
	[SerializeField] private Commands commands;
	private void OnEnable() => IRCParser.OnPRIVMSG += ParseMessage;
	private void OnDisable() => IRCParser.OnPRIVMSG -= ParseMessage;


	private void ParseMessage(string sender, string message)
	{
		if (message.Contains(commands.GetClassCommand() + " ") ||
		    CharacterManager.GetCharacterByUserName(sender) == null)
		{
			message = StripCommand(message).ToLower();
			foreach (var classs in characterClassContainer.classes)
			{
				if (message.ToLower() != classs.GetClassName().ToLower()) continue;
				var c = CharacterManager.GetCharacterByUserName(sender);
				c.ChangeClass(classs);
			}
		}
		else if (message.Contains(commands.GetClassesCommand()))
		{
			var sb = new StringBuilder();
			sb.Append("The available classes are");
			foreach (var c in characterClassContainer.classes)
			{
				sb.Append(", ");
				sb.Append(c.GetClassName());
			}

			sb.Append(".");
			TwitchCore.Instance.PRIVMSGTToTwitch(sb.ToString());
		}
	}

	private string StripCommand(string message) => message.Replace(commands.GetClassCommand() + " ", "");
}