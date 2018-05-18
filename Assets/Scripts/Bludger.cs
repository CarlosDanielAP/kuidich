using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BludgerStates;

public class Bludger : Ball
{

    protected override void Start()
    {
        base.Start();

        steering = GetComponent<SteeringCombined>();

        fsm = new FSM(gameObject, this);

        Prepare prepare = new Prepare(this);
        SelectTarget select = new SelectTarget(this);
        ChaseTarget chase = new ChaseTarget(this);

        fsm.AddState(BludgerStateID.Prepare, prepare);
        fsm.AddState(BludgerStateID.SelectTarget, select);
        fsm.AddState(BludgerStateID.ChaseTarget, chase);

        fsm.ChangeState(BludgerStateID.Prepare);

        fsm.Activate();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private bool playerHitted = false;
    public bool HitPlayer()
    {
        return playerHitted;
    }
    public void ResetPlayerHitted()
    {
        playerHitted = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Le pegamos a un jugador
        if(GameManager.instancia.team1Players.Contains(collision.transform) ||
            GameManager.instancia.team2Players.Contains(collision.transform))
        {
            playerHitted = true;
        }
    }

    public Transform GetTarget()
    {
        return steering.Target;
    }

    private int lastBeaterIntervention;
    public int GetLastBeaterIntervention()
    {
        return lastBeaterIntervention;
    }
    public void TurnOffBeaterIntervention()
    {
        lastBeaterIntervention = 0;
    }
    /// <summary>
    /// Un golpeador (beater) debería llamar en algún lado a esta función
    /// para rechazar la bludger hacia un rival.
    /// Si no lo hace, solo será golpeado por la bludger.
    /// </summary>
    /// <param name="player"></param>
    public void BeaterIntervention(GameObject player)
    {
        if(GameManager.instancia.team1Players.Contains(player.transform))
        {
            lastBeaterIntervention = 1;
        }
        if(GameManager.instancia.team2Players.Contains(player.transform))
        {
            lastBeaterIntervention = 2;
        }
    }
}
