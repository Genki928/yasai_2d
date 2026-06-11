using UnityEngine;

public class BossCarrot : BossBase
{
    override protected void Start()
    {
        // フェーズ設定
        state_list.Add(new BossCarrotPhase1());

        // 基底クラス
        base.Start();
    }

    override protected void Update()
    {
        base.Update();
    }
}

public class BossCarrotPhase1 : BossState
{
    override public void Enter(BossBase bb)
    {
        Debug.Log("phae 1 enter");
    }
    override public void Update(BossBase bb)
    {
        ;
    }
    override public void Exit(BossBase bb)
    {
        Debug.Log("phae 1 exit");
    }
}
