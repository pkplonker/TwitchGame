using System;
using StuartHeathTools;
using TMPro;
using Unity.Netcode;
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

		private void Start() => joinCodeText.text = joinCodeMessage + " TBC";

		public void InputFieldUpdated() => GetInput();
		private string GetInput() => inputField.text;

		public async void JoinPrivate()
		{
			var s = GetInput();
			if (string.IsNullOrWhiteSpace(s)) return;
			try
			{
				if (MultiplayerGameConnection.Instance.IsRelayEnabled) await MultiplayerGameConnection.Instance.JoinRelay(s);
				else Logger.Instance.LogError("Err here");
				if (NetworkManager.Singleton.StartClient()) Logger.Instance.Log("Started Client");
				else Logger.Instance.LogError("Unable to start host");
				
			}
			catch (Exception e)
			{
				Debug.LogError("Unable to join requested server" + e);
				throw;
			}
		}

		public async void HostPrivate()
		{
			try
			{
				if (MultiplayerGameConnection.Instance.IsRelayEnabled)
				{
					var hostData = await MultiplayerGameConnection.Instance.SetupRelay();
					joinCodeText.text = joinCodeMessage +
					                    (hostData.JoinCode).WithColor(codeJoinColor);
				}
				else Logger.Instance.LogError("Err here2");
				if (NetworkManager.Singleton.StartHost()) Logger.Instance.Log("started host");
				else Logger.Instance.LogError("Unable to start host");
				
			}
			catch (Exception e)
			{
				Debug.Log("Failed to create private gam " + e);
			}
		}

		public void Stop()
		{
		}
	}
}