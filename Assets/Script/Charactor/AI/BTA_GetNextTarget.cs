namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_GetNextTarget : Action
    {
        public SharedTransformList targetList;
        public SharedInt currentTargetIndex;
        public SharedVector3 currentTargetPos;

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
            currentTargetPos.Value = targetList.Value[currentTargetIndex.Value].position;
        }
    }

}