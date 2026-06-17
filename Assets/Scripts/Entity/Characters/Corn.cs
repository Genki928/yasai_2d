using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Corn : CharBase
{
    [SerializeField] AudioClip se1;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bomb;
    SpriteRenderer sr;
    [SerializeField] List<Sprite> img = new();
    [NonSerialized] public GameObject bullet_obj;
    public GameObject bomb_obj;
    [SerializeField] Sprite popcorn;
    
    //炎SE
    override protected void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
    }

    override protected void Update()
    {
        base.Update();
        speed.generic = data.speed + burst / 20;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    override public void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (bullet_obj == null)
            {
                // 中断処理
                if (skill_1_cooltime > 0 || !can_control) return;
                audioSource.PlayOneShot(se1);

                // 座標・ベクトルの算出
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // 弾を生成 -> idの紐づけ
                bullet_obj = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
                bullet_obj.GetComponent<DamageArea>().Init(id, 10, direction * 0.5f, true);
                sr.sprite = img[1];

                // 硬直・クールタイム
                skill_1_cooltime = data.skill_1_cooltime;
                rb.linearVelocity = Vector2.zero;
                can_control = false;
                StartCoroutine(Shoot());
            }
            else
            {
                // 爆発生成
                GameObject particle = Instantiate(bomb_obj, bullet_obj.transform.position, Quaternion.identity);
                particle.GetComponent<DamageArea>().Init(id, 10, new(0, 0), true, true);
                
                // ポップコーン生成
                bullet_obj.GetComponent<SpriteRenderer>().sprite = popcorn;
                bullet_obj.GetComponent<DamageArea>().Init(id, 0, new(0, 0), false);
                Rigidbody2D rb_c = bullet_obj.GetComponent<Rigidbody2D>();
                rb_c.gravityScale = 5.0f;
                rb_c.linearVelocity = new(direction.x, 15.0f);
                bullet_obj = null;
            }
        }
    }

    public override void Skill2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_2_cooltime > 0 || !can_control) return;
            //audioSource.PlayOneShot(se1);

            // 処理
            GameObject go = Instantiate(bomb, transform.position, Quaternion.identity);
            go.GetComponent<Bomb>().Init(id);

            // 硬直・クールタイム
            skill_2_cooltime = data.skill_2_cooltime;
            //rigid += data.skill_2_rigid;
        }
    }

    //IEnumerator BackShot()
    //{
    //    yield return new WaitForSeconds(0.05f);
    //    can_control = true;
    //    rb.linearVelocity = Vector2.zero;
    //    rigid += data.skill_2_rigid;
    //}

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.2f);
        can_control = true;
        sr.sprite = img[0];
    }

    public override Sprite GetDefaultImage()
    {
        return img[0];
    }

    public override void Damage(int value, int id)
    {
        base.Damage(value, id);
    }
}