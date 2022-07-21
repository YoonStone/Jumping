using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class WaitRoomMng : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countTxt;
    public Button startBtn, escBtn;
    public Button[] colorBtns;

    private void Awake()
    {
        Time.timeScale = 1;
        startBtn.onClick.AddListener(ClickStartBtn);
    }

    void ClickStartBtn()
    {
        PhotonNetwork.LoadLevel("3. PlayScene");
    }

    void Start()
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
        ExitGames.Client.Photon.Hashtable color = otherPlayer.CustomProperties;
        colorBtns[(int)color["color"]].interactable = true;
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("1. LobbyScene");
    }
    #endregion
}
