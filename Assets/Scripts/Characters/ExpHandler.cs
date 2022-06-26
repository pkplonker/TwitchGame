using Control;
using UnityEngine;

namespace Characters
{
   public class ExpHandler : MonoBehaviour
   {
      [SerializeField] private int winnerXp = 150;
      [SerializeField] private int loserXP = 50;
      private void OnEnable()=>FightController.OnFightOver += EarnExp;
   

      private void OnDisable()=> FightController.OnFightOver -= EarnExp;
   

      private void EarnExp(Character winner, Character loser)
      {
         winner.GetCharacterStats().EarnXP(winnerXp);
      }
   }
}
