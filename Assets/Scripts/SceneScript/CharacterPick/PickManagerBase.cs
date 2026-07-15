using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PickManagerBase : MonoBehaviour
{
    // ----- 変数 ----- //
    [Header("◇アイコン")]
    [SerializeField] protected GameObject icon_pf;
    [SerializeField] protected List<Sprite> icon_img = new();
    protected List<GameObject> icon_obj = new();
    protected Vector2 pos = new(0, 5);

    [Header("◇Ready")]
    [SerializeField] protected GameObject[] ready = new GameObject[2];

    [Header("◇カーソル")]
    [SerializeField] protected GameObject cursor_pf;
    [SerializeField] protected Cursor[] cursor = new Cursor[2];
    protected GameObject[] cursor_obj = new GameObject[2];
    protected int current_controller = -1;

    [Header("◇モデル")]
    [SerializeField] protected List<PickData> pick_data = new();
    [SerializeField] protected StateIndicater[] state = new StateIndicater[2];

    // ----- 定数 ----- //
    protected const float ICON_HORIZONTAL_SPACE = 1.5f;
    protected const float ICON_VERTICAL_SPACE = 2.0f;
    protected const int ICON_LINEFEED_COUNT = 3;
    protected const int X = 0;
    protected const int Y = 1;

    protected virtual void Start()
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
    }

    virtual public void Interact(InputAction.CallbackContext ctx)
    {
        // 決定
        cursor[current_controller].interact = true;
        state[current_controller].button.SetActive(true);
        if (cursor[current_controller].interact && cursor[0].interact)
        {
            ready[0].SetActive(true);
            ready[1].SetActive(true);
        }
    }

    virtual public void Cansel(InputAction.CallbackContext ctx)
    {
        // Ready表記を消す
        for (int i = 0; i < cursor.Length; i++)
        {
            if (cursor[i].interact)
            {
                ready[0].SetActive(false);
                ready[1].SetActive(false);
            }
        }

        // 決定
        cursor[current_controller].interact = false;
        state[current_controller].button.SetActive(false);
    }

    virtual public void CursorUp(InputAction.CallbackContext ctx)
    {
        // 決定済みなら移動不可
        if (cursor[current_controller].interact) return;

        if (--cursor[current_controller].pos[Y] < 0) cursor[current_controller].pos[Y] = icon_obj.Count / (ICON_LINEFEED_COUNT);
        if (icon_obj.Count % 3 == 0)
            --cursor[current_controller].pos[Y];
        if (cursor[current_controller].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
        {
            if (cursor[current_controller].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
            {
                cursor[current_controller].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;
            }
        }

        // 描画
        Draw(current_controller);
    }

    virtual public void CursorDown(InputAction.CallbackContext ctx)
    {

        // 決定済みなら移動不可
        if (cursor[current_controller].interact) return;

        if (++cursor[current_controller].pos[Y] > (icon_obj.Count - 1) / ICON_LINEFEED_COUNT)
        {
            cursor[current_controller].pos[Y] = 0;
        }
        if (cursor[current_controller].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT
            && cursor[current_controller].pos[Y] == (icon_obj.Count - 1) / ICON_LINEFEED_COUNT)
        {
            cursor[current_controller].pos[Y] = 0;
        }

        // 描画
        Draw(current_controller);
    }

    virtual public void CursorRight(InputAction.CallbackContext ctx)
    {

        // 決定済みなら移動不可
        if (cursor[current_controller].interact) return;

        // 移動
        if (++cursor[current_controller].pos[X] > ICON_LINEFEED_COUNT - 1) cursor[current_controller].pos[X] = 0;

        // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を左端に整える
        if (cursor[current_controller].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            if (cursor[current_controller].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                cursor[current_controller].pos[X] = 0;

        // 描画
        Draw(current_controller);
    }

    virtual public void CursorLeft(InputAction.CallbackContext ctx)
    {
        // 決定済みなら移動不可
        if (cursor[current_controller].interact) return;

        // 移動
        if (--cursor[current_controller].pos[X] < 0) cursor[current_controller].pos[X] = ICON_LINEFEED_COUNT - 1;

        // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を右端に整える
        if (cursor[current_controller].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            if (cursor[current_controller].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                cursor[current_controller].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;

        // 描画
        Draw(current_controller);
    }

    protected void Draw(int n)
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
        Gizmos.DrawSphere(new(0, 3), 0.2f);
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
