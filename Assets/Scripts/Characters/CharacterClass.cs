 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using UnityEngine;

 namespace Characters
 {
	 /// <summary>
	 ///CharacterClass full description
	 /// </summary>
	 [CreateAssetMenu(fileName = "Character Class",menuName = "Character Class")]

	 public class CharacterClass : ScriptableObject
	 {
		 [SerializeField] private string className;
		 public Sprite sprite;
		 [SerializeField] private string animationControllerPath;
		 public string GetClassName() => className.ToLower();
		 public string GetAnimationControllerPath() => animationControllerPath;
	 }
	 
 }
