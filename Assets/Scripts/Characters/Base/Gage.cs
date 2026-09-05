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
    SkillCooltimer _cooltimeUI;

    // ----- イベント
    public event Action OnCooltimeCharged;

    public Cooltime(SkillCooltimer cooltimeUI, float max, float value = 0.0f)
    {
        // 初期値を代入
        _cooltimeUI = cooltimeUI;
        _currentCooltime = value;
        _maxCooltime = max;
    }

    /// <summary> クールタイムを減少させる </summary>
    public Cooltime RemoveCooltime(float deltaTime)
    {
        CooltimeChange(_currentCooltime - deltaTime);

        return this;
    }

    /// <summary> クールタイムを設定する </summary>
    public void SetCooltime()
    {
        _currentCooltime = _maxCooltime;
    }
    
    /// <summary> クールタイムを一定割合回復する </summary>
    /// <param name="par"> 0.0f <= n <= 1.0f </param>
    public void RefleshCooltime(float par)
    {
        CooltimeChange(_currentCooltime - (_maxCooltime * par));
    }

    void CooltimeChange(float remove)
    {
        // 減少処理、減少しきったらイベント起動
        _currentCooltime = MathF.Max(0.0f, remove);
        _cooltimeUI.RefreshCooltimer(this);
        if (_currentCooltime == 0.0f) OnCooltimeCharged?.Invoke();
    }
}