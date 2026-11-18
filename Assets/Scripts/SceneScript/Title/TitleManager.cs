using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] int arrow_max = 1;
    [SerializeField] int arrow_min = 0;

    // 無操作時間
    float timer = 0f;

    // 20秒で自動遷移
    [SerializeField] float autoTransitionTime = 20f;

    // SE用
    [SerializeField] List<AudioClip> se;
    public AudioSource audioSource;

    bool isChangingScene = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // タイマーを初期化
        timer = 0f;
    }

    void Update()
    {
        // シーン遷移中はタイマーを動かさない
        if (isChangingScene) return;

        timer += Time.deltaTime;

        // 20秒経過したらプロローグへ
        if (timer >= autoTransitionTime)
        {
            isChangingScene = true;
            SceneManager.LoadScene(SceneName.PROLOGUE);
        }
    }

    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (!ctx.performed) return;

        if (TextArrow.instance.arrow_pos == 0)
        {
            StartCoroutine(
                PlaySEAndLoadScene(se[1], SceneName.CHARACTER_PICK_PVP)
            );
        }
        else if (TextArrow.instance.arrow_pos == 1)
        {
            StartCoroutine(
                PlaySEAndLoadScene(se[1], SceneName.CHARACTER_PICK_PVE)
            );
        }
    }

    IEnumerator PlaySEAndLoadScene(AudioClip clip, string sceneName)
    {
        isChangingScene = true;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void Up(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;

        if (ctx.performed)
        {
            timer = 0f;

            audioSource.PlayOneShot(se[0]);

            TextArrow.instance.arrow_pos++;

            if (TextArrow.instance.arrow_pos > arrow_max)
                TextArrow.instance.arrow_pos = arrow_min;
        }
    }

    public void Down(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;

        if (ctx.performed)
        {
            timer = 0f;

            audioSource.PlayOneShot(se[0]);

            TextArrow.instance.arrow_pos--;

            if (TextArrow.instance.arrow_pos < arrow_min)
                TextArrow.instance.arrow_pos = arrow_max;
        }
    }
}