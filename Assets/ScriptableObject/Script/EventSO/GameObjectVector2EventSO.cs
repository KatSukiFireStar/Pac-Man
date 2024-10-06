using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "GameObjectVector2EventSO", menuName = "Events/GameObjectVector2EventSO")]
    public class GameObjectVector2EventSO : GenericEventSO<(GameObject, Vector2)>
    {
        
    }
}