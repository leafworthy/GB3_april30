using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
    [ExecuteInEditMode]
    public class DoorAnimEvents : MonoBehaviour
    {
        [FormerlySerializedAs("door")] public DoorInteraction doorInteraction;

        private void OnEnable()
        {
            doorInteraction = GetComponentInParent<DoorInteraction>();
        }

  

        public void DoorOpenFinish()
        {
            doorInteraction.Animation_OnOpenDoor(true);
        }

        public void DoorCloseFinish()
        {
            doorInteraction.Animation_OnOpenDoor(false);
        }
    }
}
