using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check IsEnemy Attackable")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsAttackable : Conditional
    {
        public Enemy owner;

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

            if (owner.IsAttackable())
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
