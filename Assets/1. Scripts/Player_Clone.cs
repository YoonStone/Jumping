using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Clone : MonoBehaviour
{
    [HideInInspector]
    public Color originColor;

    [HideInInspector]
    public bool flipX;

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        originColor.a = 0.5f;
        sr.color = originColor;

        sr.flipX = flipX;
    }

    public void Active()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        originColor.a = 1f;
        sr.color = originColor;
    }
}
