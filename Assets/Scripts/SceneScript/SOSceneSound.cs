using UnityEngine;

public class AudioAssets : ScriptableObject { }

[CreateAssetMenu(menuName = "AudioAssets/CharacterSelect")]
public class CharacterSelectAA : AudioAssets
{
    public AudioClip CursorMoveSE;
    public AudioClip CancelSE;
    public AudioClip SelectSE;
}