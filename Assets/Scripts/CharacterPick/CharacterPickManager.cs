using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterPickManager : MonoBehaviour
{
    [Header("◇アイコン")]
    [SerializeField] GameObject icon_pf;
    [SerializeField] List<Sprite> icon_img = new();
    List<GameObject> icon_obj = new();
    Vector2 pos = new(0, 0);

    [Header("◇カーソル")]
    [SerializeField] GameObject cursor_pf;
    [SerializeField] Cursor[] cursor = new Cursor[2];
    [SerializeField] Sprite mix_cursor;
    List<GameObject> cursor_obj = new();

    [Header("◇モデル")]
    [SerializeField] GameObject[] model = new GameObject[2];

    const float ICON_HORIZONTAL_SPACE = 1.5f;
    const float ICON_VERTICAL_SPACE = 1.4f;
    const int ICON_LINEFEED_COUNT = 3;
    const int X = 0;
    const int Y = 1;

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
        cursor_obj.Add(Instantiate(cursor_pf));
        cursor_obj[1].GetComponent<SpriteRenderer>().sprite = cursor[1].img;
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
            //if (--cursor[0].pos[Y] < 0) cursor[0].pos[Y] = icon_obj.Count / ICON_LINEFEED_COUNT;
            //if (cursor[0].pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            //{
            //    if (cursor[0].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
            //    {
            //        cursor[0].pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;
            //    }
            //}

            //cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
        }
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //if (++cursor[0].pos[Y] > (icon_obj.Count - 1) / ICON_LINEFEED_COUNT) cursor[0].pos[Y] = 0;
            //if (icon_obj.Count % ICON_LINEFEED_COUNT != 0)
            //{
            //    if (cursor[0].pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT) cursor[0].pos[Y] = 0;
            //}


            //cursor_obj[0].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[0].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[0].pos[Y]);
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
        Debug.Log(cursor[n].pos[Y] / ICON_LINEFEED_COUNT + cursor[n].pos[X]);
        cursor_obj[n].transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor[n].pos[X], pos.y - ICON_VERTICAL_SPACE * cursor[n].pos[Y]);
        model[n].GetComponent<SpriteRenderer>().sprite = icon_img[cursor[n].pos[Y] * ICON_LINEFEED_COUNT + cursor[n].pos[X]];
    }
}

[Serializable]
public class Cursor
{
    public Sprite img;
    public int[] pos = new int[2] { 0, 0 };
}

[CreateAssetMenu(menuName = "Character/PickData")]
public class PickData : ScriptableObject
{
    
}