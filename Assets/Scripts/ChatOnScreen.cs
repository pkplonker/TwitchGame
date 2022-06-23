using TMPro;
using UnityEngine;

public class ChatOnScreen : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI prefab;
	[SerializeField] private Transform container;

	private void OnEnable() => IRCParser.OnPRIVMSG += HandleNewTwitchMessage;
	private void OnDisable() => IRCParser.OnPRIVMSG -= HandleNewTwitchMessage;

	private void HandleNewTwitchMessage(string user, string message)
	{
		var go = Instantiate(prefab, container);
		go.text = user + ": " + message;
	}
}