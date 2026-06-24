using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterMelon : CharBase
{
    [SerializeField] SpriteRenderer sprite;

    public Sprite WaterMelon_default;
    public Sprite WaterMelon_skill;
    private bool isShoot;


    //skill1
    [SerializeField] GameObject bullet;
    [SerializeField]static int bullet_damage = 10;
    [SerializeField] AudioClip bullet_sound;

    //skill2
    [SerializeField] GameObject cutter;
    [SerializeField] static int cutter_damage = 60;
    private bool isCutter;
    [SerializeField] AudioClip cutter_sound;
    [SerializeField] AudioClip charge_sound;



    private Vector2 defaultSize;
    private bool isHeadBanging = false;

    override protected void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    override protected void Update()
    {
        base.Update();
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
            if (skill_1_cooltime != 0 || !can_control||isCutter) return;

            StartCoroutine(WaterMelonShoot());
      
           
            rb.linearVelocity = Vector2.zero;
            rigid += data.skill_1_rigid;
            skill_1_cooltime = data.skill_1_cooltime;
        }
    }

    private IEnumerator WaterMelonShoot()
    {
        isShoot = true;
        sprite.sprite = WaterMelon_skill;
        speed.generic = 2;
        // 座標・ベクトルの算出
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 弾を生成 -> idの紐づけ
        GameObject go = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
        go.GetComponent<DamageArea>().Init(id, bullet_damage, direction * 0.5f / 2, true);
        audioSource.PlayOneShot(bullet_sound);
        yield return new WaitForSeconds(0.1f);
        // 座標・ベクトルの算出
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 弾を生成 -> idの紐づけ
        go = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
        go.GetComponent<DamageArea>().Init(id, bullet_damage, direction * 0.5f / 2, true);
        audioSource.PlayOneShot(bullet_sound);
        yield return new WaitForSeconds(0.1f);
        // 座標・ベクトルの算出
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 弾を生成 -> idの紐づけ
        go = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
        go.GetComponent<DamageArea>().Init(id, bullet_damage, direction * 0.5f / 2, true);
        audioSource.PlayOneShot(bullet_sound);
        yield return new WaitForSeconds(0.1f);
        sprite.sprite = WaterMelon_default;
        speed.generic = 4;
        isShoot = false;
    }
   
    override public void Skill2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_2_cooltime != 0 || !can_control||isShoot) return;

            StartCoroutine(WaterMelonCutter());

            // 硬直・クールタイム
            rb.linearVelocity = Vector2.zero;
            rigid += data.skill_2_rigid;
            skill_2_cooltime = data.skill_2_cooltime;
        }
    }

    private IEnumerator WaterMelonCutter()
    {
        audioSource.PlayOneShot(charge_sound);
        isCutter = true;
        sprite.sprite = WaterMelon_skill;
        speed.generic = 1;
        yield return new WaitForSeconds(0.7f);
        // 座標・ベクトルの算出
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 弾を生成 -> idの紐づけ
        GameObject go = Instantiate(cutter, transform.position, Quaternion.Euler(0, 0, angle - 270f));
        go.GetComponent<DamageArea>().Init(id, cutter_damage, direction * 0.7f / 2, true);
        audioSource.PlayOneShot(cutter_sound);
        yield return new WaitForSeconds(0.2f);
        sprite.sprite = WaterMelon_default;
        isCutter = false;
        speed.generic = 4;
    }

    public override Sprite GetDefaultImage()
    {
        return WaterMelon_default;
    }

    public override void Damage(int value, int id)
    {
        base.Damage(value, id);
    }
}




