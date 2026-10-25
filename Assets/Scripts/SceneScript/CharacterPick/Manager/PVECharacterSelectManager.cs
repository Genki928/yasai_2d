using Const;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class PVECharacterSelectManager : CharacterSelectManagerBase
{
    protected override void Start()
    {
        PLAYER_COUNT = 1;
        NEXT_SCENE = SceneName.BATTLE_PVE;
        base.Start();
    }

    public override void Interact(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // いずれのプレイヤーも決定中なら、対戦シーンに移行する
        if (cursor.Count(n => n.Interact) == PLAYER_COUNT)
        {
            // カーソルが対象としているアイコンから、キャラクターのIDを取得
            CharId num = cursor[0].Icon.CharacterData.Id;

            // ランダムが選択されているなら、キャラクターを抽選
            if (num == CharId.random)
                num = (CharId)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CharId)).Length);

            // スタティック変数に代入
            PVESelectedCharId.id = num;

            // SEの再生
            StartCoroutine(WaitAndLoadScene(audioAssets.SelectSE, NEXT_SCENE));
            return;
        }

        base.Interact(ctx);
    }

    protected override void Identification(InputAction.CallbackContext ctx) { }
}