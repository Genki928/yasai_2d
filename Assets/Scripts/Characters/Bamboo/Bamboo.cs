using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bamboo : CharBase
{
    [SerializeField] GameObject spear;
    [SerializeField] GameObject wall;

    // 長押し検知
    [SerializeField] InputActionReference hold;
    [SerializeField] GameObject createPointPrefab;
    GameObject createPoint;
    [SerializeField] float MAX_DISTANCE = 1.0f;
    InputAction holdAction;
    float holdPer = 0.0f;

    override protected void Start()
    {
        base.Start();
        holdAction = hold;
    }

    override protected void Update()
    {
        base.Update();

        // スキル2ボタンの長押しの取得、されていなければ中断
        if (createPoint == null) return;

        // 壁を生成する位置の算出
        holdPer = holdAction.GetTimeoutCompletionPercentage();
        var distance = holdPer * MAX_DISTANCE;
        createPoint.transform.position = transform.position + (Vector3)direction * distance;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        // ベクトルの取得
        vec = ctx.ReadValue<Vector2>();

        // 向きを保存
        if (vec != new Vector2(0, 0))
        {
            cursor_obj.Refresh(vec);
            if (holdPer == 0.0f)
                direction = vec;
        }
    }

    protected override void FixedUpdate()
    {
        if (can_control)
        {
            // 硬直が無ければ移動
            if (rigid == 0 && holdPer == 0.0f)
                rb.linearVelocity = vec * moveSpeed.Value;
            // 硬直があれば移動不可
            else
                rb.linearVelocity = Vector2.zero;
        }
        else if (burst >= max_burst) rb.linearVelocity = Vector2.zero;
    }

    override public void Skill1(InputAction.CallbackContext ctx)
    {
        if (!CanUseSkill1 || !ctx.performed) return;

        // 生成
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        Vector2 pos = new(transform.position.x, transform.position.y + 0.5f);
        var obj = Instantiate(spear, pos, Quaternion.Euler(0.0f, 0.0f, angle));
        StartCoroutine(obj.GetComponent<BambooSpear>().Spear(direction));
        obj.GetComponent<SimpleDamageArea>().Init(id, new(15, DamageType.Soundable), new(0.0f, 0.0f), 0.5f, false);

        rigid += data.skill_1_rigid;
        cooltime[(int)SkillName.Skill1].SetCooltime();
    }

    override public void Skill2(InputAction.CallbackContext ctx)
    {
        if (!CanUseSkill2) return;

        if (ctx.performed)
        {
            createPoint = Instantiate(createPointPrefab);
        }
        else if (ctx.canceled)
        {
            StartCoroutine(CreateWall(createPoint.transform.position));
            Destroy(createPoint);

            rigid += data.skill_2_rigid;
            cooltime[(int)SkillName.Skill2].SetCooltime();
            holdPer = 0.0f;
        }
    }

    IEnumerator CreateWall(Vector3 center)
    {
        Vector3 perpendicular = new Vector3(-direction.y, direction.x).normalized;

        // 中央
        Destroy(Instantiate(wall, center, Quaternion.identity), 2.0f);
        yield return new WaitForSeconds(0.1f);

        // 左右に2個ずつ
        for (int i = 1; i <= 2; i++)
        {
            Destroy(Instantiate(wall, center + perpendicular * 0.8f * i, Quaternion.identity), 2.0f);
            Destroy(Instantiate(wall, center - perpendicular * 0.8f * i, Quaternion.identity), 2.0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}