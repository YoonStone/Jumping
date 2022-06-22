using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomMng : MonoBehaviourPunCallbacks
{
    public Text countTxt;
    public Button startBtn;

    private void Awake()
    {
        startBtn.onClick.AddListener(ClickStartBtn);
    }

    void ClickStartBtn()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            PhotonNetwork.LoadLevel("3. PlayScene");
    }

    void Start()
    {
        PhotonNetwork.Instantiate("Player_Wait", Vector2.zero, Quaternion.identity);

        countTxt.text
            = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        countTxt.text 
            = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            startBtn.interactable = true;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        countTxt.text
            = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
            startBtn.interactable = false;
    }
}
