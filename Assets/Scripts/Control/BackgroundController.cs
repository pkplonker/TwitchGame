using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control
{
	public class BackgroundController : MonoBehaviour
	{
		[SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
		[SerializeField] private GameObject sky;

		private void Awake() => sky.SetActive(PlayerPrefs.GetInt("sky") != 0);


		public void ToggleSky()
		{
			sky.SetActive(!sky.activeSelf);
			PlayerPrefs.SetInt("sky", !sky.activeSelf ? 0 : 1);
		}

		public void ShowBackground() => EnableAllSpriteRenderers();
		public void HideBackground() => DisableAllSpriteRenderers();

		private void EnableSpriteRenderer(int index)
		{
			if (index > spriteRenderers.Count) return;
			spriteRenderers[index].enabled = true;
		}

		private void DisableSpriteRenderer(int index)
		{
			if (index > spriteRenderers.Count) return;
			spriteRenderers[index].enabled = true;
		}

		private void EnableAllSpriteRenderers()
		{
			foreach (var spriteRenderer in spriteRenderers) spriteRenderer.enabled = true;
		}

		private void DisableAllSpriteRenderers()
		{
			foreach (var spriteRenderer in spriteRenderers)
			{
				spriteRenderer.enabled = false;
			}
		}


		public void ShowFloorOnly()
		{
			DisableAllSpriteRenderers();
			EnableSpriteRenderer(0);
		}
	}
}