using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultistageMusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;
    int prog=-1;
    public int[] loopThese;
    public void SetMusic(int ID)
    {
        prog = ID;
        if (prog >= clips.Length)return;
        source.clip = clips[prog];
        source.loop = false;
        foreach(int l in loopThese) if(l==prog)source.loop = true;
        source.Play();
    }
    private void Update()
    {
        if(!source.isPlaying&&prog<clips.Length)
        {
            SetMusic(prog+1);
        }
    }
}
