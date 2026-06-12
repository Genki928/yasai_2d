using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Corn : CharBase
{
    [SerializeField] AudioClip se1;
    [SerializeField] GameObject bullet;
    SpriteRenderer sr;
    [SerializeField] List<Sprite> img = new();
    //炎SE
    override protected void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
    }

    override protected void Update()
    {
        base.Update();
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

            //// 弾を生成 -> idの紐づけ
            GameObject go = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            go.GetComponent<DamageArea>().Init(id, 1, direction * 0.5f, true);
            Damage(5, id);
            sr.sprite = img[1];

            //GameObject go = Instantiate(breath, pos, Quaternion.Euler(0, 0, angle - 90));
            //go.GetComponent<DamageArea>().Init(id, 1, new Vector2(0, 0));

            //// 硬直・クールタイム
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
            //    // 中断処理
            //    if (skill_2_cooltime > 0 || !can_control) return;
            //    audioSource.PlayOneShot(se1);
            //    // 処理
            //    rb.linearVelocity = -direction * 30.0f;
            //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //    GameObject go = Instantiate(breath, transform.position, Quaternion.Euler(0, 0, angle - 90));
            //    go.GetComponent<DamageArea>().Init(id, 1, direction * 0.05f);

            //    // 硬直・クールタイム
            //    can_control = false;
            //    skill_2_cooltime = data.skill_2_cooltime;
            //    StartCoroutine(BackShot());
            Heal(10);
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