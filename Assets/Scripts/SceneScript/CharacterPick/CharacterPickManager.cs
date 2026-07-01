using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Const;

public class CharacterPickManager : MonoBehaviour
{
    [Header("◇アイコン")]
    [SerializeField] GameObject icon_pf;
    [SerializeField] List<Sprite> icon_img = new();
    List<GameObject> icon_obj = new();
    Vector2 pos = new(0, 5);

    [Header("◇カーソル")]
    [SerializeField] GameObject cursor_pf;
    [SerializeField] Sprite mix_cursor;
    //[SerializeField] GameObject[] button;
    [SerializeField] Cursor[] cursor = new Cursor[2];
    GameObject[] cursor_obj = new GameObject[2];

    [Header("◇モデル")]
    //[SerializeField] GameObject[] model = new GameObject[2];
    [SerializeField] List<PickData> pick_data = new();
    [SerializeField] StateIndicater[] state = new StateIndicater[2];

    [Header("◇Ready")]
    [SerializeField] GameObject[] ready = new GameObject[2];

    const float ICON_HORIZONTAL_SPACE = 1.5f;
    const float ICON_VERTICAL_SPACE = 2.0f;
    const int ICON_LINEFEED_COUNT = 3;
    const int X = 0;
    const int Y = 1;

    void  Start()
    {
        // アイコン生成
        for (int i = 0; i < icon_img.Count; i++)
        {
            icon_obj.Add(Instantiate(icon_pf));
        }

        // 移動
        for (int i = 0; i < icon_obj.Count; i++)
        {
            pos = new(-ICON_HORIZONTAL_SPACE, 3);
            // 座標決定
            icon_obj[i].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * (i % ICON_LINEFEED_COUNT),
                                                 pos.y + -ICON_VERTICAL_SPACE * (i / ICON_LINEFEED_COUNT));

            // アイコンの変更
            icon_obj[i].GetComponent<PickIcon>().SetIcon(icon_img[i]);
        }
        for (int i = 0; i < 2; i++)
        {
            cursor_obj[i] = Instantiate(cursor_pf);
            cursor_obj[i].GetComponent<SpriteRenderer>().sprite = cursor[i].img;
            Draw(i);
        }
        //cursor[0].pos = icon_obj[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // すべてのプレイヤーがキャラクターを決定していたら、シーンを遷移
            if (cursor[0].interact && cursor[1].interact)
            {
                int[] num = new int[2]
                {
                    cursor[0].pos[Y] * ICON_LINEFEED_COUNT + cursor[0].pos[X],
                    cursor[1].pos[Y] * ICON_LINEFEED_COUNT + cursor[1].pos[X]
                };
                for(int i = 0; i < 2; i ++)
                {
                    if (num[i] == icon_img.Count - 1) num[i] = UnityEngine.Random.Range(0, icon_img.Count - 1);
                }
                PlayerPick.pick = new int[2] {
                    num[0],
                    num[1]
                };
                Debug.Log(num[0] + " , " + num[1]);
                SceneManager.LoadScene(SceneName.BATTLE_PVP);
            }

            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            // 決定
            cursor[n].interact = true;
            state[n].button.SetActive(true);
            if (cursor[0].interact && cursor[1].interact)
            {
                ready[0].SetActive(true);
                ready[1].SetActive(true);
            }
        }
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!cursor[0].interact && !cursor[1].interact)
            {
                SceneManager.LoadScene("TitleScene"); 
                return;
            }

            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            //
            if (cursor[0].interact && cursor[1].interact)
            {
                ready[0].SetActive(false);
                ready[1].SetActive(false);
            }

            // 決定
            cursor[n].interact = false;
            state[n].button.SetActive(false);
        }
    }

    public void CursorUP(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            // 決定済みなら移動不可
            if (cursor[n].interact) return;

            if (--cursor[n].pos[Y] < 0) cursor[n].pos[Y] = icon_obj.Count / (ICON_LINEFEED_COUNT);
            if (icon_obj.Count % 3 == 0)
                --cursor[n].pos[Y];
            if (cursor[n].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            {
                if (cursor[n].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                {
                    cursor[n].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;
                }
            }

            // 描画
            Draw(n);
        }
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            // 決定済みなら移動不可
            if (cursor[n].interact) return;

            if (++cursor[n].pos[Y] > (icon_obj.Count - 1) / ICON_LINEFEED_COUNT) cursor[n].pos[Y] = 0;
            if (icon_obj.Count % ICON_LINEFEED_COUNT != 0)
            {
                if (cursor[n].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT) cursor[n].pos[Y] = 0;
            }

            // 描画
            Draw(n);
        }
    }

    public void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            // 決定済みなら移動不可
            if (cursor[n].interact) return;

            // 移動
            if (--cursor[n].pos[X] < 0) cursor[n].pos[X] = ICON_LINEFEED_COUNT - 1;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を右端に整える
            if (cursor[n].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor[n].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor[n].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;

            // 描画
            Draw(n);
        }
    }

    public void CursorRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 識別
            int n = -1;
            if (Gamepad.all[0] == ctx.control.device) n = 0;
            else n = 1;

            // 決定済みなら移動不可
            if (cursor[n].interact) return;

            // 移動
            if (++cursor[n].pos[X] > ICON_LINEFEED_COUNT - 1) cursor[n].pos[X] = 0;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を左端に整える
            if (cursor[n].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor[n].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor[n].pos[X] = 0;

            // 描画
            Draw(n);
        }
    }

    void Draw(int n)
    {
        // 描画
        cursor_obj[n].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[n].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[n].pos[Y]);
        state[n].model.GetComponent<SpriteRenderer>().sprite = icon_img[cursor[n].pos[Y] * ICON_LINEFEED_COUNT + cursor[n].pos[X]];
        state[n].name.text = pick_data[cursor[n].pos[Y] * ICON_LINEFEED_COUNT + cursor[n].pos[X]].char_name;
        state[n].lore.text = pick_data[cursor[n].pos[Y] * ICON_LINEFEED_COUNT + cursor[n].pos[X]].lore;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new(0,3), 0.2f);
    }
}

[Serializable]
public class Cursor
{
    public Sprite img;
    public int[] pos = new int[2] { 0, 0 };
    public bool interact = false;
}

[Serializable]
public class StateIndicater
{
    public Text name;
    public Text lore;
    public GameObject model;
    public GameObject button;
}

public static class PlayerPick
{
    public static int[] pick = new int[2] { 0, 5 };
}
