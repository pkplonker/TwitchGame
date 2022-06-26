using System;
using TMPro;
using TwitchIntegration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TwitchLogin : MonoBehaviour
	{
	
		[SerializeField] private TMP_InputField channel;

		private void Awake() => LoadValues();

		//ui
		public void OnUpdatedInput()
		{
			if (string.IsNullOrWhiteSpace(channel.text)) return;
			Debug.Log("updating login details");
			TwitchCore.Instance.UpdateChannel( channel.text);
			SaveValues();
		}

		private void SaveValues()
		{
			
			PlayerPrefs.SetString("channel", channel.text);
			PlayerPrefs.Save();
		}

		private void LoadValues()
		{
			
			channel.text = PlayerPrefs.GetString("channel");
			TwitchCore.Instance.UpdateChannel( channel.text);
		}
	}
}