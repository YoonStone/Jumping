using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayManager : MonoBehaviour
{
    public Button escBtn;
    public TextMeshProUGUI countTxt, timerTxt;
    public Image[] playerImgs;
    public TextMeshProUGUI[] rankTxts;
    public RectTransform ranking, rankingView;
    public Color[] colors;

    public bool isCanMove;

    public static PlayManager instance;

    public Player_Play[] players;
    public string[] ranks;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerImgs[i].gameObject.SetActive(true);
            //ExitGames.Client.Photon.Hashtable color = PhotonNetwork.CurrentRoom.GetPlayer(i).CustomProperties;
            //playerImgs[i].color = colors[(int)color["color"]];

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

    float timer = 5;

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

            for (int i = 0; i < players.Length; i++)
            {
                rankTxts[i].text = (players[i].transform.position.y + 10).ToString("00.0") + "km";
            }
        }
    }

    IEnumerator Ending()
    {
        // 랭킹화면 옮기기
        rankingView.gameObject.SetActive(true);
        rankingView.SetParent(ranking.parent);
        for (int i = 0; i < ranking.childCount; i++)
            ranking.GetChild(i).SetParent(rankingView);
        ranking.gameObject.SetActive(false);

        // ** 내 순위표만 남는 오류있음**

        // 서서히 가운데로 오면서 커지기
        Vector3 curPos = rankingView.anchoredPosition;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.unscaledDeltaTime;
            rankingView.anchoredPosition = Vector3.Lerp(curPos, Vector3.zero, timer);
            rankingView.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2, timer);
            yield return null;
        }

        //rankTxts[0].text = $"<color=#B02121>{ranks[0]}</color>";
    }
}
