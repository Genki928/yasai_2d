using UnityEngine;

public abstract class BossState
{
    //protected BossStateMachine stateMachine;

    //public BossState(BossStateMachine stateMachine)
    //{
    //    this.stateMachine = stateMachine;
    //}

    public abstract void Enter(BossBase bb);
    public abstract bool Update();
    public abstract void Exit();
}