using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardEntryData : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerWaveText;
    public Image PlayerProfilePicture;
    public int HighestWave;
    public int CurrentRanking;
    public Image RankingBubble;

    public void SetMyInfo(string MyName, int MyScore, Sprite MyPicture)
    {
        PlayerNameText.text = MyName;
        PlayerWaveText.text = "Wave " + MyScore.ToString();
        PlayerProfilePicture.sprite = MyPicture;
        HighestWave = MyScore;
    }
}
