using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PvPCharacterPickManager : MonoBehaviour
{
    // カーソル
    [SerializeField] GameObject cursorPrefab;
    [SerializeField] IconSelecter[] baseIcon = new IconSelecter[2];
    CursorData[] cursor = new CursorData[2] { new(), new() };
    GameObject[] cursorObj = new GameObject[2];
    [SerializeField] Sprite[] cursorSprite = new Sprite[2];
    [SerializeField] Model[] model = new Model[2];
    int currentController;

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            // 生成
            cursorObj[i] = Instantiate(cursorPrefab);
            cursorObj[i].GetComponent<SpriteRenderer>().sprite = cursorSprite[i];

            // 位置と参照先を変更
            cursorObj[i].transform.position = baseIcon[i].transform.position;
            cursor[i].icon = baseIcon[i];
            Draw(i);
        }
    }

    void Update()
    {

    }

    void Identification(InputAction.CallbackContext ctx)
    {
        // 識別
        if (Gamepad.all[0] == ctx.control.device) currentController = 0;
        else currentController = 1;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (cursor[0].intaract && cursor[1].intaract)
        {
            int[] num = new int[2] {
                    (int)cursor[0].icon.CharacterData.Id,
                    (int)cursor[1].icon.CharacterData.Id
            };

            for (int i = 0; i < 2; i++)
                if (num[i] == (int)CharId.random)
                    num[i] = UnityEngine.Random.Range(0, 6);

            // スタティック変数に代入
            PlayerPick.pick = num;
            SceneManager.LoadScene("BattleScene-pvp");
        }

        Identification(ctx);
        cursor[currentController].intaract = true;
        Draw(currentController);
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Identification(ctx);

        if (!cursor[0].intaract && !cursor[1].intaract)
        {
            SceneManager.LoadScene("TitleScene");
        }

        cursor[currentController].intaract = false;
        Draw(currentController);
    }

    public void CursorMoveRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Identification(ctx);

        if (cursor[currentController].intaract) return;
        CursorMove(DIRECT.RIGHT);
    }

    public void CursorMoveLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Identification(ctx);

        if (cursor[currentController].intaract) return;
        CursorMove(DIRECT.LEFT);
    }

    public void CursorMoveUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Identification(ctx);

        if (cursor[currentController].intaract) return;
        CursorMove(DIRECT.UP);
    }

    public void CursorMoveDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Identification(ctx);

        if (cursor[currentController].intaract) return;
        CursorMove(DIRECT.DOWN);
    }

    protected void CursorMove(DIRECT direct)
    {
        var nextIcon = cursor[currentController].icon.Icons[(int)direct];
        cursorObj[currentController].transform.position = nextIcon.transform.position;
        cursor[currentController].icon = nextIcon;
        Draw(currentController);
    }

    void Draw(int id)
    {
        model[id].name.text = cursor[id].icon.CharacterData.Name;
        model[id].model.sprite = cursor[id].icon.CharacterData.Sprite;
        model[id].model.rectTransform.sizeDelta = cursor[id].icon.CharacterData.Size;
        if (cursor[id].intaract)
            model[currentController].model.sprite = cursor[currentController].icon.CharacterData.InteractedSprite;
        else
            model[currentController].model.sprite = cursor[currentController].icon.CharacterData.Sprite;
    }

    class CursorData
    {
        public IconSelecter icon;
        public bool intaract = false;
    }

    [Serializable]
    class Model
    {
        public Image model;
        public Text name;
    }
}