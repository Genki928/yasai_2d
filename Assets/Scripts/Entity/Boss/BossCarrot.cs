using UnityEngine;

public class BossCarrot : BossBase
{
    
    override protected void Start()
    {
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
