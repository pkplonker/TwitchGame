//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TwitchIntegration;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	/// <summary>
	///IRCParserEditor full description
	/// </summary>
	[CustomEditor(typeof(IRCParser))]
	public class IRCParserEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Join Testers", EditorStyles.miniButton))
					IRCParser.JoinTesters();
				if (GUILayout.Button("Fight Testers", EditorStyles.miniButton))
					IRCParser.FightTesters();
			}

			base.OnInspectorGUI();
		}
		
		[MenuItem("TwitchGame/Test Join", false, 0)]
		public static void JoinTesters()
		{
			IRCParser.JoinTesters();

		}


		[MenuItem("TwitchGame/Test Fight", false, 0)]
		public static void FightTesters()
		{
			IRCParser.FightTesters();

		}
	}
}