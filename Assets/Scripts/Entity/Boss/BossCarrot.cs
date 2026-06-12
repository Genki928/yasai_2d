using UnityEngine;
using UnityEngine.UIElements;

public class BossCarrot : BossBase
{
    override protected void Start()
    {
        // フェーズ設定
        state_list.Add(new BossCarrotTackle());

        // 基底クラス
        base.Start();
    }

    override protected void Update()
    {
        base.Update();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            if (!damaged.Contains(cb.id))
            {
                cb.Damage(damage, id);
                damaged.Add(cb.id);
            }
        }
    }
}

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
        switch(phase)
        {
            case 0:
                bb.transform.rotation = Quaternion.Euler(0, 0, rotate += 10);
                if (rotate >= 90) phase = 1;
                break;

            case 1:
                bb.transform.rotation = Quaternion.Euler(0, 0, rotate -= 10);
                if (rotate <= 0) phase = 0;
                break;
        }
    }
    override public void Exit(BossBase bb)
    {
        bb.Freeze(true);
        bb.damage = 0;
        bb.damaged.Clear();
    }
}
