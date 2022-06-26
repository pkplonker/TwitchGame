using TMPro;
using TwitchIntegration;
using UnityEngine;


namespace UI
{
	public class TwitchLogin : MonoBehaviour
	{
		[SerializeField] private TMP_InputField channel;
		[SerializeField] private TMP_InputField username;
		[SerializeField] private TMP_InputField password;

		private void Awake() => LoadValues();

		//ui
		public void OnUpdatedInput()
		{
			if (string.IsNullOrWhiteSpace(channel.text) || string.IsNullOrWhiteSpace(username.text) ||
			    string.IsNullOrWhiteSpace(password.text)) return;
			Debug.Log("updating login details");
			TwitchCore.Instance.UpdateChannel(channel.text, password.text, username.text);
			SaveValues();
		}


		private void SaveValues()
		{
			PlayerPrefs.SetString("username", username.text);
			PlayerPrefs.SetString("password", password.text);
			PlayerPrefs.SetString("channel", channel.text);
			PlayerPrefs.Save();
		}

		private void LoadValues()
		{
			username.text = PlayerPrefs.GetString("username");
			password.text = PlayerPrefs.GetString("password");
			channel.text = PlayerPrefs.GetString("channel");
			TwitchCore.Instance.UpdateChannel(channel.text, password.text, username.text);
		}
	}
}