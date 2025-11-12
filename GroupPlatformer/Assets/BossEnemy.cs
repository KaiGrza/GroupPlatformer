using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public int health = 3;
    public Animator am;
    int animatorState;
    public SpriteRenderer sr;
    int sheildDir = 0;
    float hte = 0f;
    public Collider2D col;
    public void hit(Vector2 dir)
    {

        if (animatorState == 4) return;
        if (dir.normalized.x > 0.5f && sheildDir == 1) return;
        if (dir.normalized.y < -0.5f && sheildDir == 2) return;
        if (dir.normalized.x < -0.5f && sheildDir == 3) return;
        health--;
        animatorState = health<=0?4:3;
        if(animatorState==4)
        {
            col.enabled = false;
        }
        hte=Time.time;
    }
    int lastAmstate = 0;
    private void Update()
    {
        Vector2 dir = (Vector2)(transform.position - PlayerMovement.main.transform.position);
        switch (health)
        {
            case 3: sheildDir = dir.normalized.y<-.5f?2:0; break;
            case 2: sheildDir = dir.normalized.y < -.5f ? 2 : 1; break;
            case 1: sheildDir = dir.x < 0 ? 3 : 1; break;
            default: break;
        }
        if (animatorState <3 || Time.time-hte> (animatorState==3?0.167f:.833f))
        {
            if (animatorState == 4)
                gameObject.SetActive(false);
            animatorState = sheildDir==0?0:sheildDir==2?2:1;
        }
        if(lastAmstate!=animatorState)
        {
            am.SetInteger("State", animatorState);
            am.SetTrigger("Set");
            lastAmstate = animatorState;
        }
        sr.flipX = sheildDir == 3;
    }

}
