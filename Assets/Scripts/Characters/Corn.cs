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
        //speed.generic = data.speed + burst / 20;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    override public void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_1_cooltime > 0 || !can_control) return;
            audioSource.PlayOneShot(se1);

            // 座標・ベクトルの算出
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 弾を生成 -> idの紐づけ
            bullet_obj = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            bullet_obj.GetComponent<DamageArea>().Init(id, 20, direction * 0.5f / 2, true);
            bullet_obj.GetComponent<CornBullet>().Init(id, direction, bullet_obj);
            sr.sprite = img[1];

            // 硬直・クールタイム
            skill_1_cooltime = data.skill_1_cooltime;
            rb.linearVelocity = Vector2.zero;
            can_control = false;
            StartCoroutine(Shoot());
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