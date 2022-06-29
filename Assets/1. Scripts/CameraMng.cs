using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : MonoBehaviour
{
    [Header("제한 범위")]
    public Vector2 center;
    public Vector2 size;

    [Header("카메라 이동 속도")]
    public float speed;

    Vector2 sizeC;

    public Transform player;


    void Start()
    {
        sizeC.y = Camera.main.orthographicSize;
        sizeC.x = Screen.width * sizeC.y / Screen.height;
    }

    void LateUpdate()
    {
        if (player) 
        {
            transform.position = Vector3.Lerp(transform.position, player.position, speed * Time.deltaTime);

            float offsetY = size.y * 0.5f - sizeC.y;
            float offsetX = size.x * 0.5f - sizeC.x;

            float clampY = Mathf.Clamp(transform.position.y, center.y - offsetY, center.y + offsetY);
            float clampX = Mathf.Clamp(transform.position.x, center.x - offsetX, center.x + offsetX);

            transform.position = new Vector3(clampX, clampY, -10);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
