using UnityEngine;
using UnityEngine.UI;

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

    public void RefreshCooltimer(Cooltime cooltime)
    {
        image.fillAmount = 1 - cooltime.Current / cooltime.Max;
        //if (1 - cooltimer != 1) OneShot = true;
        //else if(OneShot==true)
        //{
        //    audioSource.PlayOneShot(CtSound);
        //    OneShot = false;
        //} 
            
    }
}
