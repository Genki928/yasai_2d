using UnityEngine;

public class Cooltime
{
    // ----- プロパティ
    public float Current => _currentCooltime;
    public float Max => _maxCooltime;

    // ----- メンバ
    float _currentCooltime = 0.0f;
    float _maxCooltime = 0.0f;

    public Cooltime(float max, float value = 0.0f)
    {
        // 初期値を代入
        _currentCooltime = value;
        _maxCooltime = max;
    }

    /// <summary> クールタイムを減少させる </summary>
    public Cooltime RemoveCooltime(float deltaTime)
    {
        _currentCooltime = Mathf.Max(0.0f, _currentCooltime - deltaTime);
        return this;
    }

    /// <summary> クールタイムを設定する </summary>
    public void SetCooltime()
    {
        _currentCooltime = _maxCooltime;
    }
}