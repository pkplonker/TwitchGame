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

		private void Start() => joinCodeText.text = joinCodeMessage + " TBC";

		public void InputFieldUpdated() => GetInput();
		private string GetInput() => inputField.text;

		public async void JoinPrivate()
		{
			var s = GetInput();
			if (string.IsNullOrWhiteSpace(s)) return;
			try
			{
				await MultiplayerGameConnection.Instance.JoinRelay(s);
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
				var hostData = await MultiplayerGameConnection.Instance.SetupRelay();
				joinCodeText.text = joinCodeMessage +
				                    (hostData.JoinCode).WithColor(codeJoinColor);
			}
			catch (Exception e)
			{
				Debug.Log("Failed to create private gam " + e);
			}
		}
	}
}