using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> chars = new();
    public GameObject[] spawn_point = new GameObject[2];
    GameObject[] player = new GameObject[2];
    CharBase[] datas = new CharBase[2];
    int[] pick_nums = { 1, 0 };

    [Header("◇GUI")]
    public GUI[] gui = new GUI[2];

    //Px用
    public GameObject[] player_obj = new GameObject[2];
    GameObject[] p_obj = new GameObject[2];

    void Awake()
    {
        Application.targetFrameRate = 30;
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            // キャラクターを生成
            player[i] = Instantiate(chars[pick_nums[i]], spawn_point[i].transform.position, Quaternion.identity);
            datas[i] = player[i].GetComponent<CharBase>();
            datas[i].id = i;

            // ポインター
            p_obj[i]=Instantiate(player_obj[i]);

            // GUI
            gui[i].bar.Init(player[i]);
            if(player[i].TryGetComponent<CharBase>(out var p))
            {
                gui[i].name.text = p.data.char_name;
                p.cooltimer[0] = gui[i].skill1_cooltimer;
                p.cooltimer[1] = gui[i].skill2_cooltimer;
                p.burst_bar = gui[i].bar;
                gui[i].name.text = p.data.char_name;
            }
        }

        // GUI
        gui[0].bar.Init(player[0]);
        gui[1].bar.Init(player[1]);
    }

    void Update()
    {
        // ポインター
        p_obj[0].transform.position = new(player[0].transform.position.x, player[0].transform.position.y+2.0f);
        p_obj[1].transform.position = new(player[1].transform.position.x, player[1].transform.position.y+2.0f);

    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");
    }
}

[Serializable]
public class GUI
{
    public BurstBar bar;
    public Text name;
    public SkillCooltimer skill1_cooltimer;
    public SkillCooltimer skill2_cooltimer;
}