using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class Test : MonoBehaviour
{
    public TMP_InputField timer;
    public bool isSeleted;

    private void Awake()
    {
        timer.onSelect.AddListener(IPSelect);
        timer.onDeselect.AddListener(IPDeselect);
    }

    void IPSelect(string text)
    {
        isSeleted = true;
    }

    void IPDeselect(string text)
    {
        isSeleted = false;
    }

    private void Update()
    {
        timer.text = Regex.Replace(timer.text, @"[^0-9]", "");

        if (timer.text == "" || timer.text == null)
        {
            print(timer.text + "공백");
            timer.text = "";
        }
        else
        {
            print(timer.text);
        }
    }
}
