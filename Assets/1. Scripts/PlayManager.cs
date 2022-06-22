using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;

    public int[] scores = new int[2];

    public static PlayManager instance;

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
        PhotonNetwork.Instantiate("Player", Vector2.zero, Quaternion.identity);
        scoreTxt.text = "0:0";
    }

    public void ScoreChange(int playerNum)
    {
        scores[playerNum - 1]++;
        scoreTxt.text = $"{scores[0]}:{scores[1]}";
    }
}
