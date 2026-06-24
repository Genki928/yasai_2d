using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

static class Winner
{
    static public string w_name = "";
    static public int w_id;
    static public Sprite sprite;

    public static void Reset()
    {
        w_name = "";
        w_id = 0;
        sprite = null;
    }
}
public class BattleManager : MonoBehaviour
{
    bool isdeath = false;
    const int PLAYER_CNT = 2;

    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public Spawner[] spawn_point = new Spawner[PLAYER_CNT];
    GameObject[] player = new GameObject[PLAYER_CNT];
    CharBase[] datas = new CharBase[PLAYER_CNT];
    int[] pick_nums = { 0, 1 };

    [Header("◇GUI")]
    public GUI[] gui = new GUI[PLAYER_CNT];

    public GameObject[] player_obj = new GameObject[PLAYER_CNT];
    GameObject[] p_obj = new GameObject[PLAYER_CNT];


    //演出
    [SerializeField] GameObject deathEffect;
    [SerializeField] GameObject burstEffect;
    [SerializeField] private AudioClip se;
    [SerializeField] private AudioClip se1;
    protected bool sceneLoad = false;
    //オーディオソース用
    public AudioSource audioSource;

    void Awake()
    { 
        Winner.Reset();
        Application.targetFrameRate = 60;
        CharBase.OnPlayerDies += Finish;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < PLAYER_CNT; i++)
        {
            p_obj[i] = Instantiate(player_obj[i]);

            // プレイヤー生成
            pick_nums[i] = PlayerPick.pick[i];
            PlayerInput pi;

            if (i == 0)
            {
                pi = PlayerInput.Instantiate(
                     characters[pick_nums[i]].chars,
                    playerIndex: i,
                    controlScheme: "Controller2",
                    pairWithDevice: Gamepad.all[0]
                );
            }
            else
            {
                pi = PlayerInput.Instantiate(
                     characters[pick_nums[i]].chars,
                    playerIndex: i,
                    controlScheme: "Controller1",
                    pairWithDevice: Gamepad.all[1]
                );
            }
            pi.transform.position = spawn_point[i].point.transform.position;
            pi.transform.rotation = Quaternion.identity;

            // 識別IDを設定
            player[i] = pi.gameObject;
            datas[i] = player[i].GetComponent<CharBase>();
            datas[i].id = i;

            datas[i].direction = SetDirect(spawn_point[i].direct);

            // バーストバーとの紐づけ
            gui[i].bar.Init(player[i]);

            // 各種UIとの紐づけ
            if (player[i].TryGetComponent<CharBase>(out var p))
            {
                p.burst_bar = gui[i].bar;   // バースト
                gui[i].name.text = p.data.char_name;    // キャラ名
                p.cooltimer[0] = gui[i].skill1_cooltimer;   // スキル1のクールタイムを表示
                p.cooltimer[1] = gui[i].skill2_cooltimer;   // スキル2のクールタイムを表示
                gui[i].icon.sprite = characters[pick_nums[i]].icon; // アイコン
            }
        }
    }

    void Update()
    {
        p_obj[0].transform.position = new(player[0].transform.position.x, player[0].transform.position.y + 2.0f);
        p_obj[1].transform.position = new(player[1].transform.position.x, player[1].transform.position.y + 2.0f);
    }

    void OnDestroy()
    {
        CharBase.OnPlayerDies -= Finish;
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        if (isdeath) return;

        isdeath = true;

        Winner.w_id = id;
        Winner.w_name = datas[id].data.char_name;
        Winner.sprite = datas[id].GetDefaultImage();

        int loseId = id == 0 ? 1 : 0;

        StartCoroutine(GameOverEffect(loseId));
    }
    /// <summary>
    /// 演出用コルーチン
    /// </summary>
    /// <param name="direct"></param>
    /// <returns></returns>
    IEnumerator GameOverEffect(int loseId)
    {
     
        Camera cam = Camera.main;

        GameObject loser = player[loseId];
        Rigidbody2D rb = loser.GetComponent<Rigidbody2D>();

        Vector3 originalPos = cam.transform.position;
        float originalSize = cam.orthographicSize;

        Vector3 zoomPos = new(loser.transform.position.x, loser.transform.position.y + 0.5f, loser.transform.position.z);
        zoomPos.z = originalPos.z;

        loser.GetComponent<CharBase>().can_control = false;
        rb.linearVelocity = Vector2.zero;

        //ズーム
        //SE
        audioSource.PlayOneShot(se);

        float targetSize = 2.2f;
        float t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;

            cam.transform.position =
                Vector3.Lerp(originalPos, zoomPos, t / 0.1f);

            cam.orthographicSize =
                Mathf.Lerp(originalSize, targetSize, t / 0.1f);

            yield return null;
        }

        //ス〇ブラ風シェイク
        float shakeTime = 2.0f;

        while (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            Vector2 shake =
                UnityEngine.Random.insideUnitCircle * 0.25f;

            cam.transform.position =
                zoomPos + new Vector3(shake.x, shake.y, 0);

            yield return null;
        }

        ////カメラを戻す
        //t = 0;

        //while (t < 0.2f)
        //{
        //    t += Time.deltaTime;

        //    cam.transform.position =
        //        Vector3.Lerp(zoomPos, originalPos, t / 0.2f);

        //    cam.orthographicSize =
        //        Mathf.Lerp(targetSize, originalSize, t / 0.2f);

        //    yield return null;
        //}
        audioSource.PlayOneShot(se1);
        // エフェクト生成
        Instantiate(
            deathEffect,
            loser.transform.position,
            Quaternion.identity);

        Instantiate(
         burstEffect,
         new Vector3(
             Camera.main.transform.position.x,
             Camera.main.transform.position.y,
             0),
         Quaternion.identity);

        // プレイヤーを画面外へ
        loser.transform.position = new Vector3(1000, 1000, 0);//座標移動で物理的に見えなくしてる

        yield return new WaitForSeconds(4.5f);

        // シーン切り替え
        SceneManager.LoadScene("ResultScene");
    }

    Vector2 SetDirect(DIRECT direct)
    {
        if (direct == DIRECT.RIGHT) return new(1.0f, 0.0f);
        if (direct == DIRECT.LEFT) return new(-1.0f, 0.0f);
        if (direct == DIRECT.UP) return new(0.0f, 1.0f);
        if (direct == DIRECT.DOWN) return new(0.0f, -1.0f);
        return Vector2.zero;
    }
}

[Serializable]
public class GUI
{
    public BurstBar bar;
    public Text name;
    public SpriteRenderer icon;
    public SkillCooltimer skill1_cooltimer;
    public SkillCooltimer skill2_cooltimer;
}

[Serializable]
public class Character
{
    public GameObject chars;
    public Sprite icon;
}

[Serializable]
public class Spawner
{
    public GameObject point;
    public DIRECT direct;
}

public enum DIRECT
{
    RIGHT,
    LEFT,
    UP,
    DOWN
}