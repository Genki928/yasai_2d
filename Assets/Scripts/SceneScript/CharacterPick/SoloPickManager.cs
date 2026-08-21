using UnityEngine;
using UnityEngine.InputSystem;
using Const;

public class SoloPickManager : PickManagerBase
{

    protected override void Start()
    {
        base.Start();

        cursor_obj[0] = Instantiate(cursor_pf);
        cursor_obj[0].GetComponent<SpriteRenderer>().sprite = cursor[0].img;
        Draw(++current_controller);
    }

    public override void Interact(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            // サウンド
            AudioClip clip = se[1];
            audioSource.PlayOneShot(clip);

            // すべてのプレイヤーがキャラクターを決定していたら、シーンを遷移
            if (cursor[0].interact)
            {
                CharPickData.id = cursor[0].pos[Y] * ICON_LINEFEED_COUNT + cursor[0].pos[X];
                if (CharPickData.id == icon_img.Count - 1) CharPickData.id = UnityEngine.Random.Range(0, icon_img.Count - 1);

                // シーン移行（SEが鳴り終わるまで待つ）
                StartCoroutine(WaitAndLoadScene(clip, SceneName.BATTLE_PVE));
            }

            base.Interact(ctx);
        }
    }

    public virtual void Cancel(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            // サウンド
            AudioClip clip = se[2];
            audioSource.PlayOneShot(clip);
            
            // シーン移行
            if (!cursor[0].interact)
            {
                StartCoroutine(WaitAndLoadScene(clip, SceneName.TITLE));
                return;
            }

            base.Cansel(ctx);
        }
    }

    public override void CursorUp(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            base.CursorUp(ctx);
        }
    }

    public override void CursorDown(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            base.CursorDown(ctx);
        }
    }

    public override void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            base.CursorLeft(ctx);
        }
    }

    public override void CursorRight(InputAction.CallbackContext ctx)
    {
        if (IsSwitchedScene(ctx))
        {
            base.CursorRight(ctx);
        }
    }
}

public static class CharPickData
{
    public static int id = 0;
}