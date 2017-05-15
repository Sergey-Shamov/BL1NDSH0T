using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBehavior : MonoBehaviour
{
    public Text gameTimeSelector;
    public MainMenuSidePanelBehavior sidePanelCanvas;

    private int[] m_gameTimes;
    private int m_gameTimeId;

    void Start()
    {
        m_gameTimes = GameManager.GetInstance().GetGameTimes();
        m_gameTimeId = 0;
        MyUpdateSelector();
        sidePanelCanvas.Initialize(m_gameTimes);
        sidePanelCanvas.gameObject.SetActive(false);
    }

    public void NextGameTime()
    {
        m_gameTimeId++;
        if (m_gameTimeId >= m_gameTimes.Length)
            m_gameTimeId = 0;
        MyUpdateSelector();
    }

    public void PrevGameTime()
    {
        m_gameTimeId--;
        if (m_gameTimeId < 0)
            m_gameTimeId = m_gameTimes.Length - 1;
        MyUpdateSelector();
    }

    public void StartGame()
    {
        GameManager.GetInstance().StartGame(m_gameTimeId);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        sidePanelCanvas.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        sidePanelCanvas.gameObject.SetActive(true);
    }

    public void ShowHighscores()
    {
        sidePanelCanvas.gameObject.SetActive(true);
        sidePanelCanvas.ShowHighscores();
    }

    public void ShowHighscores(int gameTimeId)
    {
        sidePanelCanvas.gameObject.SetActive(true);
        sidePanelCanvas.ShowHighscores(gameTimeId);
    }

    public void ShowAbout()
    {
        sidePanelCanvas.gameObject.SetActive(true);
        sidePanelCanvas.ShowAbout();
    }

    public void UpdateHighscores()
    {
        sidePanelCanvas.RefreshHighscores();
    }

    public void RequestExit()
    {
        GameManager.GetInstance().RequestExit();
    }

    private void MyUpdateSelector()
    {
        gameTimeSelector.text = m_gameTimes[m_gameTimeId].ToString();
    }
}
