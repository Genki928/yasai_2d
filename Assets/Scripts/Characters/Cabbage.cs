using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// キャベツ
/// Skill1: 溜め攻撃のブーメラン(ボタンを押して溜め、離して発射。最大5秒溜められる)
/// Skill2: カッターを周りに3つ順番に生成し、一番近い敵へ一直線に飛ばす(敵がいなければ生成時の向きへ)
/// </summary>
public class Cabbage : CharBase
{
    [Header("◇キャベツ共通")]
    [SerializeField] AudioClip se1;
    [SerializeField] List<Sprite> img = new();   // 0:通常 1:溜め・投げ

    [Header("◇Skill1 ブーメラン")]
    [SerializeField] GameObject boomerangPrefab;     // Boomerang が付いたプレハブ
    [SerializeField] float maxChargeSeconds = 5.0f;  // 最大溜め時間
    [SerializeField] int chargeStages = 5;           // 溜めの段階数(最大溜めまでを均等に分割)
    [SerializeField] AudioClip chargeStageSe;        // 段階が上がるたびに鳴らすSE
    [SerializeField] AudioClip chargeMaxSe;          // 最大溜めに達したときのSE(空なら chargeStageSe)
    [SerializeField] int minDamage = 8;             // 溜めなしの1ヒットあたりのダメージ
    [SerializeField] int maxDamage = 30;             // 最大溜めの1ヒットあたりのダメージ
    [SerializeField] float maxScale = 1.5f;          // 最大溜めのときの大きさ倍率
    [SerializeField] float boomerangSpeed = 12.0f;
    [SerializeField] float outDistance = 8.0f;       // 行きの飛距離
    [SerializeField] float hitInterval = 0.3f;       // 多段ヒットの間隔
    [SerializeField] float throwSeconds = 0.2f;      // 投げたあとの硬直(操作不可)の長さ
    [SerializeField, Range(0f, 1f)] float chargeMoveMultiplier = 0.3f;  // 溜め中の移動速度の倍率
    [SerializeField] Color chargeColor = new(1f, 0.9f, 0.3f);   // 溜め中に染める色
    [SerializeField, Range(0f, 1f)] float maxChargeFlash = 0.6f; // 最大溜めのときの染まり具合(1で完全にchargeColor)

    [Header("◇Skill2 カッター")]
    [SerializeField] GameObject cutterPrefab;         // Cutter が付いたプレハブ
    [SerializeField] AudioClip cutterSpawnSe;         // 1つ生成するたびに鳴らすSE
    [SerializeField] int cutterCount = 3;             // 生成する数
    [SerializeField] float cutterSpawnInterval = 0.15f; // 生成の間隔
    [SerializeField] float cutterOrbitRadius = 1.0f;  // 自分からの距離
    [SerializeField] float cutterHorizontalScale = 1.6f; // 横方向の広がり(大きいほど左右の間隔が開く)
    [SerializeField] float cutterSideRaise = 0.5f;    // 左右の葉を追加で上げる量
    [SerializeField] float cutterHeightOffset = 0.8f; // 円の中心を上にずらす量(大きいほど高い位置)
    [SerializeField] float cutterLaunchDelay = 0.4f;  // 生成してから発射するまで
    [SerializeField] int cutterDamage = 10;
    [SerializeField] float cutterSpeed = 18.0f;
    [SerializeField] float cutterLifetime = 2.0f;     // 発射してから消えるまで
    [SerializeField] float cutterSearchRadius = 12.0f; // 敵を探す範囲(0以下なら無制限)

    [NonSerialized] public GameObject boomerang_obj;

    bool charging = false;
    float chargeTime = 0f;

    // ---------------------------------------------------------
    // Skill1: 溜めブーメラン
    // ---------------------------------------------------------
    InputAction skill1Action;   // 押している間かどうかを毎フレーム確認するために保持
    int chargeStage = 0;        // 現在の溜め段階(SEを鳴らす判定用)
    float releaseTimer = 0f;   // ボタンが離れている時間
    [SerializeField] float releaseGraceSeconds = 0.12f;  // これ以上離れ続けたら「離した」とみなす

    public override void Skill1(InputAction.CallbackContext ctx)
    {
        // Interaction の設定に左右されないよう、「押された」ことだけをここで受け取る。
        // 離したかどうかは Update でボタンの状態を見て判定する。
        if ((ctx.started || ctx.performed) && ctx.action.IsPressed())
        {
            skill1Action = ctx.action;
            StartCharge();
        }
    }

    void StartCharge()
    {
        if (charging) return;
        if (boomerang_obj != null) return;   // 前のブーメランが戻ってくるまで撃てない
        if (!CanUseSkill1) return;

        // 溜め中も移動できる(速度は FixedUpdate で遅くする)
        charging = true;
        chargeTime = 0f;
        chargeStage = 0;
        releaseTimer = 0f;
        SetSprite(1);
    }

