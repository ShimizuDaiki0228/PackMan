using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private static int _lifes;
    private static int _score;
    private static int _level = 1;
    private int _pelletAmount;

    [SerializeField]
    private List<GameObject> _ghostList = new List<GameObject>();
    private static List<GameObject> GhostList;

    private GameObject _packMan;

    private bool _scatter;
    private bool _chase;
    public bool Frighten;

    private const float SCATTER_TIMER = 7f;
    private float _currentScatterTimer = 0f;

    private const float CHASE_TIMER = 20f;
    private float _currentChaseTimer = 0f;

    private const float FRIGHTEND_TIMER = 5f;
    private float _currentFrightendTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }

        _lifes = 3;
        GhostList = _ghostList;
    }

    private void Start()
    {
        _scatter = true;
    }

    private void Update()
    {
        Timing();
    }

    public void AddPellet()
    {
        _pelletAmount++;
    }

    public void ReducePellet(int score)
    {
        _pelletAmount--;
        AddScore(score);
        if( _pelletAmount <= 0 )
        {
            WinCondition();
        }

        foreach(var ghost in GhostList)
        {
            PathFindings pGhost = ghost.GetComponent<PathFindings>();
            if(_score >= pGhost.PointsToCollect && !pGhost.Released)
            {
                pGhost.State = GhostStates.CHASE;
                pGhost.Released = true;
            }
        }
    }

    private void Timing()
    {
        UpdateStates();
        if(_chase)
        {
            _currentChaseTimer += Time.deltaTime;
            if(_currentChaseTimer >= CHASE_TIMER)
            {
                _currentChaseTimer = 0f;
                _chase = false;
                _scatter = true;
            }
        }
        if(_scatter)
        {
            _currentScatterTimer += Time.deltaTime;
            if(_currentScatterTimer >= SCATTER_TIMER)
            {
                _currentScatterTimer = 0f;
                _chase = true;
                _scatter = false;
            }
        }
        if(Frighten)
        {
            if(_currentChaseTimer != 0 || _currentScatterTimer != 0)
            {
                _chase = false;
                _scatter = false;
                _currentChaseTimer = 0;
                _currentScatterTimer = 0;
            }


            _currentFrightendTimer += Time.deltaTime;
            if(_currentFrightendTimer >= FRIGHTEND_TIMER)
            {
                _currentFrightendTimer = 0f;
                _chase = true;
                _scatter = false;
                Frighten = false;
            }
        }
        
    }

    private void UpdateStates()
    {
        foreach(var ghost in GhostList)
        {
            PathFindings pGhost = ghost.GetComponent<PathFindings>();
            if(pGhost.State == GhostStates.CHASE && _scatter)
            {
                pGhost.State = GhostStates.SCATTER;
            }
            else if(pGhost.State == GhostStates.SCATTER && _chase)
            {
                pGhost.State = GhostStates.CHASE;
            }
            else if(pGhost.State != GhostStates.HOME && pGhost.State != GhostStates.GOT_EATEN && Frighten)
            {
                pGhost.State = GhostStates.FRIGHTEND;
            }
            else if(pGhost.State == GhostStates.FRIGHTEND)
            {
                pGhost.State = GhostStates.CHASE;
            }
        }
    }

    private void WinCondition()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// シーンが更新された時に行う処理
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _level++;
        _packMan = GameObject.FindGameObjectWithTag("PackMan");

        if(_score >= (_level - 1) * 3000)
        {
            _lifes++;
        }
    }

    public void LoseLife()
    {
        _lifes--;
        if(_lifes <= 0)
        {
            ScoreController.Level = _level;
            ScoreController.Score = _score;

            SceneManager.LoadScene("GameOverScene");
            return;
        }

        //foreach(GameObject ghost in GhostList)
        //{
        //    ghost.GetComponent<PathFindings>().Reset();
        //}
    }

    public void AddScore(int addScore)
    {
        _score += addScore;
    }
}
