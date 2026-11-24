using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bamboo : CharBase
{
    [SerializeField] SimpleDamageArea spear;
    [SerializeField] GameObject wall;

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
        if (!CanUseSkill1 || !ctx.performed) return;

        // 生成
        var obj = Instantiate(spear, transform.position, new(0.0f, 0.0f, 0.0f, 0.0f));
        obj.Init(id, new(10, DamageType.Soundable), new(0.0f, 0.0f), 1.0f, false);
    }

    override public void Skill2(InputAction.CallbackContext ctx)
    {
        if (!CanUseSkill2 || !ctx.performed) return;

        StartCoroutine(CreateWall(0, transform.position, direction));
    }

    IEnumerator CreateWall(int space, Vector3 startPos, Vector2 dir)
    {
        // 生成
        Vector3 createPos = (Vector3)dir * ++space + startPos;
        Destroy(Instantiate(wall, createPos, new(0.0f, 0.0f, 0.0f, 0.0f)), 2.0f);
        yield return new WaitForSeconds(0.1f);

        // 壁の外なら中断
        Collider2D[] objs = Physics2D.OverlapCircleAll(createPos, 0.01f);
        foreach (var obj in objs)
            if (obj.CompareTag("Wall")) yield break;

        // 再帰
        StartCoroutine(CreateWall(space, startPos, dir));
    }
}