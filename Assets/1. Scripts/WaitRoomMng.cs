using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WaitRoomMng : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countTxt;
    public TMP_Dropdown timer;
    public Button startBtn, escBtn;
    public Button[] colorBtns;

    public bool isSeleted;

    int[] dropDown = { 120, 90, 60, 45, 30, 15, 10, 5 };

    private void Awake()
    {
        Time.timeScale = 1;
        startBtn.onClick.AddListener(ClickStartBtn);
    }

    void ClickStartBtn()
    {
        // 제한 시간
        float setTimer = dropDown[timer.value];

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "timer", setTimer } });
        startBtn.gameObject.SetActive(false);

        Invoke("StartDelay", 0.1f);
    }

    void StartDelay()
    {
        PhotonNetwork.LoadLevel("3. PlayScene");
    }

    private void Start()
    {
        Time.timeScale = 1;
        PhotonNetwork.Instantiate("Player_Wait", Vector2.zero, Quaternion.identity);

        countTxt.text
            = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #region [포톤 콜백 함수]
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        countTxt.text = playerCount  + "/" + maxPlayers;

        if (playerCount == maxPlayers)
            startBtn.interactable = true;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        countTxt.text = playerCount + "/" + maxPlayers;

        if (playerCount != maxPlayers)
            startBtn.interactable = false;

        // 떠난 플레이어가 갖고 있던 색상 활성화
        Hashtable color = otherPlayer.CustomProperties;
        colorBtns[(int)color["color"]].interactable = true;
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("1. LobbyScene");
    }
    #endregion
}
