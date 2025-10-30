using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer sr;
    public float fps;
    private void Update()
    {
        sr.sprite = sprites[Mathf.FloorToInt(Time.time*fps)%sprites.Length];
    }
}
