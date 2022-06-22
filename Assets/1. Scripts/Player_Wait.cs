using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class Player_Wait : MonoBehaviour
{
    Rigidbody2D rigd;
    PhotonView pv;

    public Text nickTxt;

    void Start()
    {
        rigd = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        // 닉네임 출력
        nickTxt.text = pv.Owner.NickName;

        // 방장이 아니면 버튼 안 보이게
        GameObject startBtn = GameObject.Find("StartBtn");
        if (!PhotonNetwork.IsMasterClient && startBtn)
            startBtn.SetActive(false);
    }

    void Update()
    {
        if (pv.IsMine)
        {
            // 점프 + 이동
            if (Input.GetButtonDown("Jump"))
                rigd.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

            float h = Input.GetAxisRaw("Horizontal");
            rigd.velocity = new Vector2(h * 5, rigd.velocity.y);
        }
    }
}
