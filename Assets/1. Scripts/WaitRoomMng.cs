using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Text.RegularExpressions;

public class WaitRoomMng : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countTxt;
    public InputField timer;
    public Button startBtn, escBtn;
    public Button[] colorBtns;

    public bool isSeleted;

    private void Awake()
    {
        Time.timeScale = 1;
        startBtn.onClick.AddListener(ClickStartBtn);
        timer.onEndEdit.AddListener(IPEditEnd);
    }

    public string curText = "60";

    void IPEditEnd(string text)
    {
        timer.text = Regex.Replace(text, @"[^0-9]", "");

        // 0이 아닐 때도 포함 **
        if (text == "0" || text == "" || text == null || timer.text.Length == 0)
            timer.text = curText;
        else
            curText = timer.text;
    }

    void ClickStartBtn()
    {
        // 제한 시간
        float setTimer = float.Parse(timer.text);

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
