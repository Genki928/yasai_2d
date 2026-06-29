using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mushroom : CharBase
{
    [SerializeField] SpriteRenderer sprite;

    public Sprite mushroom_default;
    public Sprite tackle;

    //skill1
    [SerializeField] GameObject collision;
    [SerializeField] private float headBangAngle = 60f;
    [SerializeField] private float rotateSpeed = 350f;
    [SerializeField] private int headBangDamage = 15;
    private List<CharBase> hitList = new List<CharBase>();
    [SerializeField] private CapsuleCollider2D headBangCol;
    [SerializeField] AudioClip se1;
    //skill2
    [SerializeField] GameObject gass;
    [SerializeField] int gass_damage=1;

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
        if(!can_control) sprite.flipX = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // クールタイム中なら終了
            if (skill_1_cooltime != 0 || !can_control) return;
            audioSource.PlayOneShot(se1);
            StartCoroutine(HeadBang());

            rb.linearVelocity = Vector2.zero;
            rigid += data.skill_1_rigid;
            skill_1_cooltime = data.skill_1_cooltime;
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

    override public void Skill2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_2_cooltime > 0 || !can_control) return;
            audioSource.PlayOneShot(se1);

            //画像差し替え

            // 座標・ベクトルの算出
            Vector2 pos = new Vector2(transform.position.x, transform.position.y + 0.7f) + direction * 2.0f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;



            // 弾を生成 -> idの紐づけ
            GameObject go = Instantiate(gass, transform.position, Quaternion.Euler(0, 0, angle));

            go.GetComponent<MushroomGass>().Init(
                id,
                gass_damage,
                direction
            );
            //audioSource.PlayOneShot(se2);

            // 硬直・クールタイム
            skill_2_cooltime = data.skill_2_cooltime;
            rb.linearVelocity = Vector2.zero;
            can_control = false;
            StartCoroutine(Gass());
        }
    }

    IEnumerator Gass()
    {
        yield return new WaitForSeconds(0.2f);
        can_control = true;
        //画像差し替え

    }

    public override Sprite GetDefaultImage()
    {
        return mushroom_default;
    }

    public override void Damage(int value, int id)
    {
        base.Damage(value, id);
    }
}




