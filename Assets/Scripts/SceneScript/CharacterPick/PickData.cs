using UnityEngine;

[CreateAssetMenu(menuName = "Character/PickData")]
public class PickData : ScriptableObject
{
    [Header("Бе Character Name")]
    public string char_name;

    [Header("Бе Skill 1")]
    public string skill1Name;
    [TextArea(3, 10)] public string lore1;

    [Header("Бе Skill 2")]
    public string skill2Name;
    [TextArea(3, 10)] public string lore2;
}