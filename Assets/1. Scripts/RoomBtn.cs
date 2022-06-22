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
        // �г��� �Ҵ�
        PhotonNetwork.LocalPlayer.NickName 
            = GameObject.FindGameObjectWithTag("NickInput").GetComponent<InputField>().text;

        string roomName = GetComponentInChildren<Text>().text.Split('(')[0];
        PhotonNetwork.JoinRoom(roomName);

        // ���� �ִ� ��� ��ư ��Ȱ��ȭ
        FindObjectOfType<LobbyMng>().BtnInteract(false);
    }
}
