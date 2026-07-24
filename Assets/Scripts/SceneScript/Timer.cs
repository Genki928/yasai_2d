using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    int timer = 0;
    int limit = 60;
    bool is_start = false;
    public Text text;
    public event Action OnFinish;

    void Update()
    {
        if (is_start)
        {
            if (limit > 0)
            {
                if (--timer < 0)
                {
                    timer = 60;
                    --limit;
                    Draw();
                }
            }
            else
            {
                OnFinish?.Invoke();
            }
        }
    }

    public void Init(int start_time)
    {
        limit = start_time;
        Draw();
    }

    void Draw()
    {
        text.text = $"{limit}";
    }

    public void TimerStop()
    {
        is_start = false;
    }

    public void TimerStart()
    {
        is_start = true;
    }
}
