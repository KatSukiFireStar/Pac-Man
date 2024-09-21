using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "Vector2ButtonSO", menuName = "Button/Vector2ButtonSO")]
    public class Vector2ButtonSO : GenericButtonSO<Vector2> 
    {
        public override void Trigger()
        {
            GenericEventSO<Vector2> e =  (GenericEventSO<Vector2>)EventSO;
            e.Value = modifier;
        }
    }
}