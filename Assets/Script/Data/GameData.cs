using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameDataType
{
    Pc,
    Enemy,
    Moveset,
    Resource,
    Skill,
    Ozmagic,
    Projectile,
    Level,
}
[Serializable]
public class GameData
{
    public Dictionary<string, PcData> PcDatas;
    public Dictionary<string, EnemyData> EnemyDatas;
    public Dictionary<string, MovesetData> MovesetDatas;
    public Dictionary<string, ResourceData> ResourceDatas;
    public Dictionary<string, SkillData> SkillDatas;
    public Dictionary<string, OzmagicData> OzmagicDatas;
    public Dictionary<string, ProjectileData> ProjectileDatas;
    public Dictionary<string, LevelData> LevelDatas;
}
