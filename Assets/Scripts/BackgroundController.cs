using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
	private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

	private void Awake()
	{
		foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
		{
			spriteRenderers.Add(sr);
		}
	}

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

	public void ShowBackground() => EnableAllSpriteRenderers();
	public void HideBackground() => DisableAllSpriteRenderers();

	public void ShowFloorOnly()
	{
		DisableAllSpriteRenderers();
		EnableSpriteRenderer(0);
	}
}