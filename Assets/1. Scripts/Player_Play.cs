using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;

public class Player_Play : MonoBehaviour
{
    Rigidbody2D rigd;
    SpriteRenderer sr;
    PhotonView pv;

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
        pv = GetComponent<PhotonView>();

        // 닉네임 출력
        nickTxt.text = pv.Owner.NickName;

        // 카메라 가져오기
        if (pv.IsMine)
        {
            Camera.main.GetComponent<CameraMng>().player = transform;
        }
    }

    void Update()
    {
        if (pv.IsMine)
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
                    pv.RPC("JumpFX", RpcTarget.AllBuffered);
                }
            }

            float h = Input.GetAxisRaw("Horizontal");
            rigd.velocity = new Vector2(h * 5, rigd.velocity.y);


            // 좌우 반전 전달
            if (h != 0)
                pv.RPC("SpriteFlipX", RpcTarget.AllBufferedViaServer, h);


            // 복제 기능 전달
            if (Input.GetMouseButtonDown(0))
                pv.RPC("Fire", RpcTarget.All, transform.position, sr.flipX);    
        }
    }

    #region [RPC 함수]
    [PunRPC]
    IEnumerator JumpFX()
    {
        GameObject _jupmFX = Instantiate(jumpFX, transform.position - transform.up * 0.5f, Quaternion.identity);
        yield return new WaitForSeconds(0.417f);
        Destroy(_jupmFX);
    }

    [PunRPC]
    void SpriteFlipX(float h)
    {
        if (h > 0)
            sr.flipX = false;
        else if (h < 0)
            sr.flipX = true;
    }

    [PunRPC]
    void Fire(Vector3 position, bool isFlipX)
    {
        GameObject _clone = Instantiate(clone, transform.position, Quaternion.identity);
        _clone.GetComponent<SpriteRenderer>().flipX = isFlipX;
        Destroy(_clone, 5f);
    }
    #endregion
}
