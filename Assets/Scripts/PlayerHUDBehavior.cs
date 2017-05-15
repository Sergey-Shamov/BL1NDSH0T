using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDBehavior : MonoBehaviour
{
    public Text timerText;
    public Text pointsText;
    public Image pointsLossSprite;
    public Image pointsGainSprite;
    
    void Update()
    {
        Color ptsLossColor = pointsLossSprite.color;
        if (ptsLossColor.a > 0)
        {
            ptsLossColor.a -= Time.deltaTime * .9f;
            pointsLossSprite.color = ptsLossColor;
        }

        Color ptsGainColor = pointsGainSprite.color;
        if (ptsGainColor.a > 0)
        {
            ptsGainColor.a -= Time.deltaTime * .9f;
            pointsGainSprite.color = ptsGainColor;
        }
    }

    public void IndicatePointsGain()
    {
        Color ptsGainColor = pointsGainSprite.color;
        ptsGainColor.a = .8f;
        pointsGainSprite.color = ptsGainColor;
    }

    public void IndicatePointsLoss()
    {
        Color ptsLossColor = pointsLossSprite.color;
        ptsLossColor.a = .9f;
        pointsLossSprite.color = ptsLossColor;
    }

    public void UpdateTime(float time)
    {
        timerText.text = string.Format("{0:00}:{1:00}", (int)(time / 60), ((int)time % 60));
    }

    public void UpdatePoints(int points)
    {
        pointsText.text = points.ToString();
    }

    public void Hide()
    {
        timerText.text = string.Empty;
        Color color = pointsLossSprite.color;
        color.a = 0;
        pointsLossSprite.color = color;
        color = pointsGainSprite.color;
        color.a = 0;
        pointsGainSprite.color = color;
    }
}
