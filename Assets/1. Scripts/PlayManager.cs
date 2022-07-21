using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayManager : MonoBehaviour
{
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
        float timer = 0;
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

        countTxt.text = "start!";
        countTxt.fontSize = 350;

        players = FindObjectsOfType<Player_Play>();
        for (int i = 0; i < players.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable color = players[i].pv.Owner.CustomProperties;
            playerImgs[i].color = colors[(int)color["color"]];
        }

        yield return new WaitForSeconds(1);

        countTxt.gameObject.SetActive(false);
        ranks = new string[players.Length];
        isCanMove = true;
    }

    float timer = 2;

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

            // 현재 높이 출력
            for (int i = 0; i < players.Length; i++)
                rankTxts[i].text = (players[i].transform.position.y + 10).ToString("00.0") + "km";
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
                if (float.Parse(rankTxts[i].text.Remove(4)) > float.Parse(rankTxts[j].text.Remove(4)) 
                    && float.Parse(rankTxts[i].text.Remove(4)) > float.Parse(rankTxts[biggest].text.Remove(4)))
                {
                    biggest = i;
                }
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
