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
            // 中断処理
            if (skill_1_cooltime != 0) return;

            // 座標・ベクトルの算出
            Vector2 pos = new Vector2(transform.position.x, transform.position.y) + direction * 1.5f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 炎を生成 -> idの紐づけ
            GameObject go = Instantiate(breath, pos, Quaternion.Euler(0, 0, angle - 90));
            go.GetComponent<DamageArea>().Init(id);

            // 硬直・クールタイム
            rigid += data.skill_1_rigid;
            skill_1_cooltime = data.skill_1_cooltime;
        }
    }
}