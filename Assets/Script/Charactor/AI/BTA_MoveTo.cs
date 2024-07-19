using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_MoveTo : Action
    {
        public SharedVector3 targetPostion;
        public NavMeshAgent agent;
        public Enemy enemy;
        public bool isDynamicDestination = false;

        public override void OnAwake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        public override void OnStart()
        {
            SetMovable(agent, true);
            MoveToTarget2D(agent, targetPostion.Value);
        }

        public override TaskStatus OnUpdate()
        {
            if(isDynamicDestination)
            {
                targetPostion.Value = enemy.GetTargetPosition();
            }
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
            MoveToTarget2D(agent, targetPostion.Value);
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

        private void MoveToTarget2D(NavMeshAgent agent, Vector3 target)
        {
            target.z = 0f;
            agent.destination = target;
        }
    }
}