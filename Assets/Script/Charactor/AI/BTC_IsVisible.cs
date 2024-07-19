using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check Is Enemy can see target")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsVisible : Conditional
    {
        public Enemy owner;
        public bool isAttack;

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
            if(owner.IsTargetVisible(isAttack))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
