using UnityEngine;
using UnityEngine.InputSystem;

public class Leek : CharBase
{
    //[SerializeField] Collider2D[] colliders = Collider2D[4];

    override protected void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Skill1(InputAction.CallbackContext ctx)
    {
        ;
    }

    public override void Skill2(InputAction.CallbackContext ctx)
    {
        ;
    }
}
