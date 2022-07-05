using System;
using StuartHeathTools;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Multiplayer.UI
{
	public class HostJoinButtons : CanvasGroupBase
	{
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TextMeshProUGUI joinCodeText;
		[SerializeField] private string joinCodeMessage = "Your Join code is:";
		[SerializeField] private Color codeJoinColor;
		private string code;
		public void Close() => Hide();

		private void Start() => joinCodeText.text = joinCodeMessage + " TBC";

		public void InputFieldUpdated() => GetInput();
		private string GetInput() => inputField.text;

		public async void JoinPrivate()
		{
			var data = new RelayJoinData();
			var s = GetInput();
			if (string.IsNullOrWhiteSpace(s)) return;
			try
			{
				if (MultiplayerGameConnection.Instance.IsRelayEnabled)
					data = await MultiplayerGameConnection.Instance.JoinRelay(s);
				else Logger.Instance.LogError("Err here");
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(data.IPv4Address, data.Port,
					data.AllocationIDBytes, data.Key, data.ConnectionData, data.HostConnectionData);
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
				RelayHostData data = new RelayHostData();
				if (MultiplayerGameConnection.Instance.IsRelayEnabled)
				{
					data = await MultiplayerGameConnection.Instance.SetupRelay();
					code = data.JoinCode;
					joinCodeText.text = joinCodeMessage +
					                    (code).WithColor(codeJoinColor);
				}
				else Logger.Instance.LogError("Err here2");

				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data.IPv4Address, data.Port,
					data.AllocationIDBytes, data.Key, data.ConnectionData);
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
			NetworkManager.Singleton.Shutdown();
		}


		public string GetCode() => code;
	}
}