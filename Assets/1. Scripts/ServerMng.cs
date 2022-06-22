using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ServerMng : MonoBehaviourPunCallbacks
{
    [Header("필요 컴포넌트")]
    public Button startBtn;
    public TextMeshProUGUI connetTxt;

    [Header("탭 종류")]
    public GameObject lobby;

    [Header("이미지 종류")]
    public Texture2D cursor;
    public Sprite smile;

    Image startImg;

    private void Awake()
    {
        startBtn.onClick.AddListener(ClickStartBtn);
        startImg = startBtn.GetComponent<Image>();

        // 방장은 씬 로드 가능, 모든 플레이어는 자동으로 동일한 씬 로드
        PhotonNetwork.AutomaticallySyncScene = true;

        // 물음표 커서
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    void Start()
    {
        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();

        connetTxt.text = "서버 접속 중...";
    }

    void ClickStartBtn()
    {
        // 로비 접속
        PhotonNetwork.JoinLobby();

        connetTxt.text = "로비 접속 중...";
    }

    // 서버에 접속되면 호출
    public override void OnConnectedToMaster()
    {
        connetTxt.text = "서버 접속 완료";

        startBtn.interactable = true;
        startImg.sprite = smile;
    }

    // 로비에 접속되면 호출
    public override void OnJoinedLobby()
    {
        connetTxt.text = "로비 접속 완료";

        lobby.SetActive(true);
        gameObject.SetActive(false);
    }

    // 접속 실패시 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        connetTxt.text = "접속 실패\n재접속 중...";

        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PhotonNetwork.JoinLobby();
    }
}
