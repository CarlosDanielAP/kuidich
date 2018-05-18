using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLosChidos : Team
{
    public string LosChidosName = "Los Chidos";

    private int LosChidosTeamNumber;    // Me lo asigna el game manager
    public int getTeamNumber()
    {
        return LosChidosTeamNumber;
    }

    public List<Transform> LosChidos;  // Yo quiero tener a mis jugadores en una lista
    public List<Transform> Chafas;      // rivales

    private GameObject QuaffleOwner;
	private Transform ClosestTeammateToQuaffle;

    public List<Transform> rivalGoals;

    public List<Transform> myStartingPositions; // Saber donde inician mis jugadores
    public Transform mySeekerStartingPosition;

    public Color myChidoColor;

	protected override void Start () 
    {
        //base.Start();

        // Voy a buscar a mis jugadores
        LosChidos = new List<Transform>();
        LosChidos.Add(transform.Find("Guardian Chido"));
        LosChidos.Add(transform.Find("Cazador Chido"));
        LosChidos.Add(transform.Find("Cazador Chido 2"));
        LosChidos.Add(transform.Find("Cazador Chido 3"));
        LosChidos.Add(transform.Find("Golpeador Chido"));
        LosChidos.Add(transform.Find("Golpeador Chido 2"));
        LosChidos.Add(transform.Find("Buscador Chido"));
		Teammates = LosChidos;

        // Le aviso al GameManager mi nombre de equipo y 
        // me regresa el número de equipo que me toca
        LosChidosTeamNumber =
            GameManager.instancia.SetTeamName(LosChidosName);

        // Ahora que sé el número de equipo
        if (LosChidosTeamNumber == 1)
        {
            //le puedo decir quienes son mis jugadores
            GameManager.instancia.team1Players = LosChidos;
            // Puedo saber hacia donde tiro
            rivalGoals = GameManager.instancia.team2Goals;
        }
        else if (LosChidosTeamNumber == 2)
        {
            GameManager.instancia.team2Players = LosChidos;

            rivalGoals = GameManager.instancia.team1Goals;
        }

        GameManager.instancia.SetTeamColor(LosChidosTeamNumber, myChidoColor);

        Invoke("FillLateData", 1f);
	}
	
    /// <summary>
    /// Hay información que puede no estar disponible en el Start pues no sabemos el orden en que se ejecutan
    /// los equipos, por lo que puede haber información no disponible.
    /// Este método llena la información después de cierto tiempo para tratar de asegurar que esté lista.
    /// </summary>
    void FillLateData()
    {
        if(getTeamNumber()==1)
        {
            // Mis rivales
            Chafas = GameManager.instancia.team2Players;
			Rivals = Chafas;
            // Mis posiciones iniciales
            myStartingPositions = GameManager.instancia.Team1StartPositions;
            mySeekerStartingPosition = GameManager.instancia.Team1SeekerStartPosition;
        }
        else
        {
            Chafas = GameManager.instancia.team1Players;
			Rivals = Chafas;
            myStartingPositions = GameManager.instancia.Team2StartPositions;
            mySeekerStartingPosition = GameManager.instancia.Team2SeekerStartPosition;
        }

        for (int j = 0; j < 6; j++)
        {
            LosChidos[j].GetComponent<Player>().myNumberInTeam = j;
            LosChidos[j].GetComponent<Player>().myStartingPosition = myStartingPositions[j];
        }
        LosChidos[6].GetComponent<Player>().myNumberInTeam = 6;
        LosChidos[6].GetComponent<Player>().myStartingPosition = mySeekerStartingPosition;

    }

	// Update is called once per frame
	protected override void Update ()
    {
        //base.Update();
	}

	public void FindClosestTeammateToQuaffle()
	{
		
		float less = float.MaxValue;
		float dist;

		foreach (Transform chido in LosChidos) 
		{
			dist = Vector3.Distance (chido.position, GameManager.instancia.Quaffle.transform.position);
			if (dist < less) 
			{
				less = dist;
				ClosestTeammateToQuaffle = chido;
			}
		}
	}
}
