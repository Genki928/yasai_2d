using System;
using UnityEngine;

public class Cooltime
{
    // ----- プロパティ
    public float Current => _currentCooltime;
    public float Ratio => _currentCooltime / _maxCooltime;

    // ----- 変数
    float _currentCooltime = 0.0f;
    float _maxCooltime = 0.0f;

    // ----- イベント
    public event Action OnCooltimeCharged;

    public Cooltime(float max, float value = 0.0f)
    {
        // 初期値を代入
        _currentCooltime = value;
        _maxCooltime = max;
    }

    /// <summary> クールタイムを減少させる </summary>
    public Cooltime RemoveCooltime(float deltaTime)
    {
        // 減少処理、減少しきったらイベント起動
        _currentCooltime = Mathf.Max(0.0f, _currentCooltime - deltaTime);
        if (_currentCooltime == 0.0f) OnCooltimeCharged?.Invoke();
        
        return this;
    }

    /// <summary> クールタイムを設定する </summary>
    public void SetCooltime()
    {
        _currentCooltime = _maxCooltime;
    }
}