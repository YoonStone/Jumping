using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;

public class Player_Wait : MonoBehaviour
{
    Rigidbody2D rigd;
    SpriteRenderer sr;
    PhotonView pv;

    public TextMeshProUGUI nickTxt;

    [Header("점프 관련")]
    public GameObject jumpFX;
    public LayerMask canJump;

    public Color[] colors;

    public Button[] colorBtns;

    int curBtn;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        colorBtns = FindObjectOfType<WaitRoomMng>().colorBtns;

        for (int i = 0; i < colorBtns.Length; i++)
        {
            int _i = i;
            colorBtns[_i].onClick.AddListener(() => ClickBtn(_i));
        }
    }

    void Start()
    {
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
            if(h != 0)
                pv.RPC("SpriteFlipX", RpcTarget.AllBufferedViaServer, h);
        }
    }

    void ClickBtn(int num)
    {
        if (pv.IsMine)
        {
            pv.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "color", num } });
            pv.RPC("ClickColorBtn", RpcTarget.AllBufferedViaServer, curBtn, num);

            curBtn = num;
        }
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
    IEnumerator JumpFX()
    {
        GameObject _jupmFX = Instantiate(jumpFX, transform.position - transform.up * 0.5f, Quaternion.identity);
        yield return new WaitForSeconds(0.417f);
        Destroy(_jupmFX);
    }

    [PunRPC]
    void ClickColorBtn(int cur, int num)
    {
        Debug.LogError($"번호 : {num}\n버튼개수 : {colorBtns.Length}\n색상개수 : {colors.Length}");
        
        colorBtns[cur].interactable = true; // 이전 버튼 활성화
        colorBtns[num].interactable = false;
        sr.color = colors[num];
    }
}
