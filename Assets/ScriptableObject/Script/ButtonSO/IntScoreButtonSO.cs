using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "IntScoreButtonSO", menuName = "Button/IntScoreButtonSO")]
    public class IntScoreButtonSO : GenericButtonSO<int> 
    {
        public override void Trigger()
        {
            GenericEventSO<int> e =  (GenericEventSO<int>)EventSO;
            e.Value += modifier;
        }
    }
}