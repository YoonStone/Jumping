using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayManager : MonoBehaviour
{
    public Image panel;
    public Button escBtn, backBtn;
    public TextMeshProUGUI countTxt, timerTxt;
    public Image[] playerImgs;
    public TextMeshProUGUI[] rankTxts;
    public RectTransform rankingView;
    public GameObject winner;
    public Color[] colors;

    public bool isCanMove;

    public Player_Play[] players;
    public string[] ranks;

    float timer;

    private void Awake()
    {
        backBtn.onClick.AddListener(ClickBackBtn);
    }

    void ClickBackBtn()
    {
        PhotonNetwork.LoadLevel("2. WaitRoomScene");
    }

    void Start()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerImgs[i].gameObject.SetActive(true);
            rankTxts[i].gameObject.SetActive(true);
        }

        PhotonNetwork.Instantiate("Player_Play", Vector2.zero, Quaternion.identity);

        StartCoroutine(StartCount());
    }

    IEnumerator StartCount()
    {
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            panel.color = new Vector4(panel.color.r, panel.color.g, panel.color.b, timer);
            yield return null;
        }

        // 시간 제한 적용
        Hashtable roomPp = PhotonNetwork.CurrentRoom.CustomProperties;
        this.timer = (float)roomPp["timer"];
        timerTxt.text = this.timer.ToString();

        // 3,2,1 카운트
        timer = 0;
        countTxt.text = "3";
        while (timer < 1)
        {
            timer += Time.deltaTime;
            countTxt.fontSize = Mathf.Lerp(400, 0, timer);
            yield return null;
        }

        timer = 0;
        countTxt.text = "2";
        while (timer < 1)
        {
            timer += Time.deltaTime;
            countTxt.fontSize = Mathf.Lerp(400, 0, timer);
            yield return null;
        }

        timer = 0;
        countTxt.text = "1";
        while (timer < 1)
        {
            timer += Time.deltaTime;
            countTxt.fontSize = Mathf.Lerp(400, 0, timer);
            yield return null;
        }

        // 시작 표시 + 플레이어 순위표 시작
        countTxt.text = "start!";
        countTxt.fontSize = 350;

        players = FindObjectsOfType<Player_Play>();
        for (int i = 0; i < players.Length; i++)
        {
            Hashtable color = players[i].pv.Owner.CustomProperties;
            playerImgs[i].color = colors[(int)color["color"]];
        }

        yield return new WaitForSeconds(1);
        countTxt.gameObject.SetActive(false);
        ranks = new string[players.Length];
        isCanMove = true;
    }

    private void Update()
    {
        if (isCanMove)
        {
            timer -= Time.deltaTime;
            timerTxt.text = timer.ToString("00");

            // 60초가 지나고 나면
            if (timerTxt.text == "00")
            {
                isCanMove = false;
                Time.timeScale = 0;
                StartCoroutine(Ending());
                return;
            }

            // 현재 높이 출력 (나간 플레이어는 -로 표시)
            for (int i = 0; i < players.Length; i++)
            {
                if(players[i])
                    rankTxts[i].text = (players[i].transform.position.y + 10).ToString("00.0") + "km";
                else
                    rankTxts[i].text = "-";
            }
        }
    }

    IEnumerator Ending()
    {
        // 랭킹화면 옮기기
        rankingView.SetParent(rankingView.parent.parent);

        // 서서히 가운데로 오면서 커지기
        Vector3 curPos = rankingView.anchoredPosition;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime;
            rankingView.anchoredPosition = Vector3.Lerp(curPos, Vector3.zero, timer);
            rankingView.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2, timer);
            yield return null;
        }

        // 가장 높은 사람 번호 찾기
        int biggest = 0;
        for (int i = 0; i < rankTxts.Length; i++)
        {
            for (int j = i + 1; j < rankTxts.Length; j++)
            {
                // 둘 중에 하나라도 나간 플레이어라면 비교 패스
                if(rankTxts[i].text == "-" || rankTxts[j].text == "-")
                    continue;

                float curRank = float.Parse(rankTxts[i].text.Remove(4));
                float preRank = float.Parse(rankTxts[j].text.Remove(4));
                float bigRank = float.Parse(rankTxts[biggest].text.Remove(4));

                if (curRank > preRank && curRank > bigRank)
                    biggest = i;
            }
        }

        timer = 1;
        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            for (int i = 0; i < playerImgs.Length; i++)
            {
                if(i != biggest)
                {
                    playerImgs[i].color = new Vector4(playerImgs[i].color.r, playerImgs[i].color.g, playerImgs[i].color.b, timer);
                    rankTxts[i].color = new Vector4(rankTxts[i].color.r, rankTxts[i].color.g, rankTxts[i].color.b, timer);
                }
            }
            yield return null;
        }

        winner.SetActive(true);
        backBtn.gameObject.SetActive(true);
    }
}
