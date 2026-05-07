using UnityEngine;
using UnityEngine.InputSystem;

public class Carrot : CharBase
{
    override protected void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
        base.Update();
    }

    override public void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ;
        }
    }
}