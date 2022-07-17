using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Clone : MonoBehaviour
{
    [HideInInspector]
    public Color originColor;

    [HideInInspector]
    public bool flipX;

    [HideInInspector]
    public int originID;

    SpriteRenderer sr;
    PhotonView pv;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        originColor.a = 0.5f;
        sr.color = originColor;
        sr.flipX = flipX;

        Destroy(gameObject, 5f);
    }

    public void Active()
    {
        pv.RPC("ActiveClone", RpcTarget.All);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        // ** 내꺼랑 부딪혀도 없어짐 **
        if(other.collider.GetComponent<PhotonView>().ViewID != originID)
            pv.RPC("DestoryClone", RpcTarget.All);
    }

#region [RPC 함수]
    [PunRPC]
    void ActiveClone() // 클론 활성화
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        originColor.a = 1f;
        sr.color = originColor;
    }

    [PunRPC]
    void SetClone()
    {
        print("클론생성");
    }

    [PunRPC]
    void DestoryClone()
    {
        Destroy(gameObject);
    }
#endregion
}
