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
}

public class BossCarrotTackle : BossState
{
    int phase = 0;
    int rotate = 0;

    override public void Enter(BossBase bb)
    {
        Debug.Log("phae 1 enter");
    }
    override public void Update(BossBase bb)
    {
        switch(phase)
        {
            case 0:
                bb.Freeze(false);
                bb.transform.rotation = Quaternion.Euler(0, 0, rotate += 10);
                if (rotate >= 90) phase = 1;
                break;

            case 1:
                break;
        }
    }
    override public void Exit(BossBase bb)
    {
        Debug.Log("phae 1 exit");
    }
}
