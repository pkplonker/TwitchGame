using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control
{
	public class BackgroundController : MonoBehaviour
	{
		[SerializeField] private GameObject props;
		[SerializeField] private GameObject floor;

		[SerializeField] private GameObject sky;

		private void Awake()
		{
			sky.SetActive(PlayerPrefs.GetInt("sky") != 0);
			floor.SetActive(PlayerPrefs.GetInt("floor") != 0);
			props.SetActive(PlayerPrefs.GetInt("props") != 0);
		} 
		
		public void ToggleSky()
		{
			sky.SetActive(!sky.activeSelf);
			PlayerPrefs.SetInt("sky", !sky.activeSelf ? 0 : 1);
		}
		public void ToggleFloor()
		{
			floor.SetActive(!floor.activeSelf);
			PlayerPrefs.SetInt("floor", !floor.activeSelf ? 0 : 1);
		}
		public void ToggleProps()
		{
			props.SetActive(!props.activeSelf);
			PlayerPrefs.SetInt("props", !props.activeSelf ? 0 : 1);
		}
		
		


		
	}
}