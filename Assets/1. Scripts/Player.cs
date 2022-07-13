using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody2D rigd;
    SpriteRenderer sr;

    public TextMeshProUGUI nickTxt;
    public GameObject clone;

    [Header("점프 관련")]
    public GameObject jumpFX;
    public LayerMask canJump;

    Transform cam;

    void Start()
    {
        rigd = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        Camera.main.GetComponent<CameraMng>().player = transform;
    }

    void Update()
    {
        // 점프 + 이동
        if (Input.GetButtonDown("Jump"))
        {
            Vector3 rayStart
                = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.6f, transform.position.x);
            Debug.DrawRay(rayStart, transform.right, Color.red);

            if (Physics2D.Raycast(rayStart, transform.right, 1, canJump))
            {
                rigd.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
                StartCoroutine(JumpFX());
            }
        }

        float h = Input.GetAxisRaw("Horizontal");
        rigd.velocity = new Vector2(h * 5, rigd.velocity.y);


        // 좌우 반전
        if (h > 0)
            sr.flipX = false;
        else if (h < 0)
            sr.flipX = true;

        // 복제 기능 전달
        if (Input.GetMouseButtonDown(0))
        {
            GameObject _clone = Instantiate(clone, transform.position, Quaternion.identity);
            _clone.GetComponent<SpriteRenderer>().flipX = sr.flipX;
            Destroy(_clone, 5f);
        }
    }

    IEnumerator JumpFX()
    {
        GameObject _jupmFX = Instantiate(jumpFX, transform.position - transform.up * 0.5f, Quaternion.identity);
        yield return new WaitForSeconds(0.417f);
        Destroy(_jupmFX);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<BoxCollider2D>().isTrigger = false;
            collision.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
