using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Corn : CharBase
{
    [SerializeField] AudioClip se1;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bomb;
    SpriteRenderer sr;
    [SerializeField] List<Sprite> img = new();
    [NonSerialized] public GameObject bullet_obj;
    GameObject _bomb;
    [SerializeField] GameObject _popcorn;
    [SerializeField] GameObject _explode;
    [SerializeField] AudioClip _move;

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
            if (!CanUseSkill1) return;
            audioSource.PlayOneShot(se1);

            // 座標・ベクトルの算出
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 弾を生成 -> idの紐づけ
            bullet_obj = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            bullet_obj.GetComponent<SimpleDamageArea>().Init(id, new(20, DamageType.Soundable), direction * 15.0f, 0.0f, true);
            bullet_obj.GetComponent<CornBullet>().Init(id, direction, bullet_obj);
            sr.sprite = img[1];

            // 硬直・クールタイム
            rigid += data.skill_1_rigid;
            cooltime[(int)SkillName.Skill1].SetCooltime();
            rb.linearVelocity = Vector2.zero;
            can_control = false;
            StartCoroutine(Shoot());
        }
    }

    public override void Skill2(InputAction.CallbackContext ctx)
    {
        // 中断処理
        if (!(ctx.performed && CanUseSkill2)) return;

        if (_bomb != null)
        {
            // 位置の入れ替え
            Instantiate(_popcorn, _bomb.transform.position, Quaternion.identity);
            _bomb.transform.position = transform.position;

            //
            audioSource.PlayOneShot(_move);
            cooltime[(int)SkillName.Skill2].SetCooltime();

            return;
        }

        // 処理
        _bomb = Instantiate(bomb, transform.position, Quaternion.identity);
        _bomb.GetComponent<Bomb>().Init(id, cooltime[(int)SkillName.Skill2]);

        // 硬直・クールタイム
        cooltime[(int)SkillName.Skill2].SetCooltime();
        cooltime[(int)SkillName.Skill2].RefleshCooltime(0.5f);

    }

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

    public override void Damage(Damage value, int id)
    {
        base.Damage(value, id);
    }
}