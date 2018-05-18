using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

[RequireComponent (typeof(SteeringCombined))]

public class Player : MonoBehaviour
{
    public FSM fsm;
    public SteeringCombined steering;



    // Variables del jugador
    public bool hitted = false;

    public Team myTeam;

    public float resistence = 0.5f;    // Players resistence to hits

    public int myNumberInTeam;
    public Transform myStartingPosition = null;

    // Que posicion tiene este jugador
    public enum PlayerPosition
    {
        Keeper,
        Beater,
        Chaser,
        Seeker
    };
    public PlayerPosition playerPosition;

    // Use this for initialization
    protected virtual void Start()
    {
        // Referencia a mi equipo para conocer a mis compañeros, mi cancha y los aros rivales
        myTeam = transform.parent.GetComponent<Team>();

        // Asignar el steering
        steering = GetComponent<SteeringCombined>();

        // Hay que hacer la fsm del agente
        fsm = new FSM(gameObject, this);

        // Crear los estados en que puede estar 
        GlobalMessageState global = new GlobalMessageState(this);
        Waiting wait = new Waiting(this);
        ReceiveHit hit = new ReceiveHit(this);

        // Estado global
        fsm.SetGlobalState(global);
        // Hay que agregarlos a la FSM
        fsm.AddState(StateID.Waiting, wait);
        fsm.AddState(StateID.ReceiveHit, hit);

        // Indicar cual es el estado inicial
        fsm.ChangeState(StateID.Waiting);
       
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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // Si me pega un rival o una pelota
        if(myTeam.isRival(collision.gameObject) || collision.gameObject.tag.Equals("Ball Bludger"))
        {
            // Me pegaron con suficiente fuerza
            if (collision.relativeVelocity.magnitude > 2) //calibrar
            {
                hitted = true;
            }
        }
    }
}
