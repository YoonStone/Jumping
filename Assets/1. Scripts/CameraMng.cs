using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : MonoBehaviour
{
    [Header("제한 범위")]
    public Vector2 center;
    public Vector2 size;

    Vector2 centerC, sizeC;

    void Start()
    {
        sizeC.y = Camera.main.orthographicSize;
        sizeC.x = Screen.width * sizeC.y / Screen.height;
    }

    void Update()
    {
         
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
