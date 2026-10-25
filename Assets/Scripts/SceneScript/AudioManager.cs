using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // シングルトンの定義
    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;
    }
    protected IEnumerator WaitAndLoadScene(AudioClip clip, string sceneName)
    {
        if (clip != null)
        {
            yield return new WaitForSeconds(clip.length);
        }

        SceneManager.LoadScene(sceneName);
    }
}