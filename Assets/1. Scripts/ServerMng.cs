using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ServerMng : MonoBehaviourPunCallbacks
{
    public Button startBtn;
    public Text connetTxt;
    public GameObject lobby;

    private void Awake()
    {
        startBtn.onClick.AddListener(ClickStartBtn);

        // ������ �� �ε� ����, ��� �÷��̾�� �ڵ����� ������ �� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // ���� ����
        PhotonNetwork.ConnectUsingSettings();

        connetTxt.text = "���� ���� ��...";
    }

    void ClickStartBtn()
    {
        // �κ� ����
        PhotonNetwork.JoinLobby();

        connetTxt.text = "�κ� ���� ��...";
    }

    // ������ ���ӵǸ� ȣ��
    public override void OnConnectedToMaster()
    {
        connetTxt.text = "���� ���� �Ϸ�";

        startBtn.interactable = true;
    }

    // �κ� ���ӵǸ� ȣ��
    public override void OnJoinedLobby()
    {
        connetTxt.text = "�κ� ���� �Ϸ�";

        lobby.SetActive(true);
        gameObject.SetActive(false);
    }

    // ���� ���н� ȣ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        connetTxt.text = "���� ����\n������ ��...";

        // ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PhotonNetwork.JoinLobby();
    }
}
