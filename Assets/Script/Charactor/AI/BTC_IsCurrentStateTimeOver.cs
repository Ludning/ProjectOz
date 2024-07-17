using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check Is Current State Time Over")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsCurrentStateTimeOver : Conditional
    {
        public Enemy owner;
        public SharedFloat stateTime;
        public override void OnAwake()
        {
            owner = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (owner == null)
            {
                return TaskStatus.Failure;
            }
            if (owner.CurrentStateTime >= stateTime.Value)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
