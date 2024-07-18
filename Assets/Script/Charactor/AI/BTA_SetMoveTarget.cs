using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_SetMoveTarget : Action
    {
        public SharedVector3 targetPosition;
        public SharedTransform target;
        public Enemy owner;
        public bool isDynamicDestination = false;

        public override void OnAwake()
        {
            owner = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (owner == null)
            {
                Debug.LogWarning("Unable to compare field - compare value is null");
                return TaskStatus.Failure;
            }
            targetPosition.Value = owner.GetTargetPosition();
            target.Value = owner.GetTarget();
            return TaskStatus.Success;
        }
    }
}