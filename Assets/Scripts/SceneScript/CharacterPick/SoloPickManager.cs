using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Const;

public class SoloPickManager : MonoBehaviour
{
    [Header("◇アイコン")]
    [SerializeField] GameObject icon_pf;
    [SerializeField] List<Sprite> icon_img = new();
    List<GameObject> icon_obj = new();
    Vector2 pos = new(0, 0);

    [Header("◇カーソル")]
    [SerializeField] GameObject cursor_pf;
    [SerializeField] Cursor cursor;
    GameObject cursor_obj;

    [SerializeField] List<PickData> pick_data = new();
    [SerializeField] StateIndicater state;

    [Header("◇Ready")]
    [SerializeField] GameObject[] ready = new GameObject[2];

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
        cursor_obj = Instantiate(cursor_pf);
        cursor_obj.GetComponent<SpriteRenderer>().sprite = cursor.img;
        Draw();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // すべてのプレイヤーがキャラクターを決定していたら、シーンを遷移
            if (cursor.interact)
            {
                CharPickData.id = cursor.pos[Y] * ICON_LINEFEED_COUNT + cursor.pos[X];
                SceneManager.LoadScene(SceneName.BATTLE_PVE);
            }

            // 決定
            cursor.interact = true;
            state.button.SetActive(true);
            if (cursor.interact && cursor.interact)
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
            if (!cursor.interact)
            {
                SceneManager.LoadScene(SceneName.TITLE);
                return;
            }
            if (cursor.interact && cursor.interact)
            {
                ready[0].SetActive(false);
                ready[1].SetActive(false);
            }

            // 決定
            cursor.interact = false;
            state.button.SetActive(false);
        }
    }

    public void CursorUP(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 決定済みなら移動不可
            if (cursor.interact) return;

            if (--cursor.pos[Y] < 0) cursor.pos[Y] = icon_obj.Count / ICON_LINEFEED_COUNT;
            if (cursor.pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
            {
                if (cursor.pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                {
                    cursor.pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;
                }
            }

            Draw();
        }
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 決定済みなら移動不可
            if (cursor.interact) return;

            if (++cursor.pos[Y] > (icon_obj.Count - 1) / ICON_LINEFEED_COUNT) cursor.pos[Y] = 0;
            if (icon_obj.Count % ICON_LINEFEED_COUNT != 0)
            {
                if (cursor.pos[X] > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT) cursor.pos[Y] = 0;
            }


            Draw();
        }
    }

    public void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 決定済みなら移動不可
            if (cursor.interact) return;

            // 移動
            if (--cursor.pos[X] < 0) cursor.pos[X] = ICON_LINEFEED_COUNT - 1;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を右端に整える
            if (cursor.pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor.pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor.pos[X] = (icon_obj.Count - 1) % ICON_LINEFEED_COUNT;

            // 描画
            Draw();
        }
    }

    public void CursorRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 決定済みなら移動不可
            if (cursor.interact) return;

            // 移動
            if (++cursor.pos[X] > ICON_LINEFEED_COUNT - 1) cursor.pos[X] = 0;

            // もしカーソルのX座標がアイコンのある位置から外れていたら、座標を左端に整える
            if (cursor.pos[Y] == icon_obj.Count / ICON_LINEFEED_COUNT)
                if (cursor.pos[X] % ICON_LINEFEED_COUNT > (icon_obj.Count - 1) % ICON_LINEFEED_COUNT)
                    cursor.pos[X] = 0;

            // 描画
            Draw();
        }
    }

    void Draw()
    {
        // 描画
        cursor_obj.transform.position = new(pos.x + ICON_HORIZONTAL_SPACE * cursor.pos[X], pos.y - ICON_VERTICAL_SPACE * cursor.pos[Y]);
        state.model.GetComponent<SpriteRenderer>().sprite = icon_img[cursor.pos[Y] * ICON_LINEFEED_COUNT + cursor.pos[X]];
        state.name.text = pick_data[cursor.pos[Y] * ICON_LINEFEED_COUNT + cursor.pos[X]].char_name;
        state.lore.text = pick_data[cursor.pos[Y] * ICON_LINEFEED_COUNT + cursor.pos[X]].lore;
    }
}

public static class CharPickData
{
    public static int id = 0;
}