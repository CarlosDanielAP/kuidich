using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDanis : Team
{
    public string DanisName = "Danis";

    private int DanisTeamNumber;    // Me lo asigna el game manager
    public int getTeamNumber()
    {
        return DanisTeamNumber;
    }

    public List<Transform> Danis;  // Yo quiero tener a mis jugadores en una lista
    public List<Transform> Chafas;      // rivales

    public List<Transform> DanisChasers;

    private GameObject QuaffleOwner;
	private Transform ClosestTeammateToQuaffle;
   public Transform ClosestChaserToQuaffle;
    public Transform ClosestChaserToGoal;
    public GameObject ClosestEnemyToQuaffle;


    public List<Transform> rivalGoals;
    public Transform rivalGoals_position;

    public List<Transform> myGoals;
    public Transform myGoals_position;

    public List<Transform> myStartingPositions; // Saber donde inician mis jugadores
    public Transform mySeekerStartingPosition;

    public Color myDanisColor;

	protected override void Start () 
    {
        //base.Start();
        DanisChasers = new List<Transform>();
        DanisChasers.Add(transform.Find("Cazador Danis"));
        DanisChasers.Add(transform.Find("Cazador Danis 2"));
        DanisChasers.Add(transform.Find("Cazador Danis 3"));

        // Voy a buscar a mis jugadores
        Danis = new List<Transform>();
        Danis.Add(transform.Find("Guardian Danis"));
        Danis.Add(transform.Find("Cazador Danis"));
        Danis.Add(transform.Find("Cazador Danis 2"));
        Danis.Add(transform.Find("Cazador Danis 3"));
        Danis.Add(transform.Find("Golpeador Danis"));
        Danis.Add(transform.Find("Golpeador Danis 2"));
        Danis.Add(transform.Find("Buscador Danis"));
		Teammates = Danis;

      


        // Le aviso al GameManager mi nombre de equipo y 
        // me regresa el número de equipo que me toca
        DanisTeamNumber =
            GameManager.instancia.SetTeamName(DanisName);

        // Ahora que sé el número de equipo
        if (DanisTeamNumber == 1)
        {
            //le puedo decir quienes son mis jugadores
            GameManager.instancia.team1Players = Danis;
            // Puedo saber hacia donde tiro
            rivalGoals = GameManager.instancia.team2Goals;
            myGoals = GameManager.instancia.team1Goals;


        }
        else if (DanisTeamNumber == 2)
        {
            GameManager.instancia.team2Players = Danis;

            rivalGoals = GameManager.instancia.team1Goals;
            myGoals = GameManager.instancia.team2Goals;
        }

        GameManager.instancia.SetTeamColor(DanisTeamNumber, myDanisColor);

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
            Danis[j].GetComponent<Player>().myNumberInTeam = j;
            Danis[j].GetComponent<Player>().myStartingPosition = myStartingPositions[j];
        }
        Danis[6].GetComponent<Player>().myNumberInTeam = 6;
        Danis[6].GetComponent<Player>().myStartingPosition = mySeekerStartingPosition;

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

		foreach (Transform Danis in Danis) 
		{
			dist = Vector3.Distance (Danis.position, GameManager.instancia.Quaffle.transform.position);
			if (dist < less) 
			{
				less = dist;
				ClosestTeammateToQuaffle = Danis;
                //(Danis);
			}
		}
	}

    public void FindClosestChaserToQuaffle()
    {

        float less = float.MaxValue;
        float dist;

        foreach (Transform chaser in DanisChasers)
        {
            dist = Vector3.Distance(chaser.position, GameManager.instancia.Quaffle.transform.position);
            if (dist < less)
            {
                less = dist;
                ClosestChaserToQuaffle = chaser;
                
            }
          
        }
       // //(ClosestChaserToQuaffle);
    }

    public Transform FindClosestChaserToGoal()
    {
        //("buscando cercano");
        float less = float.MaxValue;
        float dist;
        foreach (Transform chaser in DanisChasers)
        {
            dist = Vector3.Distance(chaser.position, rivalGoals[2].transform.position);
            if (dist < less)
            {
                less = dist;
                ClosestChaserToGoal = chaser;

            }

        }
        //(ClosestChaserToGoal);
        return ClosestChaserToGoal;
        
    }

    public GameObject FindClosestEnemyToQuaffle()
    {
        //("buscando cercano");
        float less = float.MaxValue;
        float dist;
        foreach (Transform chafas in Chafas)
        {
            dist = Vector3.Distance(chafas.position, GameManager.instancia.Quaffle.transform.position);
            if (dist < less)
            {
                less = dist;
               ClosestEnemyToQuaffle = chafas.gameObject;

            }

        }
        //(ClosestChaserToGoal);
        return ClosestEnemyToQuaffle;

    }

}
