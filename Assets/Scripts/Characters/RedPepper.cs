using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class RedPepper : CharBase
{
    [SerializeField] GameObject breath;

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
            Vector2 pos = new Vector2(transform.position.x, transform.position.y) + direction * 1.5f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(breath, pos, Quaternion.Euler(0, 0, angle + 135));
        }
    }
}