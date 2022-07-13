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
    public Button startBtn;
    public Button[] colorBtns;

    private void Awake()
    {
        startBtn.onClick.AddListener(ClickStartBtn);
    }

    void ClickStartBtn()
    {
        PhotonNetwork.LoadLevel("3. PlayScene");
    }

    void Start()
    {
        PhotonNetwork.Instantiate("Player_Wait", Vector2.zero, Quaternion.identity);

        countTxt.text
            = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

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
    }
}
