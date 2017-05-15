using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSidePanelBehavior : MonoBehaviour
{
    public GameObject highscorePanel;
    public GameObject aboutPanel;
    public Text datesText;
    public Text scoresText;
    public Text gameTimeText;

    private int[] m_gameTimes;
    private int m_gameTimeId;

    public void Initialize(int[] gameTimes)
    {
        m_gameTimes = gameTimes;
        m_gameTimeId = 0;
    }

    public void NextGameTime()
    {
        m_gameTimeId++;
        if (m_gameTimeId >= m_gameTimes.Length)
            m_gameTimeId = 0;
        MyUpdateHighscores();
    }

    public void PrevGameTime()
    {
        m_gameTimeId--;
        if (m_gameTimeId < 0)
            m_gameTimeId = m_gameTimes.Length - 1;
        MyUpdateHighscores();
    }

    public void RefreshHighscores()
    {
        MyUpdateHighscores();
    }

    public void ShowHighscores(int gameTimeId)
    {
        if (gameTimeId != m_gameTimeId)
            m_gameTimeId = gameTimeId;
        ShowHighscores();
    }

    public void ShowHighscores()
    {
        MyUpdateHighscores();
        highscorePanel.SetActive(true);
        aboutPanel.SetActive(false);
    }

    public void ShowAbout()
    {
        highscorePanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    private void MyUpdateHighscores()
    {
        gameTimeText.text = m_gameTimes[m_gameTimeId].ToString();

        GameManager.HighscoreEntry[] highscores = GameManager.GetInstance().GetHighscores(m_gameTimeId);
        MyPopulateLines(highscores);
    }

    private void MyPopulateLines(GameManager.HighscoreEntry[] highscores)
    {
        datesText.text = string.Empty;
        scoresText.text = string.Empty;
        if (null != highscores)
        {
            int length = highscores.Length;
            System.Text.StringBuilder sbDates = new System.Text.StringBuilder();
            System.Text.StringBuilder sbScores = new System.Text.StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sbDates.Append(highscores[i].Date.ToString("yyyy-MM-dd HH:mm:ss"));
                sbDates.Append("\n");
                sbScores.Append(highscores[i].Score);
                sbScores.Append("\n");
            }
            datesText.text = sbDates.ToString();
            scoresText.text = sbScores.ToString();
        }
    }
}
