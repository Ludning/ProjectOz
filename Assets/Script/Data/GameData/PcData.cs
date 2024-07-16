using System;

[Serializable]
public class PcData : IDataRepository
{
    public string ID;
    public string description;
    public string pcType;
    public int pcHp;
    public float pcBasePower;
    public float pcMoveSpeed;
    public string moveSet;
    public string skillSet;
    public string ozMagicSet;
    public string resourceID;
    public float pcIgnoreColTime;
}
