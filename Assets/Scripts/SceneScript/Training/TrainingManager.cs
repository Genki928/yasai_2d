//だいぶ思い付きで作ったからバグったらスマン

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    // シーン再読み込み後も選択キャラを保持する
    static int currentIndex = -1;

    [Header("◇キャラ生成")]
    [SerializeField] Character[] characters;
    [SerializeField] Spawner playerSpawn;
    [Tooltip("サンドバッグなど。空なら一人だけで配置")]
    [SerializeField] GameObject dummyPrefab;
    [SerializeField] Spawner dummySpawn;

    [Header("◇GUI")]
    [SerializeField] GUI gui;
    [SerializeField] GameObject playerMarkerPrefab; // 「P1」表示
    GameObject marker;

    [Header("◇Scene")]
    [Tooltip("スタートボタンで戻るシーン名(Build Settings に登録が必要)")]
    [SerializeField] string titleSceneName = "TitleScene";

    [Header("◇Audio")]
    [SerializeField] AudioSource audioSourceBgm;
    [SerializeField] AudioClip defaultBgm;

    [Header("◇Battle Camera")]
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float horizontalMargin = 2.0f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 9f;
    [SerializeField] float verticalOffset = 1.5f;
    [SerializeField] float bottomLimit = -1f;
    [SerializeField] float topLimit = 6f;
    [SerializeField] float leftLimit = -10f;
    [SerializeField] float rightLimit = 10f;

    GameObject player;
    CharBase playerData;
    GameObject dummy;

    bool switching = false;
    bool resetting = false;

    void Awake()
    {
        Winner.Reset();
        Application.targetFrameRate = 60;
        CharBase.OnPlayerDies += OnPlayerDies;
    }

    void Start()
    {
        if (currentIndex < 0)
        {
            try { currentIndex = (int)PlayerPick.pick[0]; }
            catch (System.Exception) { currentIndex = 0; }
        }
        currentIndex = Mathf.Clamp(currentIndex, 0, characters.Length - 1);

        if (gui.skillCooltimer == null || gui.skillCooltimer.Length < 3)
        { 
            enabled = false;
            return;
        }

        if (!IsValidChar(currentIndex))
        {
            enabled = false;
            return;
        }

        if (playerMarkerPrefab != null)
            marker = Instantiate(playerMarkerPrefab);

        SpawnPlayer(currentIndex, playerSpawn.point.transform.position, SetDirect(playerSpawn.direct));
        SpawnDummy();

        if (audioSourceBgm != null && defaultBgm != null)
        {
            audioSourceBgm.clip = defaultBgm;
            audioSourceBgm.Play();
        }
    }

    void OnDestroy()
    {
        CharBase.OnPlayerDies -= OnPlayerDies;
    }

    void Update()
    {
        var pad = Gamepad.all.Count > 0 ? Gamepad.all[0] : null;

        // L+R同時押しでリセット
        if (pad != null
            && pad.leftShoulder.isPressed && pad.rightShoulder.isPressed
            && (pad.leftShoulder.wasPressedThisFrame || pad.rightShoulder.wasPressedThisFrame))
        {
            ResetScene();
            return;
        }

        // スタートボタンでタイトル
        if (pad != null && !resetting && pad.startButton.wasPressedThisFrame)
        {
            resetting = true;
            SceneManager.LoadScene(titleSceneName);
            return;
        }

        // 十字キー左右でキャラ切り替え
        if (!switching && !resetting && pad != null)
        {
            if (pad.dpad.left.wasPressedThisFrame) StartCoroutine(SwitchCharacter(-1));
            else if (pad.dpad.right.wasPressedThisFrame) StartCoroutine(SwitchCharacter(1));
        }

        if (player == null) return;

        if (marker != null)
            marker.transform.position = player.transform.position + Vector3.up * 2.0f;

        UpdateCamera();
    }

    void SpawnPlayer(int index, Vector3 pos, Vector2 direction)
    {
        PlayerInput pi;

        if (Gamepad.all.Count > 0)
        {
            pi = PlayerInput.Instantiate(
                characters[index].chars,
                playerIndex: 0,
                controlScheme: "Controller2",
                pairWithDevice: Gamepad.all[0]);
        }
        else
        {
            Debug.LogWarning("ゲームパッド、ナシ");
            pi = PlayerInput.Instantiate(characters[index].chars, playerIndex: 0);
        }

        pi.transform.position = pos;
        pi.transform.rotation = Quaternion.identity;

        player = pi.gameObject;
        playerData = player.GetComponent<CharBase>();
        playerData.id = 0;
        playerData.direction = direction;
        playerData.can_control = true;

        // UIとの紐づけ
        gui.bar.Init(player);
        playerData.burst_bar = gui.bar;
        playerData.cooltimeUI = gui.skillCooltimer;
        playerData.InitCooltime();
        gui.name.text = playerData.data.char_name;
        gui.icon.sprite = characters[index].icon;
    }


    bool IsValidChar(int index)
    {
        var prefab = characters[index].chars;
        if (prefab == null)
        {
            Debug.LogError($"Characters[{index}]", this);
            return false;
        }
        if (!prefab.TryGetComponent<PlayerInput>(out _))
        {
            Debug.LogError($"Characters[{index}] '{prefab.name}'", prefab);
            return false;
        }
        return true;
    }

    void SpawnDummy()
    {
        if (dummyPrefab == null) return;

        dummy = Instantiate(dummyPrefab, dummySpawn.point.transform.position, Quaternion.identity);
        if (dummy.TryGetComponent<CharBase>(out var d))
        {
            d.id = 1;
            d.direction = SetDirect(dummySpawn.direct);
        }
    }

    IEnumerator SwitchCharacter(int dir)
    {
        if (characters.Length <= 1) yield break;
        switching = true;

        // PlayerInput のあるキャラまでスキップして次を探す
        int next = currentIndex;
        bool found = false;
        for (int i = 0; i < characters.Length; i++)
        {
            next = (next + dir + characters.Length) % characters.Length;
            if (IsValidChar(next)) { found = true; break; }
        }
        if (!found || next == currentIndex)
        {
            switching = false;
            yield break;
        }
        currentIndex = next;

        // 位置と向きを引き継ぐ
        Vector3 pos = player.transform.position;
        Vector2 direction = playerData.direction;

        Destroy(player);
        yield return null;

        SpawnPlayer(currentIndex, pos, direction);
        switching = false;
    }

    void ResetScene()
    {
        if (resetting) return;
        resetting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // プレイヤーが倒れたら少し待ってリセット
    void OnPlayerDies(int winnerId)
    {
        if (winnerId == 0 || resetting) return;
        StartCoroutine(ResetAfterDelay(1.5f));
    }

    IEnumerator ResetAfterDelay(float sec)
    {
        yield return new WaitForSeconds(sec);
        ResetScene();
    }

    void UpdateCamera()
    {
        Camera cam = Camera.main;

        Vector3 p1 = player.transform.position;
        Vector3 p2 = dummy != null ? dummy.transform.position : p1;

        float centerX = Mathf.Clamp((p1.x + p2.x) * 0.5f, leftLimit, rightLimit);
        float centerY = Mathf.Clamp((p1.y + p2.y) * 0.5f + verticalOffset, bottomLimit, topLimit);

        Vector3 targetPos = new(centerX, centerY, cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, followSpeed * Time.deltaTime);

        float sizeX = Mathf.Abs(p1.x - p2.x) * 0.5f + horizontalMargin;
        float sizeY = Mathf.Abs(p1.y - p2.y) * 0.8f + 2f;
        float targetSize = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minZoom, maxZoom);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, followSpeed * Time.deltaTime);
    }

    Vector2 SetDirect(DIRECT direct)
    {
        return direct switch
        {
            DIRECT.RIGHT => Vector2.right,
            DIRECT.LEFT => Vector2.left,
            DIRECT.UP => Vector2.up,
            DIRECT.DOWN => Vector2.down,
            _ => Vector2.zero
        };
    }
}