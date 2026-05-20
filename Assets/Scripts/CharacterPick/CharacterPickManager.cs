using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterPickManager : MonoBehaviour
{
    [SerializeField] GameObject icon_pf;
    [SerializeField] List<Sprite> icon_img = new();
    [Header("◇カーソル")]
    [SerializeField] GameObject cursor_pf;
    [SerializeField] Cursor[] cursor = new Cursor[2];
    [SerializeField] Sprite mix_cursor;
    List<GameObject> icon_obj = new();
    List<GameObject> cursor_obj = new();

    const float ICON_HORIZONTAL_SPACE = 1.5f;
    const float ICON_VERTICAL_SPACE = 1.4f;
    const int ICON_LINEFEED_COUNT = 3;
    const int X = 0;
    const int Y = 1;
    public Vector2 pos = new(0, 0);

    void Start()
    {
        // アイコン生成
        for (int i = 0; i < icon_img.Count; i++)
        {
            icon_obj.Add(Instantiate(icon_pf));
        }

        // 移動
        for (int i = 0; i < icon_obj.Count; i++)
        {
            pos = new(-ICON_HORIZONTAL_SPACE, 0);
            // 座標決定
            icon_obj[i].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * (i % ICON_LINEFEED_COUNT),
                                                 pos.y + -ICON_VERTICAL_SPACE * (i / ICON_LINEFEED_COUNT));

            // アイコンの変更
            icon_obj[i].GetComponent<PickIcon>().SetIcon(icon_img[i]);
        }
        cursor_obj.Add(Instantiate(cursor_pf));
        cursor_obj[0].GetComponent<SpriteRenderer>().sprite = cursor[0].img;
        //cursor[0].pos = icon_obj[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            SceneManager.LoadScene("BattleScene");
    }

    public void CursorUP(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (--cursor[0].pos[Y] < 0) cursor[0].pos[Y] = icon_obj.Count / ICON_LINEFEED_COUNT;
            if (cursor[0].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            {
                if (cursor[0].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                {
                    cursor[0].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;
                }
            }

            cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
        }
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (++cursor[0].pos[Y] > (icon_obj.Count - 1) / ICON_LINEFEED_COUNT) cursor[0].pos[Y] = 0;
            if (icon_obj.Count % ICON_LINEFEED_COUNT != 0)
            {
                if (cursor[0].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT) cursor[0].pos[Y] = 0;
            }


            cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
        }
    }

    public void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (--cursor[0].pos[X] < 0) cursor[0].pos[X] = ICON_LINEFEED_COUNT - 1;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を右端に整える
            if (cursor[0].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor[0].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor[0].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;

            cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
        }
    }

    public void CursorRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (++cursor[0].pos[X] > ICON_LINEFEED_COUNT - 1) cursor[0].pos[X] = 0;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を左端に整える
            if (cursor[0].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor[0].pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor[0].pos[X] = 0;

            cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
        }
    }
}

[Serializable]
public class Cursor
{
    public Sprite img;
    public int[] pos = new int[2] { 0, 0 };
}