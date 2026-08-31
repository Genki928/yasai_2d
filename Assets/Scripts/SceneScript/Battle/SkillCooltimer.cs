using UnityEngine;
using UnityEngine.UI;

public class SkillCooltimer : MonoBehaviour
{
    Image _sprite;
    AudioSource _audioSource;

    void Start()
    {
        _sprite = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary> UI‚ð“¯Šú‚³‚¹‚é </summary>
    public void RefreshCooltimer(Cooltime cooltime) => _sprite.fillAmount = 1 - cooltime.Ratio;
}