    // 溜め中は移動速度を落とす
    protected override void FixedUpdate()
    {
        if (charging && can_control)
        {
            rb.linearVelocity = rigid == 0
                ? vec * moveSpeed.Value * chargeMoveMultiplier
                : Vector2.zero;
            return;
        }
        base.FixedUpdate();
    }


    protected override void Update()
    {
        base.Update();

        if (!charging) return;

        // ボタンを離したら発射(押してから離すまでが溜め時間)
        // 入力が一瞬途切れる(チャタリング)場合に備え、離れた状態が一定時間続いたときだけ発射する
        if (skill1Action != null && skill1Action.IsPressed()) releaseTimer = 0f;
        else releaseTimer += Time.deltaTime;

        if (skill1Action == null || releaseTimer >= releaseGraceSeconds)
        {
            Release();
            return;
        }

        chargeTime = Mathf.Min(chargeTime + Time.deltaTime, maxChargeSeconds);

        // 段階が上がったらSE
        int stage = Mathf.FloorToInt(chargeTime / maxChargeSeconds * chargeStages);
        if (stage > chargeStage)
        {
            chargeStage = stage;
            AudioClip clip = (stage >= chargeStages && chargeMaxSe != null) ? chargeMaxSe : chargeStageSe;
            if (clip != null) audioSource.PlayOneShot(clip);
        }

        // 溜め具合を色で表現(点滅用マテリアルで chargeColor に染める)
        SetChargeFlash(chargeTime / maxChargeSeconds * maxChargeFlash);
    }

    void Release()
    {
        Debug.Log($"[Cabbage Release] chargeTime={chargeTime:F2} isPressed={(skill1Action != null && skill1Action.IsPressed())} t={Time.time:F2}");
        charging = false;
        float t = Mathf.Clamp01(chargeTime / maxChargeSeconds);

        SetChargeFlash(0f);
        audioSource.PlayOneShot(se1);

        // 溜め具合でダメージ(と大きさ)が上がる
        int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, t));

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        boomerang_obj = Instantiate(boomerangPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        boomerang_obj.transform.localScale *= Mathf.Lerp(1f, maxScale, t);
        boomerang_obj.GetComponent<Boomerang>().Init(this, direction, damage, boomerangSpeed, outDistance, hitInterval);

        // 投げ終わりの硬直・クールタイム
        rigid += data.skill_1_rigid;
        cooltime[(int)SkillName.Skill1].SetCooltime();
        rb.linearVelocity = Vector2.zero;
        can_control = false;
        StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        yield return new WaitForSeconds(throwSeconds);
        can_control = true;
        SetSprite(0);
    }

    // ---------------------------------------------------------
    // Skill2: カッター(周りに3つ順番に生成 → 一番近い敵へ一直線)
    // ---------------------------------------------------------
    public override void Skill2(InputAction.CallbackContext ctx)
    {
        if (!(ctx.performed && CanUseSkill2)) return;

        cooltime[(int)SkillName.Skill2].SetCooltime();
        StartCoroutine(SpawnCutters());
    }

    IEnumerator SpawnCutters()
    {
        // 生成時の向きを記録(敵がいないときはこの向きへ飛ぶ)
        Vector2 dir = direction == Vector2.zero ? Vector2.right : direction.normalized;

        for (int i = 0; i < cutterCount; i++)
        {
            if (cutterSpawnSe != null) audioSource.PlayOneShot(cutterSpawnSe);

            // 周りに等間隔で配置(1つ目は真上から)
            float rad = (90f + 360f / cutterCount * i) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(rad) * cutterHorizontalScale, Mathf.Sin(rad)) * cutterOrbitRadius
                             + Vector2.up * cutterHeightOffset;
            if (i != 0) offset.y += cutterSideRaise;   // 左右の葉だけ追加で上げる(1つ目=真上は動かさない)

            var obj = Instantiate(cutterPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            obj.GetComponent<CabbageCutter>().Init(
                this, offset, dir, cutterDamage,
                cutterSpeed, cutterLifetime, cutterLaunchDelay, cutterSearchRadius);

            yield return new WaitForSeconds(cutterSpawnInterval);
        }
    }

    // ---------------------------------------------------------
    // 共通
    // ---------------------------------------------------------
    // CharBase の被ダメ点滅と同じシェーダープロパティで、溜め中の色を表現する
    static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    MaterialPropertyBlock chargeMpb;

    void SetChargeFlash(float amount)
    {
        if (sprite == null) return;
        chargeMpb ??= new MaterialPropertyBlock();
        sprite.GetPropertyBlock(chargeMpb);
        chargeMpb.SetColor(FlashColorID, chargeColor);
        chargeMpb.SetFloat(FlashAmountID, amount);
        sprite.SetPropertyBlock(chargeMpb);
    }

    void SetSprite(int index)
    {
        // CharBase.Start で取得済みの sprite(SpriteRenderer)を使う
        if (sprite != null && index < img.Count)
            sprite.sprite = img[index];
    }

    public override Sprite GetDefaultImage()
    {
        return img.Count > 0 ? img[0] : null;
    }
}