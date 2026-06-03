using UnityEngine;

public class BossBase : MonoBehaviour, burst
{

    [Header("◇キャラクターデータ")]
    public int burst = 0;
    public int id { get; } = 100;

    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void Damage(int value, int id)
    {
        Debug.Log("mi");
    }
}

public interface burst
{
    public int id { get; }
    public void Damage(int value, int id);
}