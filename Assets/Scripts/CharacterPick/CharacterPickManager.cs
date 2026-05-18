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
    const float ICON_VERTICAL_CPACE = 1.2f;
    const int ICON_LINEFEED_COUNT = 3;

    void Start()
    {
        // アイコン生成
        for (int i = 0; i < icon_img.Count; i++)
        {
            icon_obj.Add(Instantiate(icon_pf));
        }

        // 移動
        Vector2 pos = new(0, 0);
        for (int i = 0; i < icon_obj.Count; i++)
        {
            // 座標決定
            pos = new(ICON_HORIZONTAL_SPACE * (i % ICON_LINEFEED_COUNT),
                      -ICON_VERTICAL_CPACE * (i / ICON_LINEFEED_COUNT));
            icon_obj[i].transform.position = pos;

            // アイコンの変更
            icon_obj[i].GetComponent<PickIcon>().SetIcon(icon_img[i]);
        }
        cursor_obj.Add(Instantiate(cursor_pf));
        cursor[0].pos = icon_obj[0].transform.position;
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
        ;
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        ;
    }

    public void CursorLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            cursor_obj[0].transform.position = new(cursor_obj[0].transform.transform.position.x - ICON_HORIZONTAL_SPACE, cursor_obj[0].transform.transform.position.y);
            if (cursor_obj[0].transform.position.x < 0)
            {
                cursor_obj[0].transform.position = new(ICON_HORIZONTAL_SPACE * (ICON_LINEFEED_COUNT - 1), cursor_obj[0].transform.transform.position.y);
            }
        }
    }

    public void CursorRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            cursor_obj[0].transform.position = new(cursor_obj[0].transform.transform.position.x + ICON_HORIZONTAL_SPACE, cursor_obj[0].transform.transform.position.y);
            if (cursor_obj[0].transform.position.x >= ICON_LINEFEED_COUNT)
            {
                cursor_obj[0].transform.position = new(0, cursor_obj[0].transform.transform.position.y);
            }
        }
    }
}

[Serializable]
public class Cursor
{
    public Sprite img;
    public Vector2 pos = new(0, 0);
}