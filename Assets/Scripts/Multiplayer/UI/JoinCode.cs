using UnityEngine;
using UnityEngine.EventSystems;

namespace Multiplayer.UI
{
	public class JoinCode : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private HostJoinButtons hostJoinButtons;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left) return;
			CopyToClipboard(hostJoinButtons.GetCode());
		}

		public static void CopyToClipboard(string str)
		{
			GUIUtility.systemCopyBuffer = str;
		}
	}
}