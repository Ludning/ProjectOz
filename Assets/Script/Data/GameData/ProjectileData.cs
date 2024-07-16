using System;

[Serializable]
public class ProjectileData : IDataRepository
{
    public string projectileID;
    public string description;
    public string projectileType;
    public float projectileSpeedRate;
    public float projectileLifeTime;
    public string projectileRemoveLandSet;
    public int projectileBounceCount;
    public string projectileRemoveTrigger2;
    public string projectileRemoveTrigger3;
    public string projectileTarget;
    public bool childColliderCheck;
    public string childColliderID_1;
    public string childColliderID_2;
}
