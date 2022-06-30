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
					IRCParser.JoinTesters("x","y");
				if (GUILayout.Button("Fight Testers", EditorStyles.miniButton))
					IRCParser.FightTesters("x","y");
			}

			base.OnInspectorGUI();
		}
		
		[MenuItem("TwitchGame/Test Join XY", false, 0)]
		public static void JoinTestersXY()
		{
			IRCParser.JoinTesters("x","y");

		}
		[MenuItem("TwitchGame/Test Join PKP", false, 0)]
		public static void JoinTestersPKP()
		{
			IRCParser.JoinTesters("pkplonker", "pkplonkertest");
		}


		[MenuItem("TwitchGame/Test Fight XY", false, 0)]
		public static void FightTestersXY()
		{
			IRCParser.FightTesters("x","y");

		}
		[MenuItem("TwitchGame/Test Fight PKP", false, 0)]
		public static void FightTestersPKP()
		{
			IRCParser.FightTesters("pkplonker", "pkplonkertest");
		}
		
		
		[MenuItem("TwitchGame/Mass Test", false, 0)]
		public static void MassJoin()
		{
			IRCParser.JoinTestersMass();
		}

	}
}