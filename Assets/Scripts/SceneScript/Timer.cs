using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    // ----- •Ï”
    Text _ui;
    Timer _timer;

    void Start() => _ui = GetComponent<Text>();

    void Draw(float time) => _ui.text = $"{time.ToString("N0")}";

    public void Init(Timer timer)
    {
        _timer = timer;
        _timer.OnCount += Draw;
    }

    void OnDestroy() => _timer.OnCount -= Draw;
}
