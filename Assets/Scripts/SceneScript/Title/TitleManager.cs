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
    [SerializeField] int timer = 0;
    //seóp
    [SerializeField] List<AudioClip> se;
    public AudioSource audioSource;

    bool isChangingScene = false; // ìÒèdëJà⁄ñhé~

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer++;
        if (timer / 60 == 20) SceneManager.LoadScene(SceneName.PROLOGUE);
    }

    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;

        if (ctx.performed && TextArrow.instance.arrow_pos == 0)
        {
            StartCoroutine(PlaySEAndLoadScene(se[1], SceneName.CHARACTER_PICK_PVP));
        }
        if (ctx.performed && TextArrow.instance.arrow_pos == 1)
        {
            StartCoroutine(PlaySEAndLoadScene(se[1], SceneName.CHARACTER_PICK_PVE));
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
            audioSource.PlayOneShot(se[0]);
            TextArrow.instance.arrow_pos++;
            if (TextArrow.instance.arrow_pos > arrow_max)
                TextArrow.instance.arrow_pos = arrow_min;
            timer = 0;
        }
    }

    public void Down(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (ctx.performed)
        {
            audioSource.PlayOneShot(se[0]);
            TextArrow.instance.arrow_pos--;
            if (TextArrow.instance.arrow_pos < arrow_min)
                TextArrow.instance.arrow_pos = arrow_max;
            timer = 0;
        }
    }
}