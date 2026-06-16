using UnityEngine;
using Const;
using System.Collections;
using System.Collections.Generic;

public class BossCarrot : BossBase
{
    int pattern = 0;
    override protected void Start()
    {
        // 基底クラス
        base.Start();

        // フェーズ設定
        states.Add("Tackle", new BossCarrotTackle());
        states.Add("Headbang", new BossCarrotHeadbang());
        states.Add("Chill", new BossChill());
        ChangeState("Headbang");
    }

    override protected void Update()
    {
        if (pattern == 0)
        {
            if (state.Update()) ChangeState("Chill");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 接触したオブジェクトがCharBaseを持っていて、
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            // そのオブジェクトのCharBaseが持つidが、damagedリストに登録されていなければ、
            if (!damaged.Contains(cb.id))
            {
                // ダメージを与え、damagedリストに登録する（連続ヒット防止）
                cb.Damage(damage, id);
                damaged.Add(cb.id);
            }
        }
    }
}

/// <summary> ボスニンジンの、ヘドバン攻撃パターン </summary>
public class BossCarrotHeadbang : BossState
{
    const int CIRCLE_DAMAGE_AREA = 0;
    BossCarrot me;
    int phase = 0;
    int rotate = 0;
    int timer = 0;
    GameObject circle;

    override public void Enter(BossBase bb)
    {
        me = (BossCarrot)bb;    // 型を渡す
        bb.Freeze(false);       // 判定を消す
        bb.damage = 20;         // ダメージを設定
        circle = me.DisplayDamageArea(CIRCLE_DAMAGE_AREA, me.transform.position);
    }
    override public bool Update()
    {
        switch (phase)
        {
            // エリア表示
            case 0:
                if (++timer >= 30) phase = 1;
                break;

            // 頭下げ
            case 1:
                me.transform.rotation = Quaternion.Euler(0, 0, rotate += 10);
                if (rotate >= 90) phase = 2;
                break;

            // 頭上げ
            case 2:
                me.transform.rotation = Quaternion.Euler(0, 0, rotate -= 10);
                if (rotate <= -180)
                {
                    me.RemoveDamageArea(circle);
                    me.ChangeState("Tackle");
                }
                break;
        }
        return false;
    }
    override public void Exit()
    {
        me.Freeze(true);
        me.damage = 0;
        me.damaged.Clear();
    }
}

/// <summary> ボスニンジンの、タックル攻撃パターン </summary>
public class BossCarrotTackle : BossState
{
    const int LINE_DAMAGE_AREA = 1;
    BossCarrot me;
    int phase = 0;
    int timer = 0;
    GameObject line;

    override public void Enter(BossBase bb)
    {
        me = (BossCarrot)bb;    // 型を渡す
        bb.Freeze(false);       // 判定を消す
        bb.damage = 20;         // ダメージを設定
        line = me.DisplayDamageArea(LINE_DAMAGE_AREA, me.transform.position);
        me.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
    }
    override public bool Update()
    {
        switch (phase)
        {
            // ロックオン
            case 0:
                line.transform.position = new(0.0f, me.transform.position.y);
                me.transform.position = new(10.0f, me.player.transform.position.y);
                if (++timer >= 60)
                {
                    phase = 1;
                }
                break;

            // 突進
            case 1:
                me.rb.linearVelocity = new(-BossCarrotConst.TACKLE_SPEED, 0.0f);
                if (me.transform.position.x <= -20.0f) phase = 2;
                break;

            // 終了
            case 2:
                me.RemoveDamageArea(line);
                me.transform.position = new(6.0f, -5);
                me.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
        }
        return false;
    }
    override public void Exit()
    {
        me.Freeze(true);
        me.damage = 0;
        me.damaged.Clear();
    }
}

