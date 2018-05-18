using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager instancia = null;

    void Awake()
    {
        if(instancia == null)
        {
            instancia = this;
        }
        else if(instancia != this)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);

        // Asignar las pelotas
        Quaffle = GameObject.Find("Quaffle");
        Snitch = GameObject.Find("Snitch");
        Bludger = new GameObject[2];
        Bludger[0] = GameObject.Find("Bludger1");
        Bludger[1] = GameObject.Find("Bludger2");

        // Buscamos las referencias a los aros
        team1Goals = new List<Transform>();
        team1Goals.Add(GameObject.Find("Team1Goal1").transform);
        team1Goals.Add(GameObject.Find("Team1Goal2").transform);
        team1Goals.Add(GameObject.Find("Team1Goal3").transform);
        for (int t = 0; t < 3; t++)
            team1Goals[t].GetComponent<ScoreScript>().setTeamOwner(1);

        team2Goals = new List<Transform>();
        team2Goals.Add(GameObject.Find("Team2Goal1").transform);
        team2Goals.Add(GameObject.Find("Team2Goal2").transform);
        team2Goals.Add(GameObject.Find("Team2Goal3").transform);
        for (int t = 0; t < 3; t++)
            team2Goals[t].GetComponent<ScoreScript>().setTeamOwner(2);

        // Referencias a las posiciones iniciales de los equipos
        // Equipo 1
        Team1StartPositions = new List<Transform>();
        for (int p = 1; p <= 6; p++)
            Team1StartPositions.Add(GameObject.Find("Team1StartPosition ("+p.ToString()+")").transform);
        Team1SeekerStartPosition = GameObject.Find("Seeker1StartPosition").transform;
        // Equipo 2
        Team2StartPositions = new List<Transform>();
        for (int p = 1; p <= 6; p++)
            Team2StartPositions.Add(GameObject.Find("Team2StartPosition (" + p.ToString() + ")").transform);
        Team2SeekerStartPosition = GameObject.Find("Seeker2StartPosition").transform;

    }

    // Referencias a las pelotas
    public GameObject Quaffle;
    public GameObject Snitch;
    public GameObject[] Bludger;

    private GameObject QuaffleControllingPlayer;
    public bool isQuaffleControlled()
    {
        return QuaffleControllingPlayer != null;
    }
    public bool ControlQuaffle(GameObject Player)
    {
        if (QuaffleControllingPlayer == null)
        {
            if (Quaffle.GetComponent<Quaffle>().Control(Player.transform))
            {
                QuaffleControllingPlayer = Player;
                return true;
            }
            
        }
        return false;
    }
    public void FreeQuaffle()
    {
        // no tiene dueño
        QuaffleControllingPlayer = null;

        Quaffle.GetComponent<Quaffle>().Free();
    }

    // Variables de equipos
    private string team1name = "";
    private string team2name = "";
    public int SetTeamName(string name)
    {
        // El manager recibe el nombre de un equipo
        if(team1name.Equals(""))
        {
            team1name = name;   // Asigno el nombre al equipo 
            team1NameText.text = team1name;
            return 1;       // Regresamos el numero de equpo que les toca
        }
        else
        {
            team2name = name;
            team2NameText.text = team2name;
            return 2;
        }
    }
    public void SetTeamColor(int team, Color color)
    {
        if(team==1)
        {
            team1ColorImage.color = color;
        }
        else if(team==2)
        {
            team2ColorImage.color = color;
        }
    }

    // Lista de jugadores
    public List<Transform> team1Players;
    public List<Transform> team2Players;

    // Referencia a los aros de cada equipo
    public List<Transform> team1Goals;  // Son los aros que defiende el equipo 1
    public List<Transform> team2Goals;

    // Puntaje de cada equipo
    public GameObject ScoreFX;
    private int team1Score;     // Los puntos que lleva el equipo 1
    private int team2Score;
    public int GetTeam1Points()
    {
        return team1Score;
    }
    public int GetTeam2Points()
    {
        return team2Score;
    }
    public void Score(int teamNumber, int points)
    {
        if (teamNumber == 1)
        {
            team1Score += points;
            lastTeamScore = 1;
            recoveryTimeLeft = afterScoreRecoveryTime;
        }
        else if (teamNumber == 2)
        {
            team2Score += points;
            lastTeamScore = 2;
            recoveryTimeLeft = afterScoreRecoveryTime;
        }

        team1ScoreText.text = team1Score.ToString();
        team2ScoreText.text = team2Score.ToString();
    }

    [Header("Scores")]
    public Text team1ScoreText;
    public Text team1NameText;
    public Image team1ColorImage;
    
    public Text team2ScoreText;
    public Text team2NameText;
    public Image team2ColorImage;


    // Posiciones iniciales para los jugadores de cada equipo
    public List<Transform> Team1StartPositions;
    public List<Transform> Team2StartPositions;
    public Transform Team1SeekerStartPosition;
    public Transform Team2SeekerStartPosition;


    private int lastTeamScore;
    // Cada equipo tiene 5 segundos para recuperar la quaffle después de que le hayan anotado
    private const float afterScoreRecoveryTime = 5f;
    private float recoveryTimeLeft;
    /// <summary>
    /// Regresa que equipo se está recuperando después de que le anotaron
    /// </summary>
    /// <returns>
    /// 0 - Ningún equipo se está recuperando
    /// 1 - Equipo 1 se está recuperando
    /// 2 - Equipo 2 se está recuperando
    /// </returns>
    public int IsRecovering()
    {
        if(recoveryTimeLeft > 0f)
        {
            if (lastTeamScore == 1)
                return 2;
            else if (lastTeamScore == 2)
                return 1;
        }
        return 0;
    }
    private void RecoverAfterScore()
    {
        if(recoveryTimeLeft > 0f)
        {
            recoveryTimeLeft -= Time.deltaTime;
        }
    }


    // Variables de control del flujo del juego
    private bool gameStarted = false;
    public bool isGameStarted()
    {
        return gameStarted;
    }
    private bool gamePause = false;
    public bool isGamePaused()
    {
        return gamePause;
    }
    private int team1PausesLeft;
    private int team2PausesLeft;
    public bool RequestPause(int requestingTeam)
    {
        if(requestingTeam == 1)
        {
            if (team1PausesLeft > 0)
            {
                team1PausesLeft--;
                gamePause = true;
                StartCoroutine("InitPause");
                return true;
            }
            else return false;
        }
        else if(requestingTeam == 2)
        {
            if (team2PausesLeft > 0)
            {
                team2PausesLeft--;
                gamePause = true;
                StartCoroutine("InitPause");
                return true;
            }
            else return false;
        }
        return false;
    }
    private float pauseTime = 15f;
    private IEnumerator InitPause()
    {
        yield return new WaitForSeconds(pauseTime);
        StartGame();
    }
    public float timeToStartGame;
    private void StartGame()
    {
        StartCoroutine("InitGame");
    }
    private IEnumerator InitGame()
    {
        yield return new WaitForSeconds(timeToStartGame);

        gameStarted = true;
        gamePause = false;

        Quaffle.GetComponent<Rigidbody>().AddForce(Vector3.up * 1500f);
    }
    private bool gameover = false;
    public bool isGameOver()
    {
        return gameover;
    }

    private int winner = 0;

    public bool GrabSnitch(GameObject player)
    {
        // Verificar si el jugador que quiere agarrar la snitch está cerca
        if (Vector3.Distance(player.transform.position, Snitch.transform.position) > 4f)
            return false;
        // Que el juego no haya terminado
        if (gameover)
            return false;
        // Que otro jugador la haya agarrado
        if (winner > 0)
            return false;

        // Podemos decir que el jugador logró atrapar la snitch
        if(team1Players.Contains(player.transform))
        {
            // damos los puntos
            Score(1, 150);
            
            // terminamos el juego
            gameover = true;

            SetWinner();

            return true;
        }
        else if(team2Players.Contains(player.transform))
        {
            Score(2, 150);
            
            gameover = true;

            SetWinner();
            return true;
        }

        return false;
    }

    private void SetWinner()
    {
        if (team1Score > team2Score)
            winner = 1;
        else if (team1Score < team2Score)
            winner = 2;

        GameObject.Find("Field Camera").GetComponent<FieldCamera>().LookSnitchAtEnd();
    }

    void Start()
    {
       

        //team1Players = new List<Transform>();
        //team2Players = new List<Transform>();

        team1Score = 0;
        team2Score = 0;
        team1ScoreText.text = "0";
        team2ScoreText.text = "0";

        gamePause = true;

        StartGame();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // El juego empezó
		if(isGameStarted() && !gameover)
        {
            RecoverAfterScore();
        }
	}
}
