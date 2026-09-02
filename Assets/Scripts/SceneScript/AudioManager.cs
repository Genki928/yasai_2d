using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ----- シングルトン ----- //
    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // ----- メンバ ----- //
    AudioSource _audioSource;
    [SerializeField] AudioDataSO _soundData;

    public void PlaySE(int n)
    {
        _audioSource.PlayOneShot(_soundData.SE[n].clip);
    }
}

[CreateAssetMenu(menuName = "AudioData")]
public class AudioDataSO : ScriptableObject
{
    public List<AudioData> SE;
    public List<AudioData> Bgm;
}

[Serializable]
public class AudioData
{
    public AudioClip clip;
    public string key;
}