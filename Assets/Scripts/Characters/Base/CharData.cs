using UnityEngine;

[CreateAssetMenu(menuName = "Character/CharData")]
public class CharData : ScriptableObject
{
    public string char_name;
    public int speed = 0;
    public int max_burst = 100;

    [Header("©“®‰ñ•œ")]
    public float regen_burst_cooltime = 120;
    public float restart_regen_burst_value = 60;

    [Header("Skill 1")]
    public float skill_1_rigid = 0;
    public float skill_1_cooltime = 0;

    [Header("Skill 2")]
    public float skill_2_rigid = 0;
    public float skill_2_cooltime = 0;

    [Header("Skill 2")]
    public float dash_cooltime = 0;
}