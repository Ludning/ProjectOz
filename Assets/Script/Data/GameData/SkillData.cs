using System;

[Serializable]
public class SkillData : IDataRepository
{
    public string ID;
    public string skillSet;
    public string description;
    public string skillRangeType;
    public float skillPowerRate;
    public float skillCooltime;
    public float value1;
    public float value2;
    public string projectileID;
}
