using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_MoveTo : Action
    {
        public SharedFloat moveSpeed;
        public SharedTransform targetPostion;
        public NavMeshAgent agent;

        public override void OnStart()
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.destination = targetPostion.Value.position;
        }

        public override TaskStatus OnUpdate()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        public override void OnEnd()
        {
            agent.isStopped = true;
        }
    }
}