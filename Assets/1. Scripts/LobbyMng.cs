using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyMng : MonoBehaviourPunCallbacks
{
    [Header("방 최대인원")]
    public byte maxPlayer;

    [Header("필요 컴포넌트")]
    public TextMeshProUGUI connetTxt;
    public Button createBtn, joinBtn, realCreateBtn;
    public TMP_InputField nickInput, roomInput;
    public Transform content;
    public GameObject roomBtn;

    [Header("탭 종류")]
    public GameObject createTab;
    public GameObject joinTab;

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
        // 닉네임 할당
        PhotonNetwork.LocalPlayer.NickName = nickInput.text;

        // 새로운 방 만들고 접속
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = maxPlayer });

        connetTxt.text = "방 접속 중...";

        // 버튼 비활성화
        BtnInteract(false);
    }

    void ClickJoinBtn()
    {
        createTab.SetActive(false);
        joinTab.SetActive(true);
        mode = Mode.Join;
    }

    // 버튼의 상호작용 관리
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

            case Mode.Create:       // 방만들기모드
                if (nickInput.text != "" && roomInput.text != "")
                    realCreateBtn.interactable = true;
                else
                    realCreateBtn.interactable = false;
                break;

            case Mode.Join:         // 방참가모드
                if (nickInput.text != "")
                    for (int i = 0; i < content.childCount; i++)
                        content.GetChild(i).GetComponent<Button>().interactable = true;
                else
                    for (int i = 0; i < content.childCount; i++)
                        content.GetChild(i).GetComponent<Button>().interactable = false;
                break;
        }
    }

    #region // 포톤 콜백 함수

    // 방 접속 성공 시 호출
    public override void OnJoinedRoom()
    {
        // 씬 전환
        PhotonNetwork.LoadLevel("2. WaitRoomScene");
    }

    // 방 생성 실패 시 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connetTxt.text = "방 접속 실패";

        // 버튼 활성화
        BtnInteract(true);
    }

    // 방 접속 실패 시 호출
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connetTxt.text = "방 접속 실패";

        // 버튼 활성화
        BtnInteract(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방 목록 일단 초기화
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // 따로 방 목록 관리 (있던거면 삭제, 없던거면 추가)
        foreach (var item in roomList)
        {
            if (myRoomList.Contains(item))
                myRoomList.Remove(item);
            else
                myRoomList.Add(item);
        }

        // 따로 관리하는 방목록대로 생성
        foreach (var item in myRoomList)
        {
            GameObject room = Instantiate(roomBtn, content);
            TextMeshProUGUI roomTxt = room.GetComponentInChildren<TextMeshProUGUI>();

            string playerCount = $"({item.PlayerCount}/{item.MaxPlayers})";
            roomTxt.text = item.Name + playerCount;
        }
    }
    #endregion
}
