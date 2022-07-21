﻿using System.Collections;
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
    public PhotonView pv;

    public TextMeshProUGUI nickTxt;
    public GameObject clone;

    [Header("점프 관련")]
    public GameObject jumpFX;
    public LayerMask canJump;

    public Color[] colors;
   
    bool isEsc;
    Button escBtn, backBtn;
    PlayManager pm;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        pm = FindObjectOfType<PlayManager>();
        escBtn = pm.escBtn;
        backBtn = pm.backBtn;
        escBtn.onClick.AddListener(ClickESC);
        backBtn.onClick.AddListener(ClickESC);
    }

    void Start()
    {
        // 닉네임 출력
        nickTxt.text = pv.Owner.NickName;

        // 색상 적용
        ExitGames.Client.Photon.Hashtable color = pv.Owner.CustomProperties;
        sr.color = colors[(int)color["color"]];

        if (pv.IsMine)
        {
            // 카메라 가져오기
            Camera.main.GetComponent<CameraMng>().player = transform;
        }
    }

    void Update()
    {
        if (!pv.IsMine)
            return;

        if (pm.isCanMove)
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
            {
                Player_Clone _clone = PhotonNetwork.Instantiate("Player_Clone", transform.position, Quaternion.identity).GetComponent<Player_Clone>();
                int viewID = _clone.GetComponent<PhotonView>().ViewID;
                pv.RPC("SetClone", RpcTarget.All, viewID);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !isEsc)
            {
                Vector3 curSize = escBtn.transform.localScale;

                if (escBtn.gameObject.activeSelf)
                {
                    StartCoroutine(CloseESC(curSize));
                }
                else
                {
                    StartCoroutine(OpenESC(curSize));
                }
            }
        }
        else if(pm.winner.activeSelf && PhotonNetwork.IsMasterClient)
        {
            pm.backBtn.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pv.IsMine && collision.CompareTag("Player"))
        {
            collision.GetComponent<Player_Clone>().Active();
        }
    }

    void ClickESC()
    {
        if (pv.IsMine)
            PhotonNetwork.LeaveRoom();
    }

    IEnumerator OpenESC(Vector3 curSize)
    {
        isEsc = true;
        escBtn.gameObject.SetActive(true);
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            escBtn.transform.localScale = Vector3.Lerp(curSize, Vector3.one, timer);
            yield return null;
        }
        isEsc = false;
    }

    IEnumerator CloseESC(Vector3 curSize)
    {
        isEsc = true;
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            escBtn.transform.localScale = Vector3.Lerp(curSize, Vector3.zero, timer);
            yield return null;
        }
        escBtn.gameObject.SetActive(false);
        isEsc = false;
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
    void SetClone(int viewID) // 클론 초기설정
    {
        Player_Clone _clone = PhotonView.Find(viewID).gameObject.GetComponent<Player_Clone>();
        _clone.flipX = sr.flipX;
        _clone.originColor = sr.color;
        _clone.originID = pv.ViewID;
    }

    [PunRPC]
    void ActiveClone(int viewID)
    {
        print("클론활성화_플레이어");
    }
    #endregion
}
