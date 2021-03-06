//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Characters;
using StuartHeathTools;
using TMPro;
using TwitchIntegration;
using UnityEngine;

namespace UI
{
	/// <summary>
	///CanvasGroup full description
	/// </summary>
	public class SettingsPanel : CanvasGroupBase
	{
		[SerializeField] private TextMeshProUGUI statusText;
		[SerializeField] private Highscores highscores;
		private void Awake() => canvasGroup = GetComponent<CanvasGroup>();
		private void Start() => Hide();
		private void OnEnable() => TwitchCore.OnConnectionStatusChange += ConnectionStateChanged;
		private void OnDisable() => TwitchCore.OnConnectionStatusChange -= ConnectionStateChanged;


		public void Connect() => TwitchCore.Instance.AttemptConnection(); //ui button
		public void Open() => Show(); //ui button
		public void Close() => Hide(); //ui button
		public void ShowHighscores() => highscores.Open();


		private void ConnectionStateChanged(ConnectionState state)
		{
			statusText.text = state switch
			{
				ConnectionState.Disconnected => "Status:Disconnected",
				ConnectionState.ConnectionConfirmed => "Status:Connection Confirmed",
				ConnectionState.ConnectionLost => "Status:Connection Lost",
				ConnectionState.Connecting => "Status:Connecting",
				_ => "Error"
			};
		}

		private void Update()
		{
			if (!Input.GetKeyDown(KeyCode.Escape)) return;
			if (canvasGroup.alpha == 0) Open();
			else Close();
		}


		public void Quit()
		{
			foreach (var character in CharacterManager.characters) character.SaveState();
			Debug.Log("Quitting");
			CharacterManager.Instance.SaveAllCharacters();
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
		
		public void OAuth()=>Application.OpenURL("https://twitchapps.com/tmi/");
		
	}
}