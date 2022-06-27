using System.Collections;
using System.Collections.Generic;
using Characters;
using Control;
using UnityEngine;

public class WinLoseTracking : MonoBehaviour
{
    private void OnEnable() => FightController.OnFightOver += FightOver;
    private void OnDisable() => FightController.OnFightOver -= FightOver;
    private void FightOver(Character winner, Character loser)
    {
        winner.GetCharacterStats().wins++;
        loser.GetCharacterStats().loses++;
    }
}
