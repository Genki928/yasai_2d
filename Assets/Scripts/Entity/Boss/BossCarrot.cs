using UnityEngine;
using Const;

public class BossCarrot : BossBase
{
    int pattern = 0;
    override protected void Start()
    {
        // 基底クラス
        base.Start();

        // フェーズ設定
        right = true;
        states.Add("Tackle", new BossCarrotTackle());
        states.Add("Headbang", new BossCarrotHeadbang());
        states.Add("Chill", new BossChill());
        ChangeState("Tackle");
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
                if (me.right) me.transform.rotation = Quaternion.Euler(0, 0, rotate -= 10);
                else me.transform.rotation = Quaternion.Euler(0,0, rotate += 10);

                if (me.right)
                {
                    if (rotate <= -90) phase = 2;
                }
                else
                {
                    if (rotate >= 90) phase = 2;
                }
                break;

            // 頭上げ
            case 2:
                if (me.right) me.transform.rotation = Quaternion.Euler(0, 0, rotate += 10);
                else me.transform.rotation = Quaternion.Euler(0, 0, rotate -= 10);

                if (!me.right)
                {
                    if (rotate <= -180)
                    {
                        me.RemoveDamageArea(circle);
                        me.ChangeState("Tackle");
                    }
                }
                else
                {
                    if (rotate >= 180)
                    {
                        me.RemoveDamageArea(circle);
                        me.ChangeState("Tackle");
                    }
                }
                break;
        }
        return false;
    }
    override public void Exit()
    {
        me.Freeze(true);
        me.damage = 0;
        phase = 0;
        rotate = 0;
        timer = 0;
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
        
        if (me.right) me.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        else me.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
    }
    override public bool Update()
    {
        switch (phase)
        {
            // -----ロックオン----- //
            case 0:
                // 線の追従
                line.transform.position = new(0.0f, me.transform.position.y);
                
                // 本体の追従
                if (me.right) me.transform.position = new(-10.0f, me.player.transform.position.y);
                else me.transform.position = new(10.0f, me.player.transform.position.y);
                
                // 一定時間経過で突進開始
                if (++timer >= 60)
                {
                    phase = 1;
                    timer = 0;
                }
                break;

            // ----- 突進 ----- //
            case 1:
                // ベクトル設定
                if (me.right) me.rb.linearVelocity = new(BossCarrotConst.TACKLE_SPEED, 0.0f);
                else me.rb.linearVelocity = new(-BossCarrotConst.TACKLE_SPEED, 0.0f);

                if (me.right)
                {
                    if (me.transform.position.x >= 20.0f) phase = 2;
                }
                else
                {
                    if (me.transform.position.x <= -20.0f) phase = 2;
                }
                break;

            // 終了
            case 2:
                if (++timer >= 30) me.ChangeState("Headbang");
                break;
        }
        return false;
    }
    override public void Exit()
    {
        me.RemoveDamageArea(line);
        if (me.right) me.transform.position = new(6.0f, -5);
        else me.transform.position = new(-6.0f, -5);
        me.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        me.Freeze(true);
        me.damage = 0;
        me.damaged.Clear();
        me.right = !me.right;
        phase = 0;
        timer = 0;
    }
}

