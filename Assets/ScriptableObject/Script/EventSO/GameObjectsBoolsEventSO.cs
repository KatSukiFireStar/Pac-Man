using System.Collections.Generic;
using UnityEngine;

namespace EventSystem.SO
{
    [CreateAssetMenu(fileName = "GameObjectsBoolsEventSO", menuName = "Events/GameObjectsBoolsEventSO")]
    public class GameObjectsBoolsEventSO : GenericEventSO<Dictionary<GameObject, bool>>
    {
        
    }
}