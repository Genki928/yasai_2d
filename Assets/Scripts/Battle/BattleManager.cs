using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> list = new List<GameObject>();
    GameObject[] chars;
    int[] pick_nums = { 0, 0 };

    void Awake()
    {
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        for (int i = 0; i < 2; i++) {
            chars[i] = list[pick_nums[i]];
        }
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");
    }
}