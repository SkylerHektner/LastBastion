using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeViewer : MonoBehaviour
{
    public Animator BigScroll;
    public List<Animator> CoinsList;
    public List<GameObject> UpgradesList;
    bool Open;
    public ScrollRect ScrollRect;
    Color GlowColor;
    public Image ColorL;
    public Image ColorR;
    public TextMeshProUGUI PlayerWealthText;
    Spectator Spectator;




    public void SetContent()
    {
        for (int i = 0; i < CoinsList.Count; i++)
        {
            if (CoinsList[i].GetBool("Selected") == true) // if the button is on and glowing
            {
                UpgradesList[i].SetActive(true);
                ScrollRect.content = UpgradesList[i].GetComponent<RectTransform>();
            }
            else
            {
                UpgradesList[i].SetActive(false);
            }
        }

    }
    private void OnEnable()
    {
        Spectator = GameObject.FindGameObjectWithTag("Spectator").GetComponent<Spectator>();
        PlayerWealthText.text = PlayerData.Instance.PlayerWealth.Get().ToString();
    }

    public void ToggleOpen(Animator Coin)
    {
        // close it if highlighted button is pressed
        if (Open && Coin.GetBool("Selected") == true)
        {
            BigScroll.SetBool("Open", false);
            Coin.SetBool("Selected", false);
            Open = false;
        }
        else if(!Open && Coin.GetBool("Selected") == false) // unhighlighted button is pressed, open me
        {
            BigScroll.SetBool("Open", true);
            Coin.SetBool("Selected", true);
            Open = true;
            SetContent();
        }
        else if (Open && Coin.GetBool("Selected") == false)
        {
            foreach (Animator coin in CoinsList)
            {
                if (coin != Coin)
                {
                    coin.SetBool("Selected", false);
                }

            }
            Coin.SetBool("Selected", true);
            SetContent();
        }


    }

    public void SetGlow(Button CoinButton)
    {

        GlowColor = CoinButton.colors.highlightedColor;
        ColorR.color = GlowColor;
        ColorL.color = GlowColor;
    }

    private void OnDisable()
    {
        Open = false;
    }
}
