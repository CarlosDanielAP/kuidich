using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamRaspatitos : Team
{
   

    public string LosChidosName = "Raspatitos";

    private int LosChidosTeamNumber;    // Me lo asigna el game manager

    public List<Transform> LosChidos;  // Yo quiero tener a mis jugadores en una lista
    public List<Transform> Chafas;      // rivales

    private GameObject QuaffleOwner;

    public List<Transform> rivalGoals;


	void Start () 
    {
        // Voy a buscar a mis jugadores
        LosChidos = new List<Transform>();
        LosChidos.Add(transform.Find("GuardianRaspatito"));
        LosChidos.Add(transform.Find("CazadorRaspatito"));
        LosChidos.Add(transform.Find("CazadorRaspatito2"));
        LosChidos.Add(transform.Find("CazadorRaspatito3"));
        LosChidos.Add(transform.Find("CazadorRaspatito4"));
        LosChidos.Add(transform.Find("GolpeadorRaspatito"));
        LosChidos.Add(transform.Find("GolpeadorRaspatito2"));
        LosChidos.Add(transform.Find("BuscadorRaspatito"));

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

            Chafas = GameManager.instancia.team2Players;
        }
        else if (LosChidosTeamNumber == 2)
        {
            GameManager.instancia.team2Players = LosChidos;

            rivalGoals = GameManager.instancia.team1Goals;

            Chafas = GameManager.instancia.team1Players;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		// Si no me pasaron los integrantes en el start
        if(Chafas.Count == 0)
        {
            if (LosChidosTeamNumber == 1)
            {
                Chafas = GameManager.instancia.team2Players;
            }
            else if (LosChidosTeamNumber == 2)
            {
                Chafas = GameManager.instancia.team1Players;
            }
        }
	}

    public bool isTeammate(GameObject player)
    {
        return LosChidos.Contains(player.transform);
    }

    public bool isRival(GameObject player)
    {
        return Chafas.Contains(player.transform);
    }

    public int BludgerNumber(GameObject player)
    {
        // El manager recibe el nombre de un equipo
        if (player.name=="GolpeadorRaspatito")
        {
             
            return 1;       
        }
        else if (player.name == "GolpeadorRaspatito2")
        {

            return 2;
        }

        return 0;
    }
   
}
