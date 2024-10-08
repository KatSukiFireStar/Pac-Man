using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "GameObjectBoolEventSO", menuName = "Events/GameObjectBoolEventSO")]
    public class GameObjectBoolEventSO : GenericEventSO<(GameObject, bool)>
    {
        
    }
}