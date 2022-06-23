//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using UnityEngine;

/// <summary>
///CanvasGroup full description
/// </summary>
public abstract class CanvasGroupBase : MonoBehaviour
{
	[SerializeField] protected CanvasGroup canvasGroup;

	protected virtual void Show()
	{
		if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	protected virtual void Hide()
	{
		if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
}