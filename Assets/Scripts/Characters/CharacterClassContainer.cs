//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
	/// <summary>
	///CharacterClassContainer full description
	/// </summary>
	[CreateAssetMenu(fileName = "CharacterClassContainer", menuName = "Character Class Container")]
	public class CharacterClassContainer : ScriptableObject
	{
		public List<CharacterClass> classes;
	}
}