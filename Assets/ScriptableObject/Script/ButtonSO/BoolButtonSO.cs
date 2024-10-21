using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "BoolButtonSO", menuName = "Button/BoolButtonSO")]
    public class BoolButtonSO : GenericButtonSO<bool> 
    {
        public override void Trigger()
        {
            GenericEventSO<bool> e =  (GenericEventSO<bool>)EventSO;
            e.Value = modifier;
        }
    }
}