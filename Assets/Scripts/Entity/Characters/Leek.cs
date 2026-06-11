using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leek : CharBase
{
    // 斬撃用
    [SerializeField] private GameObject collision;
    [SerializeField] private int skill1Damage = 20;
    public bool isSrash=false;

    // カウンター用
    [SerializeField] private GameObject countercircle;
    [SerializeField] private float counterTime = 1.0f;
    [SerializeField] private int counterDamage = 50;

    private bool isCounter = false;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite leek_default;

    protected override void Start()
    {
        base.Start();
        speed = data.speed;
        sprite = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        if(isCounter==true) speed = 3;
        else speed = 5;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 通常攻撃
    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // 中断処理
        if (skill_1_cooltime != 0 || !can_control) return;

        // 生成位置
        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        // direction の角度を取得
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // プレハブのデフォルトが左向きなので180°補正
        Quaternion rot = Quaternion.Euler(0, 0, angle + 180f);

        // 生成
        GameObject obj = Instantiate(collision, spawnPos, rot);

        // ダメージ設定
        HitDamageArea hit = obj.GetComponent<HitDamageArea>();
        hit.Init(id, skill1Damage, Vector2.zero);

        // 硬直・クールタイム
        rigid += data.skill_1_rigid;
        skill_1_cooltime = data.skill_1_cooltime;
    }

    // カウンター構え
    public override void Skill2(InputAction.CallbackContext ctx)
    {
      
        if (!ctx.performed) return;

        // 中断処理
        if (skill_1_cooltime != 0 || !can_control) return;

        GameObject obj = Instantiate(countercircle);

        CounterCircle circle = obj.GetComponent<CounterCircle>();
        circle.owner = this;   // this は Leek

        StartCoroutine(Counter());

        // 硬直・クールタイム
        rigid += data.skill_2_rigid;
        skill_2_cooltime = data.skill_2_cooltime;
    }

    private IEnumerator Counter()
    {
        isCounter = true;

        yield return new WaitForSeconds(counterTime);

        isCounter = false;
    }

    // カウンター攻撃
    private void CounterAttack()
    {
        // 生成位置
        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        // direction の角度を取得
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // プレハブのデフォルトが左向きなので180°補正
        Quaternion rot = Quaternion.Euler(0, 0, angle + 180f);

        // 生成
        GameObject obj = Instantiate(collision, spawnPos, rot);

        // ダメージ設定
        HitDamageArea hit = obj.GetComponent<HitDamageArea>();
        hit.Init(id, counterDamage, Vector2.zero);
    }

    public override void Damage(int damage, int attackerId)
    {
        if (isCounter)
        {
            Debug.Log("Counter!");

            CounterAttack();
            isCounter = false;
            return;
        }

        base.Damage(damage, attackerId);
    }

    public override Sprite GetDefaultImage()
    {
        return leek_default;
    }
}