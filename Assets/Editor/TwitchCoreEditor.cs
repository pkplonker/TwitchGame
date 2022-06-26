using TwitchIntegration;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(TwitchCore))]
	public class TwitchCoreEditor : UnityEditor.Editor
	{
		private static string message;
		public override void OnInspectorGUI()
		{
			message = EditorGUILayout.TextField("Message",message);
			if (GUILayout.Button("Send Message"))
				TwitchCore.Instance.PRIVMSGTToTwitch(message);
			base.OnInspectorGUI();
		}
	}
}