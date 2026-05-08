using UnityEngine;

[CreateAssetMenu(menuName = "Character/CharData")]
public class CharData : ScriptableObject
{
    public int speed = 0;
    public int max_burst = 100;

    [Header("Skill 1")]
    public int skill_1_rigid = 0;
    public int skill_1_cooltime = 0;

    [Header("Skill 2")]
    public int skill_2_rigid = 0;
    public int skill_2_cooltime = 0;

    [Header("Skill 3")]
    public int skill_3_rigid = 0;
    public int skill_3_cooltime = 0;
}