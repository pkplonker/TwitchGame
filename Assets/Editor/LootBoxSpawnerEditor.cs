 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //
using UnityEngine;
using UnityEditor;

    /// <summary>
    ///LootBoxSpawnerEditor full description
    /// </summary>
    [CustomEditor(typeof(LootBoxSpawner))]
public class LootBoxSpawnerEditor : UnityEditor.Editor
{
   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();
      var lootBoxSpawner = (LootBoxSpawner)target;
      if (GUILayout.Button("Spawn Loot Box"))
      {
	      lootBoxSpawner.timer = lootBoxSpawner.currentTargetTime;
      }
   }
}
