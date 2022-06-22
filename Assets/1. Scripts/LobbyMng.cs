using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMng : MonoBehaviourPunCallbacks
{
    public Text connetTxt;

    public Button createBtn, joinBtn, realCreateBtn;
    public GameObject createTab, joinTab, eventSys, roomBtn;

    public InputField nickInput, roomInput;

    public Transform content;

    enum Mode
    {
        None, Create, Join
    }

    Mode mode = Mode.None;

    List<RoomInfo> myRoomList = new List<RoomInfo>();

    private void Awake()
    {
        createBtn.onClick.AddListener(ClickCreateBtn);
        joinBtn.onClick.AddListener(ClickJoinBtn);
        realCreateBtn.onClick.AddListener(ClickRealCBtn);
    }

    void ClickCreateBtn()
    {
        createTab.SetActive(true);
        joinTab.SetActive(false);
        mode = Mode.Create;
    }

    void ClickRealCBtn()
    {
        // �г��� �Ҵ�
        PhotonNetwork.LocalPlayer.NickName = nickInput.text;

        // ���ο� �� ����� ����
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });

        connetTxt.text = "�� ���� ��...";

        // ��ư ��Ȱ��ȭ
        BtnInteract(false);
    }

    void ClickJoinBtn()
    {
        createTab.SetActive(false);
        joinTab.SetActive(true);
        mode = Mode.Join;
    }

    // ��ư�� ��ȣ�ۿ� ����
    public void BtnInteract(bool isTrue)
    {
        Button[] btns = FindObjectsOfType<Button>();
        foreach (var item in btns)
            item.interactable = isTrue;

        if (!isTrue)
            mode = Mode.None;
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.None: break;

            case Mode.Create:       // �游�����
                if (nickInput.text != "" && roomInput.text != "")
                    realCreateBtn.interactable = true;
                else
                    realCreateBtn.interactable = false;
                break;

            case Mode.Join:         // ���������
                if (nickInput.text != "")
                    for (int i = 0; i < content.childCount; i++)
                        content.GetChild(i).GetComponent<Button>().interactable = true;
                else
                    for (int i = 0; i < content.childCount; i++)
                        content.GetChild(i).GetComponent<Button>().interactable = false;
                break;
        }
    }

    #region // ���� �ݹ� �Լ�

    // �� ���� ���� �� ȣ��
    public override void OnJoinedRoom()
    {
        // �� ��ȯ
        PhotonNetwork.LoadLevel("2. WaitRoomScene");
    }

    // �� ���� ���� �� ȣ��
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connetTxt.text = "�� ���� ����";

        // ��ư Ȱ��ȭ
        BtnInteract(true);
    }

    // �� ���� ���� �� ȣ��
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connetTxt.text = "�� ���� ����";

        // ��ư Ȱ��ȭ
        BtnInteract(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // �� ��� �ϴ� �ʱ�ȭ
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // ���� �� ��� ���� (�ִ��Ÿ� ����, �����Ÿ� �߰�)
        foreach (var item in roomList)
        {
            if (myRoomList.Contains(item))
                myRoomList.Remove(item);
            else
                myRoomList.Add(item);
        }

        // ���� �����ϴ� ���ϴ�� ����
        foreach (var item in myRoomList)
        {
            GameObject room = Instantiate(roomBtn, content);
            Text roomTxt = room.GetComponentInChildren<Text>();

            string playerCount = $"({item.PlayerCount}/{item.MaxPlayers})";
            roomTxt.text = item.Name + playerCount;
        }
    }
    #endregion
}
