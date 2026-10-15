using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurstBar : MonoBehaviour
{
    CharBase targetPlayer;
    Image image;
    [SerializeField] List<GageColor> gageColor = new();

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        ;
    }

    public void Init(GameObject player_obj)
    {
        CharBase player = player_obj.GetComponent<CharBase>();
        targetPlayer = player;
    }

    public void Draw(int burst, int max)
    {
        // Š„‡‚ğ‘ã“ü
        var ratio = 1.0f - (targetPlayer.burst / (float)max);
        image.fillAmount = ratio;
        Debug.Log(ratio);

        // F‚ğ•ÏX
        foreach (var color in gageColor)
        {
            if (ratio <= color.Ratio)
            {
                image.color = color.Color;
                break;
            }
        }
    }

    [Serializable]
    class GageColor
    {
        public float Ratio => ratio;
        public Color Color => color;

        [SerializeField] float ratio;
        [SerializeField] Color color;
    }
}