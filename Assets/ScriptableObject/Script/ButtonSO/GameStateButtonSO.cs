using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "GameStateButtonSO", menuName = "Button/GameStateButtonSO")]
    public class GameStateButtonSO : GenericButtonSO<GameState> 
    {
        public override void Trigger()
        {
            GenericEventSO<GameState> e =  (GenericEventSO<GameState>)EventSO;
            e.Value = modifier;
        }
    }
}