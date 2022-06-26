using System;
using TMPro;
using TwitchIntegration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TwitchLogin : MonoBehaviour
	{
		[SerializeField] private TMP_InputField username;
		[SerializeField] private TMP_InputField password;
		[SerializeField] private TMP_InputField channel;

		private void Awake() => LoadValues();

		//ui
		public void OnUpdatedInput()
		{
			if (string.IsNullOrWhiteSpace(username.text) && string.IsNullOrWhiteSpace(password.text) &&
			    string.IsNullOrWhiteSpace(channel.text)) return;

			Debug.Log("updating login details");
			TwitchCore.Instance.UpdateLogin(username.text, password.text, channel.text);
			SaveValues();
		}

		private void SaveValues()
		{
			PlayerPrefs.SetString("username", username.text);
			PlayerPrefs.SetString("pass", password.text);
			PlayerPrefs.SetString("channel", channel.text);
			PlayerPrefs.Save();
		}

		private void LoadValues()
		{
			username.text = PlayerPrefs.GetString("username");
			password.text = PlayerPrefs.GetString("pass");
			channel.text = PlayerPrefs.GetString("channel");
			TwitchCore.Instance.UpdateLogin(username.text, password.text, channel.text);
		}
	}
}