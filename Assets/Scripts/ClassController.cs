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
		if (message == commands.GetClassesCommand())
		{
			var sb = new StringBuilder();
			sb.Append("The available classes are");
			foreach (var c in characterClassContainer.classes)
			{
				sb.Append(", ");
				sb.Append(CapitaliseFirstLetter(c.GetClassName()));
			}

			sb.Append(".");
			TwitchCore.Instance.PRIVMSGTToTwitch(sb.ToString());
		}
		else if (message.Contains((commands.GetClassCommand() + " ")) ||
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
	}

	private static string CapitaliseFirstLetter(string s)
	{
		var x = s[0].ToString().ToUpper();
		return x + s.Remove(0, 1);
	}

	private string StripCommand(string message) => message.Replace(commands.GetClassCommand() + " ", "");
}