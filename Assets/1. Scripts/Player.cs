using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class Player : MonoBehaviour, IPunObservable
{
    Rigidbody2D rigd;
    PhotonView pv;

    public Text nickTxt;
    public Slider hpBar;
    public GameObject bullet;

    public int hp = 100;

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


            // 총 쏘는 기능 전달
            if (Input.GetMouseButtonDown(0))
                pv.RPC("Fire", RpcTarget.All, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    void Fire(int acotrNum)
    {
        GameObject _bullet = Instantiate(bullet, transform.position + transform.right, Quaternion.identity);
        _bullet.GetComponent<Bullet>().acotrNum = acotrNum;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            int acotrNum = collision.gameObject.GetComponent<Bullet>().acotrNum;

            // 다른 사람의 총에 맞으면 피 닳기
            if (pv.Owner.ActorNumber != acotrNum)
            {
                hp -= 10;
                hpBar.value = hp / 100f;

                // 죽으면 쏜 사람 스코어 업 + 리셋
                if(hp <= 0)
                {
                    PlayManager.instance.ScoreChange(acotrNum);
                    gameObject.SetActive(false);
                    pv.RPC("Respawn", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.5f);
        transform.position = Vector3.zero;
        hp = 100;
        hpBar.value = hp / 100f;
        gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(hpBar.value);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
            hpBar.value = (float)stream.ReceiveNext();
        }
    }
}
