namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_GetNextTarget : Action
    {
        public SharedTransformList targetList;
        public SharedInt currentTargetIndex;
        public SharedTransform currentTarget;

        public override void OnStart()
        {
            SetNextTarget();
        }
        private void SetNextTarget()
        {
            bool isNextValid = currentTargetIndex.Value + 1 < targetList.Value.Count;

            if (isNextValid == false)
            {
                currentTargetIndex.Value = 0;
            }
            else
            {
                currentTargetIndex.Value = currentTargetIndex.Value + 1;
            }
            currentTarget.Value = targetList.Value[currentTargetIndex.Value];
        }
    }

}