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

    // ----- コナミコマンド ----- //
    [Header("◇コナミコマンド")]
    [Tooltip("成功時に飛ぶシーン名")]
    [SerializeField] string trainingSceneName = "TOREMO";
    [SerializeField] AudioClip konamiSe;

    enum Cmd { Up, Down, Left, Right, B, A }

    // ↑↑↓↓←→←→BA
    static readonly Cmd[] KONAMI =
    {
        Cmd.Up, Cmd.Up, Cmd.Down, Cmd.Down,
        Cmd.Left, Cmd.Right, Cmd.Left, Cmd.Right,
        Cmd.B, Cmd.A
    };
    int konamiIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // タイマーを初期化
        timer = 0f;
    }

    void Update()
    {
        // コナミコマンドは遷移判定より先に見る
        // 最後のAが決定ボタンと被っても、こちらを優先する
        if (CheckKonami()) return;

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

    /// <summary> 迫真のコナミコマンドの入力を監視する。成功してシーン遷移したら true </summary>
    bool CheckKonami()
    {
        if (!TryGetPressedCmd(out Cmd cmd)) return false;

        // 何か押している間は無操作扱いにしない
        timer = 0f;

        if (cmd == KONAMI[konamiIndex])
        {
            konamiIndex++;
        }
        else
        {
            // 間違えたら最初から(押したのが先頭の↑なら1つ目として数える)
            konamiIndex = cmd == KONAMI[0] ? 1 : 0;
        }

        if (konamiIndex < KONAMI.Length) return false;

        // 成功
        konamiIndex = 0;
        isChangingScene = true;
        StopAllCoroutines();    // 決定ボタン側の遷移待ちが走っていても止める

        if (konamiSe != null && audioSource != null)
            audioSource.PlayOneShot(konamiSe);

        SceneManager.LoadScene(trainingSceneName);
        return true;
    }

    /// <summary> このフレームに押されたコマンド入力を取得する </summary>
    bool TryGetPressedCmd(out Cmd cmd)
    {
        foreach (var pad in Gamepad.all)
        {
            if (pad.dpad.up.wasPressedThisFrame) { cmd = Cmd.Up; return true; }
            if (pad.dpad.down.wasPressedThisFrame) { cmd = Cmd.Down; return true; }
            if (pad.dpad.left.wasPressedThisFrame) { cmd = Cmd.Left; return true; }
            if (pad.dpad.right.wasPressedThisFrame) { cmd = Cmd.Right; return true; }
            if (pad.buttonEast.wasPressedThisFrame) { cmd = Cmd.B; return true; }   
            if (pad.buttonSouth.wasPressedThisFrame) { cmd = Cmd.A; return true; }  
        }

        cmd = default;
        return false;
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