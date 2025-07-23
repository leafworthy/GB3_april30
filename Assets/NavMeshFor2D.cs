using UnityEngine;
using UnityEngine.AI;

public class NavMeshFor2D : MonoBehaviour
{
   private NavMeshAgent agent =>  _agent ??= GetComponent<NavMeshAgent>();
   private NavMeshAgent _agent;

   private void OnEnable()
   {
	   agent.updateRotation = false;
	   agent.updateUpAxis = false;
	   transform.rotation = Quaternion.identity;
   }

   private void Update()
   {
	   transform.rotation = Quaternion.identity;
   }
}
