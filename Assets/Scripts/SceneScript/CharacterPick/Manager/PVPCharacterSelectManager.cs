using Const;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class PVPCharacterSelectManager : CharacterSelectManagerBase
{
    protected override void Start()
    {
        PLAYER_COUNT = 2;
        NEXT_SCENE = SceneName.BATTLE_PVP;
        base.Start();
    }

    public override void Interact(InputAction.CallbackContext ctx)
    {
        // 入力中じゃなければ中断
        if (!ctx.performed) return;

        // いずれのプレイヤーも決定中なら、対戦シーンに移行する
        if (cursor.Count(n => n.Interact) == PLAYER_COUNT)
        {
            CharId[] num = new CharId[PLAYER_COUNT];
            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                // カーソルが対象としているアイコンから、キャラクターのIDを取得
                num[i] = cursor[i].Icon.CharacterData.Id;

                // ランダムが選択されているなら、キャラクターを抽選
                if (num[i] == CharId.random)
                    num[i] = (CharId)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CharId)).Length);
            }

            // スタティック変数に代入
            PlayerPick.pick = num;

            // SEの再生
            StartCoroutine(WaitAndLoadScene(audioAssets.SelectSE, NEXT_SCENE));
            return;
        }

        base.Interact(ctx);
    }
}