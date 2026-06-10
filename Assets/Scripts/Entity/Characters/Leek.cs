using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leek : CharBase
{
    // 斬撃用
    [SerializeField] private GameObject collision;
    [SerializeField] private int skill1Damage = 20;

    // カウンター用
    [SerializeField] private float counterTime = 1.0f;
    [SerializeField] private int counterDamage = 30;

    private bool isCounter = false;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite leek_default;

    protected override void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 通常攻撃
    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        GameObject obj =
            Instantiate(collision, spawnPos, Quaternion.identity);

        HitDamageArea hit =
            obj.GetComponent<HitDamageArea>();

        hit.Init(id, skill1Damage, Vector2.zero);
    }

    // カウンター構え
    public override void Skill2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        StartCoroutine(Counter());
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
        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        GameObject obj =
            Instantiate(collision, spawnPos, Quaternion.identity);

        HitDamageArea hit =
            obj.GetComponent<HitDamageArea>();

        hit.Init(id, counterDamage, Vector2.zero);
    }

    public override void Damage(int damage, int attackerId)
    {
        if (isCounter)
        {
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