using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        if (ctx.performed)
        {
            // すべてのプレイヤーがキャラクターを決定していたら、シーンを遷移
            if (cursor[0].interact)
            {
                CharPickData.id = cursor[0].pos[Y] * ICON_LINEFEED_COUNT + cursor[0].pos[X];
                if (CharPickData.id == icon_img.Count - 1) CharPickData.id = UnityEngine.Random.Range(0, icon_img.Count - 1);
                SceneManager.LoadScene(SceneName.BATTLE_PVE);
            }

            base.Interact(ctx);
        }
    }

    public virtual void Cancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!cursor[0].interact)
            {
                SceneManager.LoadScene(SceneName.TITLE);
                return;
            }

            base.Cansel(ctx);
        }
    }

    public override void CursorUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.CursorUp(ctx);
        }
    }

    public override void CursorDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.CursorDown(ctx);
        }
    }

    public override void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.CursorLeft(ctx);
        }
    }

    public override void CursorRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.CursorRight(ctx);
        }
    }
}

public static class CharPickData
{
    public static int id = 0;
}