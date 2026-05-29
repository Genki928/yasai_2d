using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Carrot : CharBase
{
    [SerializeField] SpriteRenderer sprite;

    public Sprite carrot_default;
    public Sprite tackle;

    // タックル関連
    [SerializeField] private float tackleSpeed = 20f;
    [SerializeField] private float tackleTime = 0.5f;
    [SerializeField] private int tackleDamage = 20;
    [SerializeField] GameObject dust;
    [SerializeField] GameObject collision;
    [SerializeField] AudioClip se1;
    private List<CharBase> tackleHitList = new List<CharBase>();

    //ヘドバン関連
    [SerializeField] private float headBangAngle = 60f;
    [SerializeField] private float rotateSpeed = 350f;
    [SerializeField] private int headBangDamage = 15;
    private List<CharBase> hitList = new List<CharBase>();
    [SerializeField] private CapsuleCollider2D headBangCol;
    [SerializeField] AudioClip se2;

    private Vector2 defaultSize;
    private bool isHeadBanging = false;

    override protected void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
        defaultSize = headBangCol.size;
    }

    override protected void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    override public void Skill2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_1_cooltime != 0) return;

            // タックル開始
            audioSource.PlayOneShot(se2);
            StartCoroutine(Tackle());
            Debug.Log("tackle");

            // 硬直・クールタイム
            rigid += data.skill_1_rigid;
            skill_1_cooltime = data.skill_1_cooltime;
            Vector2 pos = new(transform.position.x, transform.position.y);
            GameObject obj = Instantiate(dust, pos, Quaternion.identity);
            if (direction.x > 0) obj.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private IEnumerator Tackle()
    {
        tackleHitList.Clear();

        sprite.sprite = tackle;

        can_control = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        rb.linearVelocity = direction * tackleSpeed;

        yield return new WaitForSeconds(tackleTime);

        rb.linearVelocity = Vector2.zero;

        can_control = true;
        sprite.sprite = carrot_default;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // クールタイム中なら終了
            if (skill_2_cooltime != 0) return;
            audioSource.PlayOneShot(se1);
            StartCoroutine(HeadBang());

            rb.linearVelocity = Vector2.zero;
            rigid += data.skill_2_rigid;
            skill_2_cooltime = data.skill_2_cooltime;
        }
    }
    private IEnumerator HeadBang()
    {
        hitList.Clear();
        isHeadBanging = true;

        float startZ = transform.eulerAngles.z;

        float sign = direction.x < 0 ? -1f : 1f;

        float currentAngle = 0;

        // ===== 当たり判定生成 =====

        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        GameObject obj =
            Instantiate(collision, spawnPos, Quaternion.identity);

        HitDamageArea hit =
            obj.GetComponent<HitDamageArea>();

        hit.Init(id, headBangDamage, Vector2.zero);

        // =========================

        // 倒す
        while (currentAngle < headBangAngle)
        {
            currentAngle += rotateSpeed * Time.deltaTime;

            transform.rotation =
                Quaternion.Euler(0, 0, startZ - currentAngle * sign);

            yield return null;
        }

        // 戻す
        while (currentAngle > 0)
        {
            currentAngle -= rotateSpeed * Time.deltaTime;

            transform.rotation =
                Quaternion.Euler(0, 0, startZ - currentAngle * sign);

            yield return null;
        }

        // 元に戻す
        transform.rotation =
            Quaternion.Euler(0, 0, startZ);

        headBangCol.size = defaultSize;
        isHeadBanging = false;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            // タックル
            if (cb.id != id && !can_control)
            {
                // 既に当たっていたら無効
                if (tackleHitList.Contains(cb)) return;

                tackleHitList.Add(cb);

                cb.Damage(tackleDamage);

                Vector2 knockbackDir =
                    (cb.transform.position - transform.position).normalized;

                cb.KnockBack(10, knockbackDir);
            }
        }
    }

}




