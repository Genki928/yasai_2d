using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Const;
using System.Collections;
using System.Collections.Generic;

public class CharacterPickManager : PickManagerBase
{
    //se用
    [SerializeField] List<AudioClip> se;
    public AudioSource audioSource;

    bool isChangingScene = false; // 二重遷移防止

    // ----- 定数 ----- //
    protected const int PLAYER_CNT = 2;

    protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        base.Start();

        for (int i = 0; i < PLAYER_CNT; i++)
        {
            cursor_obj[i] = Instantiate(cursor_pf);
            cursor_obj[i].GetComponent<SpriteRenderer>().sprite = cursor[i].img;
            current_controller = i;
            Draw(current_controller);
        }
    }

    public override void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isChangingScene) return;

            AudioClip clip = se[1];
            audioSource.PlayOneShot(clip);

            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.Interact(ctx);

            // すべてのプレイヤーがキャラクターを決定していたら、シーンを遷移
            if (cursor[0].interact && cursor[1].interact)
            {
                // スタティック変数に代入する準備
                int[] num = new int[PLAYER_CNT]
                {
                    cursor[0].pos[Y] * ICON_LINEFEED_COUNT + cursor[0].pos[X],
                    cursor[1].pos[Y] * ICON_LINEFEED_COUNT + cursor[1].pos[X]
                };

                // もし選択キャラクターが、キャラクターリストの最大値と同じ（ランダム）なら、キャラクターをランダムに選択
                for (int i = 0; i < PLAYER_CNT; i++)
                    if (num[i] == icon_img.Count - 1) num[i] = UnityEngine.Random.Range(0, icon_img.Count - 1);

                // スタティック変数に代入
                PlayerPick.pick = new int[PLAYER_CNT] {
                    num[0],
                    num[1]
                };

                // シーン移行（SEが鳴り終わるまで待つ）
                StartCoroutine(WaitAndLoadScene(clip, SceneName.BATTLE_PVP));
            }

            current_controller = -1;
        }
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isChangingScene) return;

            AudioClip clip = se[2];
            audioSource.PlayOneShot(clip);

            if (!cursor[0].interact && !cursor[1].interact)
            {
                StartCoroutine(WaitAndLoadScene(clip, "TitleScene"));
                return;
            }

            //
            if (cursor[0].interact && cursor[1].interact)
            {
                ready[0].SetActive(false);
                ready[1].SetActive(false);
            }

            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.Cansel(ctx);
        }
    }

    IEnumerator WaitAndLoadScene(AudioClip clip, string sceneName)
    {
        isChangingScene = true;

        if (clip != null)
        {
            yield return new WaitForSeconds(clip.length);
        }

        SceneManager.LoadScene(sceneName);
    }

    public override void CursorUp(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (ctx.performed)
        {
            audioSource.PlayOneShot(se[0]);
            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.CursorUp(ctx);
        }
    }

    public override void CursorDown(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (ctx.performed)
        {
            audioSource.PlayOneShot(se[0]);

            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.CursorDown(ctx);
        }
    }

    public override void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (ctx.performed)
        {
            audioSource.PlayOneShot(se[0]);
            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.CursorLeft(ctx);
        }
    }

    public override void CursorRight(InputAction.CallbackContext ctx)
    {
        if (isChangingScene) return;
        if (ctx.performed)
        {
            audioSource.PlayOneShot(se[0]);
            // 識別
            if (Gamepad.all[0] == ctx.control.device) current_controller = 0;
            else current_controller = 1;

            base.CursorRight(ctx);
        }
    }
}