using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    private int myTeam;
    public void setTeamOwner(int team)
    {
        myTeam = team;
    }

    private void OnTriggerEnter(Collider other)
    {
        // La quaffle pasa por el aro
        if(other.tag.Equals("Ball Quaffle"))
        {
            // El equipo contrario recibe puntos
            if(myTeam == 1)
            {
                GameManager.instancia.Score(2, 10);
            }
            else if(myTeam == 2)
            {
                GameManager.instancia.Score(1, 10);
            }
            Instantiate(GameManager.instancia.ScoreFX, transform.position, Quaternion.identity);
        }

        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
