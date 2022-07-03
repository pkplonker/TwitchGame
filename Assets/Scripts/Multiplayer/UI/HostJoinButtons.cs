using System;
using StuartHeathTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.UI
{
	public class HostJoinButtons : CanvasGroupBase
	{
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TextMeshProUGUI joinCodeText;
		[SerializeField] private string joinCodeMessage = "Your Join code is:";
		[SerializeField] private Color codeJoinColor;
		public void Close() => Hide();

		private void Start()
		{
			joinCodeText.text = joinCodeMessage + " TBC";
		}

		public void InputFieldUpdated()
		{
			GetInput();
		}

		private string GetInput() => inputField.text;


		public async void JoinPrivate()
		{
			var s = GetInput();
			if (string.IsNullOrWhiteSpace(s)) return;
			try
			{
				await MultiplayerGameConnection.Instance.HandleJoinServer(s);

			}
			catch (Exception e)
			{
				Debug.LogError("Unable to join requested server" +e);
				throw;
			}

			//todo Implement join
		}

		public async void HostPrivate()
		{
			try
			{
				await MultiplayerGameConnection.Instance.CreateMatchmakingGame(true);
				joinCodeText.text = joinCodeMessage + (MultiplayerGameConnection.Instance.lobbyCode).WithColor(codeJoinColor);
			}
			catch (Exception e)
			{
				Debug.Log("Failed to create private gam " + e);
			}
		}

		public async void JoinMatchmaking()
		{
			await MultiplayerGameConnection.Instance.HandleJoinServer();
		}
	}
}