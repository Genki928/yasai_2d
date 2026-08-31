using System;
using UnityEngine;

public class Timer
{
    // ----- プロパティ
    public float TImeLimit => _currentTime;

    // ----- 変数
    float _currentTime = 0.0f;
    bool _countdown = true;
    bool _stop = true;

    // イベント
    public event Action OnTimeUp;
    public event Action<float> OnCount;

    public Timer(float startTime, bool countdown, bool stop = true)
    {
        _currentTime = startTime;
        _countdown = countdown;
        _stop = stop;
    }

    public void Count(float deltaTime)
    {
        // 停止中なら中断
        if (_stop) return;

        // タイマーを進める
        if (_countdown) _currentTime = Mathf.Max(0.0f, _currentTime - deltaTime);
        else _currentTime += deltaTime;
        OnCount?.Invoke(_currentTime);

        // カウントアップなら中断
        if (!_countdown) return;

        // タイマーが0.0fになったらイベント起動
        if (_currentTime == 0.0f) OnTimeUp?.Invoke();
    }

    public void Switch(bool swtich)
    {
        _stop = !swtich;
    }
}