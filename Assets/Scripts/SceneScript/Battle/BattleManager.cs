using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public TextMeshProUGUI fpsText; private float updateInterval = 0.5f; private float accum = 0.0f; private int frames = 0; private float timeLeft;

    //演出
    //開始
    //public Text fpsText;
    [Header("Start")]
    [SerializeField] Text readyText;
    [SerializeField] Text goText;
    private Vector3 defaultCameraPos;
    private float defaultCameraSize;
    [SerializeField] private AudioClip start_se;

    //終了
    [Header("End")]
    [SerializeField] GameObject deathEffect;
    [SerializeField] GameObject burstEffect;
    [SerializeField] Text koText;
    [SerializeField] private AudioClip se;
    [SerializeField] private AudioClip se1;
    protected bool sceneLoad = false;

    //バトルカメラ
    [Header("Battle Camera")]
    [SerializeField] float followSpeed = 5f;
    // 画面の余白
    [SerializeField] float horizontalMargin = 2.0f;
    // 最小・最大ズーム
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 9f;
    // Y座標固定
    [SerializeField] float cameraY = 0f;
    //カメラ座標
    [SerializeField] float verticalOffset = 1.5f;

    [SerializeField] float bottomLimit = -1f;
    [SerializeField] float topLimit = 6f;
    // 演出中はfalse
    bool battleCamera = false;

    //オーディオソース用
    public AudioSource audioSource;
    [SerializeField] ShakeCamera shake;

    [SerializeField] Timer timer;
    bool is_suddendeath = false;
    int suddendeath_timer_limit = 60;
    int suddendeath_timer_current = 0;

    void Awake()
    { 
        Winner.Reset();
        Application.targetFrameRate = 60;
        CharBase.OnPlayerDies += Finish;
        timer.OnFinish += SuddenDeath;
    }
    void Start()
    {
        //カメラ取得
        Camera cam = Camera.main;

        defaultCameraPos = cam.transform.position;
        defaultCameraSize = cam.orthographicSize;


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
            //datas[i].camera = shake;
        }
        for (int i = 0; i < PLAYER_CNT; i++)
        {
            datas[i].can_control = false;
        }
        timer.Init(10);
        StartCoroutine(StartBattleEffect());
    }

    void Update()
    {
        p_obj[0].transform.position = new(player[0].transform.position.x, player[0].transform.position.y + 2.0f);
        p_obj[1].transform.position = new(player[1].transform.position.x, player[1].transform.position.y + 2.0f); timeLeft = updateInterval;

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (timeLeft <= 0.0f)
        {
            float fps = accum / frames;
            fpsText.text = string.Format("{0:F2} FPS", fps); timeLeft = updateInterval;
            accum = 0.0f; frames = 0;
        }
        if (battleCamera && !isdeath)
        {
            UpdateBattleCamera();
        }

        if (is_suddendeath)
        {
            if (++suddendeath_timer_current > suddendeath_timer_limit)
            {
                for (int i = 0; i < PLAYER_CNT; i++)
                {
                    datas[i].Damage(10, i == 0 ? 1 : 0);
                    suddendeath_timer_current = 0;
                }
            }
            
        }
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
        battleCamera = false;

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

        audioSource.PlayOneShot(se1);

        // エフェクト生成
        Instantiate(
            deathEffect,
            loser.transform.position,
            Quaternion.identity);

        // プレイヤーを消す
        loser.transform.position = new Vector3(1000, 1000, 0);
        // または
        // loser.SetActive(false);

        // KO表示
        yield return ShowKO();

        yield return new WaitForSeconds(4.5f);

        // シーン切り替え
        SceneManager.LoadScene("ResultScene");
    }

    IEnumerator ShowKO()
    {
        koText.gameObject.SetActive(true);

        Color c = koText.color;
        c.a = 0;
        koText.color = c;

        koText.transform.localScale = Vector3.one * 4f;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float p = t / 0.2f;

            koText.transform.localScale =
                Vector3.Lerp(Vector3.one * 4f, Vector3.one, p);

            c.a = p;
            koText.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;

            c.a = 1 - t / 0.3f;
            koText.color = c;

            yield return null;
        }

        koText.gameObject.SetActive(false);
    }
    Vector2 SetDirect(DIRECT direct)
    {
        if (direct == DIRECT.RIGHT) return new(1.0f, 0.0f);
        if (direct == DIRECT.LEFT) return new(-1.0f, 0.0f);
        if (direct == DIRECT.UP) return new(0.0f, 1.0f);
        if (direct == DIRECT.DOWN) return new(0.0f, -1.0f);
        return Vector2.zero;
    }

    IEnumerator StartBattleEffect()
    {
        Camera cam = Camera.main;

        Vector3 originalPos = defaultCameraPos;
        float originalSize = defaultCameraSize;

        float zoomSize = 2.5f;

        // Player1
        yield return ZoomToPlayer(
            player[0].transform.position,
            zoomSize,
            0.5f);

        yield return new WaitForSeconds(0.4f);

        // Player2
        yield return ZoomToPlayer(
            player[1].transform.position,
            zoomSize,
            0.5f);

        yield return new WaitForSeconds(0.4f);

        yield return MoveCamera(
       defaultCameraPos,
       defaultCameraSize,
       0.6f);

        yield return ShowReady();

        yield return ShowGo();

        // 操作開始
        for (int i = 0; i < PLAYER_CNT; i++)
        {
            datas[i].can_control = true;
        }

        // ダイナミックカメラ開始
        battleCamera = true;
        timer.TimerStart();
    }
    IEnumerator ZoomToPlayer(Vector3 pos, float size, float time)
    {
        Camera cam = Camera.main;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        Vector3 target =
            new Vector3(
                pos.x,
                pos.y + 0.5f,
                startPos.z);

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;

            cam.transform.position =
                Vector3.Lerp(
                    startPos,
                    target,
                    t / time);

            cam.orthographicSize =
                Mathf.Lerp(
                    startSize,
                    size,
                    t / time);

            yield return null;
        }
    }
    IEnumerator MoveCamera(Vector3 pos, float size, float time)
    {
        Camera cam = Camera.main;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;

            cam.transform.position =
                Vector3.Lerp(
                    startPos,
                    pos,
                    t / time);

            cam.orthographicSize =
                Mathf.Lerp(
                    startSize,
                    size,
                    t / time);

            yield return null;
        }
    }

    IEnumerator ShowReady()
    {
        readyText.gameObject.SetActive(true);

        Color c = Color.white;
        c.a = 1;
        readyText.color = c;

        readyText.transform.localScale = Vector3.one * 2f;

        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            float p = t / 0.5f;

            readyText.transform.localScale =
                Vector3.Lerp(Vector3.one * 2f, Vector3.one, p);

            c.a = p;
            readyText.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(0.7f);

        t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;

            c.a = 1 - t / 0.3f;
            readyText.color = c;

            yield return null;
        }

        readyText.gameObject.SetActive(false);
    }

    IEnumerator ShowGo()
    {
        goText.gameObject.SetActive(true);

        Color c = goText.color;
        c.a = 0;
        goText.color = c;

        goText.transform.localScale = Vector3.one * 3f;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float p = t / 0.2f;

            goText.transform.localScale =
                Vector3.Lerp(Vector3.one * 3f, Vector3.one, p);

            c.a = p;
            goText.color = c;

            yield return null;
        }
        audioSource.PlayOneShot(start_se);

        yield return new WaitForSeconds(0.5f);

        t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            c.a = 1 - t / 0.2f;
            goText.color = c;

            yield return null;
        }

        goText.gameObject.SetActive(false);
    }
    void UpdateBattleCamera()
    {
        Camera cam = Camera.main;

        Vector3 p1 = player[0].transform.position;
        Vector3 p2 = player[1].transform.position;

        //=========================
        // カメラ位置
        //=========================

        float centerX = (p1.x + p2.x) * 0.5f;

        // 縦は中間を少しだけ追う
        float centerY = (p1.y + p2.y) * 0.5f + verticalOffset;

        centerY = Mathf.Clamp(
            centerY,
            bottomLimit,
            topLimit);

        Vector3 targetPos = new Vector3(
            centerX,
            centerY,
            cam.transform.position.z);

        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            followSpeed * Time.deltaTime);

        //=========================
        // ズーム
        //=========================

        float width = Mathf.Abs(p1.x - p2.x);
        float height = Mathf.Abs(p1.y - p2.y);

        float sizeX = width * 0.5f + horizontalMargin;
        float sizeY = height * 0.8f + 2f;

        float targetSize = Mathf.Max(sizeX, sizeY);

        targetSize = Mathf.Clamp(
            targetSize,
            minZoom,
            maxZoom);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            followSpeed * Time.deltaTime);
    }

    void SuddenDeath()
    {
        is_suddendeath = true;
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