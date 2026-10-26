using Const;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectManagerBase : MonoBehaviour
{
    [Header("▼ Cursor")]
    [SerializeField] protected GameObject cursorPrefab;
    [SerializeField] protected List<Sprite> cursorSprite = new();
    [SerializeField] protected List<IconSelecter> baseIcon = new();
    protected List<CursorData> cursor = new();
    protected List<GameObject> cursorObj = new();
    protected int currentController = 0;

    [Header("▼ UI")]
    [SerializeField] protected List<Model> model = new();

    [Header("▼ Audio")]
    [SerializeField] protected CharacterSelectAA audioAssets;
    protected AudioSource audioSource;
    protected bool isChangingScene = false; // 二重遷移防止

    // --- 定数 --- //
    protected int PLAYER_COUNT = 1;
    protected string NEXT_SCENE = "";

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // カーソルの初期設定
        for (int i = 0; i < PLAYER_COUNT; i++)
        {
            // 生成
            cursorObj.Add(Instantiate(cursorPrefab));
            cursorObj[i].GetComponent<SpriteRenderer>().sprite = cursorSprite[i];

            // 位置と参照先を変更
            cursorObj[i].transform.position = baseIcon[i].transform.position;
            cursor.Add(new(baseIcon[i]));
            Draw(i);
        }
    }

    protected virtual void Identification(InputAction.CallbackContext ctx)
    {
        // 識別
        if (Gamepad.all[0] == ctx.control.device) currentController = 0;
        else currentController = 1;
    }

    public virtual void Interact(InputAction.CallbackContext ctx)
    {
        // 入力を真に切り替える
        Identification(ctx);
        cursor[currentController].SetInteract(true);
        
        // カーソルの描画
        Draw(currentController);

        // SEの再生
        audioSource.PlayOneShot(audioAssets.SelectSE);
    }

    public virtual void Cancel(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // いずれのプレイヤーも決定中じゃなければ、タイトルへ戻る
        if (cursor.Count(n => n.Interact) == 0)
        {
            StartCoroutine(WaitAndLoadScene(audioAssets.CancelSE, SceneName.TITLE));
            return;
        }

        // 入力を偽に切り替える
        Identification(ctx);
        cursor[currentController].SetInteract(false);

        // カーソルの描画
        Draw(currentController);

        // SEの再生
        audioSource.PlayOneShot(audioAssets.CancelSE);
    }

    public void CursorMoveRight(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // 決定中なら中断
        Identification(ctx);
        if (cursor[currentController].Interact) return;

        // カーソルを移動
        CursorMove(DIRECT.RIGHT);
    }

    public void CursorMoveLeft(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // 決定中なら中断
        Identification(ctx);
        if (cursor[currentController].Interact) return;

        // カーソルを移動
        CursorMove(DIRECT.LEFT);
    }

    public void CursorMoveUp(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // 決定中なら中断
        Identification(ctx);
        if (cursor[currentController].Interact) return;

        // カーソルを移動
        CursorMove(DIRECT.UP);
    }

    public void CursorMoveDown(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // 決定中なら中断
        Identification(ctx);
        if (cursor[currentController].Interact) return;

        // カーソルを移動
        CursorMove(DIRECT.DOWN);
    }

    protected void CursorMove(DIRECT direct)
    {
        // カーソルを移動
        var nextIcon = cursor[currentController].Icon.Icons[(int)direct];
        cursorObj[currentController].transform.position = nextIcon.transform.position;

        // カーソルの描画
        cursor[currentController].SetIcon(nextIcon);
        Draw(currentController);

        // SEの再生
        audioSource.PlayOneShot(audioAssets.CursorMoveSE);
    }

    protected virtual void Draw(int id)
    {
        // 名前を変更
        model[id].name.text = cursor[id].Icon.CharacterData.Name;
        model[id].lore.text = cursor[id].Icon.CharacterData.Lore.lore;

        // 画像を変更
        model[id].model.sprite = cursor[id].Icon.CharacterData.Sprite;
        model[id].model.rectTransform.sizeDelta = cursor[id].Icon.CharacterData.Size;

        // 決定中かどうかで、変更する画像を切り替える
        var cur = cursor[currentController].Icon.CharacterData;
        model[currentController].model.sprite = (cursor[id].Interact) ? cur.InteractedSprite : cur.Sprite;
    }

    protected IEnumerator WaitAndLoadScene(AudioClip clip, string sceneName)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        SceneManager.LoadScene(sceneName);
    }

    protected class CursorData
    {
        // --- プロパティ --- //
        public IconSelecter Icon => icon;
        public bool Interact => interact;

        // --- フィールド --- //
        IconSelecter icon;
        bool interact = false;

        // --- コンストラクタ --- //
        public CursorData(IconSelecter baseIcon)
        {
            icon = baseIcon;
        }

        // --- メソッド --- //
        public void SetIcon(IconSelecter icon)
        {
            this.icon = icon;
        }

        public void SetInteract(bool interact)
        {
            this.interact = interact;
        }
    }

    [Serializable]
    protected class Model
    {
        public Image model;
        public Text name;
        public Text lore;
    }
}

public static class PVESelectedCharId
{
    public static CharId id = CharId.Carrot;
}