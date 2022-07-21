using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;
using ExitGames.Client.Photon;

public class Player_Wait : MonoBehaviourPunCallbacks
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
    bool isEsc;

    WaitRoomMng waitRoomMng;
    GameObject startBtn; // 다음 방장이 될 수도 있으므로
    Button escBtn;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        waitRoomMng = FindObjectOfType<WaitRoomMng>();
        colorBtns = waitRoomMng.colorBtns;
        escBtn = waitRoomMng.escBtn;

        for (int i = 0; i < colorBtns.Length; i++)
        {
            int _i = i;
            colorBtns[_i].onClick.AddListener(() => ClickBtn(_i));
        }
        escBtn.onClick.AddListener(ClickESC);
    }

    void Start()
    {
        // 닉네임 출력
        nickTxt.text = pv.Owner.NickName;

        // 방장이 아니면 버튼 안 보이게
        startBtn = GameObject.Find("StartBtn");
        if (!PhotonNetwork.IsMasterClient && startBtn)
            startBtn.SetActive(false);

        if (pv.IsMine)
        {
            // 원래 저장되어있던 색이 있다면 = 대기실로 돌아온 것
            ExitGames.Client.Photon.Hashtable color = pv.Owner.CustomProperties;
            if (color["color"] != null)
            {
                sr.color = colors[(int)color["color"]];
                pv.RPC("ClickColorBtn", RpcTarget.AllBuffered, true, curBtn, (int)color["color"]);
            }
            // 원래 저장되어있던 색이 없다면 = 새로 생긴 대기실
            else
            {
                if (PhotonNetwork.IsMasterClient)
                    FirstColor();
                else // 방장이 아니면 딜레이
                    Invoke("FirstColor", 0.1f);
            }
        }
    }

    // 랜덤으로 시작 색상 고르기
    void FirstColor()
    {
        print("처음임");
        for (int i = 0; i < colorBtns.Length; i++)
        {
            if (colorBtns[i].interactable)
            {
                pv.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "color", i } });
                pv.RPC("ClickColorBtn", RpcTarget.AllBuffered, true, curBtn, i);
                curBtn = i;
                break;
            }
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
            if(h != 0)
                pv.RPC("SpriteFlipX", RpcTarget.AllBufferedViaServer, h);
          
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
    }

    void ClickBtn(int num)
    {
        if (pv.IsMine)
        {
            pv.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "color", num } });
            pv.RPC("ClickColorBtn", RpcTarget.AllBuffered, false, curBtn, num);

            curBtn = num;
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
    void ClickColorBtn(bool isFirst, int cur, int num)
    {
        if(!isFirst)
            colorBtns[cur].interactable = true; // 이전 버튼 활성화
       
        colorBtns[num].interactable = false;
        sr.color = colors[num];
    }
    #endregion

    #region [포톤 콜백 함수]
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // 내가 다음 차례 방장이라면 버튼 on
        if (pv.IsMine && PhotonNetwork.IsMasterClient)
            startBtn.SetActive(true);
    }
    #endregion
}
