using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class RoomBtn : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ClickMe);
    }

    void ClickMe()
    {
        // 닉네임 할당
        PhotonNetwork.LocalPlayer.NickName 
            = GameObject.FindGameObjectWithTag("NickInput").GetComponent<InputField>().text;

        string roomName = GetComponentInChildren<Text>().text.Split('(')[0];
        PhotonNetwork.JoinRoom(roomName);

        // 씬에 있는 모든 버튼 비활성화
        FindObjectOfType<LobbyMng>().BtnInteract(false);
    }
}
