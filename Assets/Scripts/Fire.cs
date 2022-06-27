using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Control;
using UnityEngine;

public class Fire : MonoBehaviour
{
	private Animator animator;
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	private void FightOver(Character arg1, Character arg2) =>  SetFlame(false);
	private void FightStart() =>  SetFlame(true);
	private void Start() => SetFlame(false);

	private void SetFlame(bool s)
	{
		animator.enabled = s;
		if (!s) spriteRenderer.sprite = null;
	}


	private void OnEnable()
	{
		FightController.OnFightOver += FightOver;
		FightController.OnFightStart += FightStart;
	}

	private void OnDisable()
	{
		FightController.OnFightOver -= FightOver;
		FightController.OnFightStart -= FightStart;
	}
}