using UnityEngine;

[CreateAssetMenu(menuName = "Character/PickData")]
public class PickData : ScriptableObject
{
    public string char_name;
    [TextArea(4,10)]
    public string lore;
}