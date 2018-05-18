using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamStates;

public class Team : MonoBehaviour
{
    public FSM fsm;
    
    // Variables del equipo
	public List<Transform> Teammates;  // Yo quiero tener a mis jugadores en una lista
	public List<Transform> Rivals;      // rivales
        
    // Use this for initialization
    protected virtual void Start()
    {
        // Hay que hacer la fsm del agente
        fsm = new FSM(gameObject, this);

        // Crear los estados en que puede estar 
        /*Cooking cook = new Cooking(this);
        Bathroom bath = new Bathroom(this);
        Housework work = new Housework(this);*/

        // Hay que agregarlos a la FSM
        /*fsm.AddState(StateID.Cooking, cook);
        fsm.AddState(StateID.Bathroom, bath);
        fsm.AddState(StateID.DoHousework, work);
       */
        // Indicar cual es el estado inicial
        //fsm.ChangeState(StateID.DoHousework);
       
        // Activo la fsm
        fsm.Activate();
    }

    protected virtual void Update()
    {
        if (fsm != null && fsm.IsActive())
        {
            fsm.UpdateFSM();
        }
    }

	public bool isTeammate(GameObject player)
	{
		return Teammates.Contains(player.transform);
	}

	public bool isRival(GameObject player)
	{
		return Rivals.Contains(player.transform);
	}
}
