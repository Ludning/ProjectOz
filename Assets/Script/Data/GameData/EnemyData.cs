using System;

[Serializable]
public class EnemyData : IDataRepository
{
    public string ID;
    public string description;
    public string enemyMoveType;
    public string enemyAttackType;
    public float enemyHp;
    public float enemyBasePower;
    public float enemyMoveSpeed;
    public string moveSet;
    public string skillSet;
    public float enemyColDamage;
}
