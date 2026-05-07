using UnityEngine;

[CreateAssetMenu(menuName = "Character/CharData")]
public class CharData : ScriptableObject
{
    public int speed = 0;
    public int burst = 0;
    public int max_burst = 100;
}