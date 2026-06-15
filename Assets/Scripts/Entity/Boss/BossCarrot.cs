using UnityEngine;
using Const;
using System.Collections;

public class BossCarrot : BossBase
{

    override protected void Start()
    {
        // 基底クラス
        base.Start();

        // フェーズ設定
        states.Add("Tackle", new BossCarrotTackle());
        ChangeState("Tackle");
    }

    override protected void Update()
    {
        base.Update();
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
public class BossCarrotTackle : BossState
{
    int phase = 0;
    int rotate = 0;

    override public void Enter(BossBase bb)
    {
        bb.Freeze(false);
        bb.damage = 20;
    }
    override public void Update(BossBase bb)
    {
        //bb.StartCoroutine(Tackle());
    }
    override public void Exit(BossBase bb)
    {
        bb.Freeze(true);
        bb.damage = 0;
        bb.damaged.Clear();
    }
    private IEnumerator Tackle(BossBase bb)
    {
        switch (phase)
        {
            // 頭下げ
            case 0:
                bb.transform.rotation = Quaternion.Euler(0, 0, rotate += 10);
                if (rotate >= 90) phase = 1;
                break;

            // 頭上げ
            case 1:
                bb.transform.rotation = Quaternion.Euler(0, 0, rotate -= 10);
                if (rotate <= -90)
                {
                    phase = 2;
                    bb.rb.linearVelocity = new(0.0f, -BossCarrotConst.TACKLE_SPEED);
                }
                break;

            // ロックオン
            case 2:
                break;
        }
        yield return null;
    }
}

