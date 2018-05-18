using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamElfosDomesticos : Team
{
    public string LosElfosName = "Los Elfos Domesticos";

    private int LosElfosTeamNumber;    // Me lo asigna el game manager
    public int getTeamNumber()
    {
        return LosElfosTeamNumber;
    }

    public List<Transform> LosElfos;  // Yo quiero tener a mis jugadores en una lista
    public List<Transform> Chafas;      // rivales

    private GameObject QuaffleOwner;
	private Transform ClosestTeammateToQuaffle;

    public List<Transform> rivalGoals;

    public List<Transform> myStartingPositions; // Saber donde inician mis jugadores
    public Transform mySeekerStartingPosition;


	public GameObject ChaserCercano, ChaserLejano;

	public List<GameObject> chasers;


	void Start () 
    {
        // Voy a buscar a mis jugadores
        LosElfos = new List<Transform>();
        LosElfos.Add(transform.Find("Guardian Elfo"));
        LosElfos.Add(transform.Find("Cazador Elfo"));
        LosElfos.Add(transform.Find("Cazador Elfo 2"));
        LosElfos.Add(transform.Find("Cazador Elfo 3"));
        LosElfos.Add(transform.Find("Golpeador Elfo"));
        LosElfos.Add(transform.Find("Golpeador Elfo 2"));
        LosElfos.Add(transform.Find("Buscador Elfo"));
		Teammates = LosElfos;

        // Le aviso al GameManager mi nombre de equipo y 
        // me regresa el número de equipo que me toca
        LosElfosTeamNumber =
            GameManager.instancia.SetTeamName(LosElfosName);

        // Ahora que sé el número de equipo
        if (LosElfosTeamNumber == 1)
        {
            //le puedo decir quienes son mis jugadores
            GameManager.instancia.team1Players = LosElfos;
            // Puedo saber hacia donde tiro
            rivalGoals = GameManager.instancia.team2Goals;
        }
        else if (LosElfosTeamNumber == 2)
        {
            GameManager.instancia.team2Players = LosElfos;

            rivalGoals = GameManager.instancia.team1Goals;
        }

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
		Debug.Log (LosElfos.Count);

        for (int j = 0; j < 6; j++)
        {
            LosElfos[j].GetComponent<Player>().myNumberInTeam = j;
            LosElfos[j].GetComponent<Player>().myStartingPosition = myStartingPositions[j];
        }
        LosElfos[6].GetComponent<Player>().myNumberInTeam = 6;
        LosElfos[6].GetComponent<Player>().myStartingPosition = mySeekerStartingPosition;

    }


	void PosicionInicialChasers()
	{
		for (int j = 0; j < 6; j++)
		{
			LosElfos[j].GetComponent<Player>().myNumberInTeam = j;
			LosElfos[j].GetComponent<Player>().myStartingPosition = myStartingPositions[j];
		}
		LosElfos[6].GetComponent<Player>().myNumberInTeam = 6;
		LosElfos[6].GetComponent<Player>().myStartingPosition = mySeekerStartingPosition;
	}

	// Update is called once per frame
	protected override void Update ()
    {
		
	}





	public void FindClosestTeammateToQuaffle()
	{
		
		float less = float.MaxValue;
		float dist;

		foreach (Transform chido in LosElfos) 
		{
			dist = Vector3.Distance (chido.position, GameManager.instancia.Quaffle.transform.position);
			if (dist < less) 
			{
				less = dist;
				ClosestTeammateToQuaffle = chido;
			}
		}
	}

	public bool isTeammate(Transform player)
	{
		return Teammates.Contains(player.transform);
	}

	public bool isRival(Transform player)
	{
		return Rivals.Contains(player.transform);
	}



	public Transform SupportPlayer()
	{
		GameObject temp = null;
		int c = int.MaxValue;
		foreach (var item in chasers)
		{

			if (item.GetComponent<ChaserElfo>().enemigosCercanos.Count < c)
			{
				c = item.GetComponent<ChaserElfo>().enemigosCercanos.Count;
				temp = item;
			}

		}

		if (temp != null)
		{
			temp.GetComponent<ChaserElfo>().fsm.ChangeState(ChaserElfoStates.ChaserStateID.ChaseBall);
			return temp.transform;

		}
		else
			return null;

	}



}
