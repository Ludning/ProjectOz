using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_MoveTo : Action
    {
        public SharedTransform targetPostion;
        public NavMeshAgent agent;
        public bool isDynamicDestination = false;

        public override void OnAwake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            SetMovable(agent, true);
            MoveToTarget(agent, targetPostion.Value.position);
        }

        public override TaskStatus OnUpdate()
        {
            //네브메시가 경로 계산중인지 확인 해야함
            if (agent.pathPending == true)
            {
                return TaskStatus.Running;
            }

            bool isArrived = agent.remainingDistance <= agent.stoppingDistance;
            if (isArrived)
            {
                return TaskStatus.Success;
            }
            MoveToTarget(agent, targetPostion.Value.position);
            return TaskStatus.Running;
        }
        public override void OnEnd()
        {
            agent.isStopped = true;
        }



        private void SetMovable(NavMeshAgent agent, bool isMovable)
        {
            agent.enabled = isMovable;
            agent.isStopped = !isMovable;
        }

        private void MoveToTarget(NavMeshAgent agent, Vector3 target)
        {
            agent.destination = targetPostion.Value.position;
        }
    }
}