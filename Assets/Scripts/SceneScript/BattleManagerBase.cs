using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BattleManagerBase : MonoBehaviour
{
    // ----- 定数 ----- //
    const int PLAYER_CNT = 2;

    // ----- 変数 ----- //
    [Header("◇キャラクター")]
    [SerializeField] protected List<Character> characters = new();
    protected GameObject[] player = new GameObject[PLAYER_CNT];
    protected CharBase[] datas = new CharBase[PLAYER_CNT];
    public Spawner[] spawn_point = new Spawner[PLAYER_CNT];

    [Header("◇開始時演出")]
    [SerializeField] protected Text readyText;
    [SerializeField] protected Text goText;
    protected Vector3 defaultCameraPos;
    protected float defaultCameraSize;

    [Header("◇サウンド")]
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip start_se;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //カメラ取得
        Camera cam = Camera.main;
        defaultCameraPos = cam.transform.position;
        defaultCameraSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {

    }

    virtual protected IEnumerator StartBattleEffect()
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
    }
    protected IEnumerator ZoomToPlayer(Vector3 pos, float size, float time)
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
    protected IEnumerator MoveCamera(Vector3 pos, float size, float time)
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

    protected IEnumerator ShowReady()
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

    protected IEnumerator ShowGo()
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
        datas[0].can_control = true;
    }
}
