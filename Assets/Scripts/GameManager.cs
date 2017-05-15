using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct HighscoreEntry
    {
        public System.DateTime Date;
        public int Score;

        public HighscoreEntry(System.DateTime entryDate, int entryScore)
        {
            Date = entryDate;
            Score = entryScore;
        }
    }

    #region Nested Class Highscores
    private class Highscores
    {
        private const int c_capacity = 10;
        private List<HighscoreEntry>[] m_highscoresArray;
        private int[] m_gameTimes;                  // for future use if game times will change

        public Highscores(int[] gameTimes)
        {
            m_gameTimes = gameTimes;
            m_highscoresArray = new List<HighscoreEntry>[m_gameTimes.Length];
        }

        /// <summary>
        /// Checks if the score exceeds any highscore entry. If so, adds it.
        /// </summary>
        /// <param name="score">Score to add.</param>
        /// <param name="gameTimeId">Game time id.</param>
        /// <returns>True if the score was added, otherwise false.</returns>
        public bool TryAddScore(System.DateTime date, int score, int gameTimeId)
        {
            bool success = false;
            if (null == m_highscoresArray[gameTimeId])
            {
                m_highscoresArray[gameTimeId] = new List<HighscoreEntry>(c_capacity);
                m_highscoresArray[gameTimeId].Add(new HighscoreEntry(date, score));
                success = true;
            }
            else
            {
                int length = m_highscoresArray[gameTimeId].Count;
                int place = length;
                for (int i = 0; i < length; ++i)
                {
                    if (score > m_highscoresArray[gameTimeId][i].Score)
                    {
                        place = i;
                        break;
                    }
                }

                if (place < c_capacity)
                {
                    m_highscoresArray[gameTimeId].Insert(place, new HighscoreEntry(date, score));
                    length++;
                    success = true;
                }

                if (length > c_capacity)
                    m_highscoresArray[gameTimeId].RemoveRange(c_capacity, length - c_capacity + 1);
            }
            return success;
        }

        public HighscoreEntry[] GetEntries(int gameTimeId)
        {
            if (null == m_highscoresArray[gameTimeId])
                return null;
            else
                return m_highscoresArray[gameTimeId].ToArray();
        }

        public void WriteToFile(string file)
        {
            using (System.IO.Stream stream = System.IO.File.Open(Application.persistentDataPath + c_highscoreFileName, System.IO.FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, m_highscoresArray);
            }
        }

        public bool TryReadFromFile(string file)
        {
            if (System.IO.File.Exists(Application.persistentDataPath + c_highscoreFileName))
            {
                using (System.IO.Stream stream = System.IO.File.Open(Application.persistentDataPath + c_highscoreFileName, System.IO.FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    var array = (List<HighscoreEntry>[])bformatter.Deserialize(stream);
                    if (null != array)
                    {
                        m_highscoresArray = array;
                        return true;
                    }
                }
            }
            return false;
        }
    }
    #endregion Nested Class Highscores

    public enum GameInputMode
    {
        Mouse,
        VR
    }

    private enum GameStates
    {
        Menu,
        Playing
    }

    private static readonly int[] c_gameTimes = new int[]{ 3, 5, 7 };  // round times in minutes
    private const string c_highscoreFileName = @"/h1ghsc0re.dat";
    
    [Header("Menu-related objects")]
    public PlayerHUDBehavior playerHUD;
    public MainMenuBehavior mainMenu;
    public GameObject logo;

    [Header("Gun settings")]
    public ClickScript gun;
    public Color[] projectileColors = new Color[] { Color.black };
    [Tooltip("Projectile color will change each time player gets this amount of points.")]
    public int projectileColorChange = 400;

    private static GameManager m_instance;

    private NPCManager m_npcManager;
    private PaintProjectileManager m_projectileManager;

    private Highscores m_highscores;

    private GameStates m_state;
    private int m_gameTimeId;               // game times are basically game modes, so we store game time id
    private float m_timeLeft = 180;
    private int m_points;
    private int m_waveCapacity;
    private float m_spawnDelay;
    private int m_projColorCount;


    public static GameManager GetInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        if (null != m_instance)
            Destroy(gameObject);
        else
        {
            m_instance = this;
        }
    }

    void Start()
    {

        m_npcManager = GetComponent<NPCManager>();
        UnityEngine.Debug.Assert(null != m_npcManager, "NPC Manager script is not assigned to Game Manager object.");

        m_projectileManager = GetComponent<PaintProjectileManager>();
        UnityEngine.Debug.Assert(null != m_npcManager, "Paint Projectile Manager script is not assigned to Game Manager object.");

        m_state = GameStates.Menu;
        m_projColorCount = projectileColors.Length;

        // try to load highscores
        m_highscores = new Highscores(c_gameTimes);
        MyReadHighscores();
    }

    void Update()
    {
        if (m_state == GameStates.Playing)
        {
            m_timeLeft -= Time.deltaTime;

            if (m_timeLeft > 0)
            {
                playerHUD.UpdateTime(m_timeLeft);
                playerHUD.UpdatePoints(m_points);

                if (0 == m_waveCapacity)
                {
                    // build new wave
                    m_waveCapacity = Random.Range(5, 40);
                    m_spawnDelay = Random.Range(5, 10);     // first delay is a bit longer than in-wave
                }
                else
                {
                    if (m_spawnDelay > 0)
                        m_spawnDelay -= Time.deltaTime;
                    else
                    {
                        // drone to land npc spawn rate is 1 : 3
                        if (Random.Range(0, 4) == 0)
                            GetNpcManager().SpawnDroneNpcAtRandom();
                        else
                            GetNpcManager().SpawnLandNpcAtRandom();
                        m_spawnDelay = Random.Range(2, 7);
                        m_waveCapacity--;
                    }
                }
            }
            else
            {
                MyToMenuState();
            }
        }
    }

    public NPCManager GetNpcManager()
    {
        return m_npcManager;
    }

    public PaintProjectileManager GetProjectileManager()
    {
        return m_projectileManager;
    }

    public void AddPoints(int amount)
    {
        if (amount < 0)
            playerHUD.IndicatePointsLoss();
        else
            playerHUD.IndicatePointsGain();

        m_points += amount;

        GetProjectileManager().paintBombColor = projectileColors[(m_points / projectileColorChange) % m_projColorCount];
    }

    public int[] GetGameTimes()
    {
        return c_gameTimes;
    }

    public void StartGame(int gameTimeId)
    {
        m_gameTimeId = gameTimeId;
        m_timeLeft = c_gameTimes[m_gameTimeId] * 10;
        MyToPlaying();
    }

    public void RequestExit()
    {
        Application.Quit();
    }

    public HighscoreEntry[] GetHighscores(int gameTimeId)
    {
        return m_highscores.GetEntries(gameTimeId);
    }

    private void MyToMenuState()
    {
        m_state = GameStates.Menu;
        // TODO: also show highscore
        GetNpcManager().DestroyAllNpc();
        playerHUD.Hide();
        mainMenu.Show();
        if (MyCheckMadeHighscore())
        {
            mainMenu.UpdateHighscores();
            MyWriteHighscores();            //TODO: only write on app close
        }
        mainMenu.ShowHighscores(m_gameTimeId);
        logo.SetActive(true);
        gun.SetGunMode(ClickScript.GunMode.UIPointer);
    }

    private void MyToPlaying()
    {
        m_points = 0;
        GetNpcManager().NewGame();
        mainMenu.Hide();
        logo.SetActive(false);
        gun.SetGunMode(ClickScript.GunMode.PaintGun);
        m_state = GameStates.Playing;
    }

    private bool MyCheckMadeHighscore()
    {
        return m_highscores.TryAddScore(System.DateTime.Now, m_points, m_gameTimeId);
    }

    private void MyWriteHighscores()
    {
        m_highscores.WriteToFile(Application.persistentDataPath + c_highscoreFileName);
    }

    private void MyReadHighscores()
    {
        m_highscores.TryReadFromFile(Application.persistentDataPath + c_highscoreFileName);
    }
}
