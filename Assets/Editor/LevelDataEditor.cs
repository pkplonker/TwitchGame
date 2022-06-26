//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using UnityEditor;
using UnityEngine;

namespace Editor
{
	/// <summary>
	///LevelDataEditor full description
	/// </summary>
	[CustomEditor(typeof(LevelData))]
	public class LevelDataEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var ld = (LevelData) target;
			if (GUILayout.Button("GenerateLevelData"))
			{
				ld.CalculateLevelArray();
			}

			base.OnInspectorGUI();
		}
	}
}