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
    [SerializeField] GameObject[] bars = new GameObject[2];
    [SerializeField] GameObject[] names = new GameObject[2];
    [SerializeField] GameObject[] l_gage = new GameObject[2];
    [SerializeField] GameObject[] r_gage = new GameObject[2];
    public GUI left_gui;
    public GUI right_gui;

    //Px用
    public GameObject[] player_obj = new GameObject[2];
    public GameObject[] p_obj = new GameObject[2];

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
            Vector2 pos = new(spawn_point[i].transform.position.x, spawn_point[i].transform.position.y);
            player[i] = Instantiate(chars[pick_nums[i]], pos, Quaternion.identity);
            datas[i] = player[i].GetComponent<CharBase>();
            datas[i].id = i;

            //
            p_obj[i]=Instantiate(player_obj[i]);

            // GUI
            if (bars[i].TryGetComponent<BurstBar>(out var bar)) bar.Init(player[i]);
            if (names[i].TryGetComponent<Text>(out var text)) text.text = player[i].GetComponent<CharBase>().data.char_name;
        }

        // GUI
        //left_gui.bar.Init(player[0]);
        //left_gui.skill1_cooltimer.character = player[0].GetComponent<CharBase>();
        //right_gui.bar.Init(player[1]);
        //left_gui.skill1_cooltimer.character = player[0].GetComponent<CharBase>();
    }

    void Update()
    {
        if (l_gage[0].TryGetComponent<Image>(out var rs1)) rs1.fillAmount = 1 - datas[0].skill_1_cooltime / (float)datas[0].data.skill_1_cooltime;
        if (l_gage[1].TryGetComponent<Image>(out var ls2)) ls2.fillAmount = 1 - datas[0].skill_2_cooltime / (float)datas[0].data.skill_2_cooltime;
        if (r_gage[0].TryGetComponent<Image>(out var s1)) s1.fillAmount = 1 - datas[1].skill_1_cooltime / (float)datas[1].data.skill_1_cooltime;
        if (r_gage[1].TryGetComponent<Image>(out var s2)) s2.fillAmount = 1 - datas[1].skill_2_cooltime / (float)datas[1].data.skill_2_cooltime;


        //
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