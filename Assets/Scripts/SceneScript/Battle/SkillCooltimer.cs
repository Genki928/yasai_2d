using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SkillCooltimer : MonoBehaviour
{
    Image image;
    public CharBase character;
    [SerializeField] AudioClip CtSound;
    public bool OneShot=false;

    //オーディオソース用
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        image = GetComponent<Image>();
    }

    public void RefreshCooltimer(int cooltimer, int cooltime)
    {
        image.fillAmount = 1 - cooltimer / (float)cooltime;
        if (1 - cooltimer != 1) OneShot = true;
        else if(OneShot==true)
        {
            audioSource.PlayOneShot(CtSound);
            OneShot = false;
        } 
            
    }
}
