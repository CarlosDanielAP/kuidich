using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaserChidoStates;

public class ChaserChido : Player
{
    public float ThrowStrength;
    public float distanceToShoot;

    // Como hereda de player, ya tiene un FSM y un Steering

	protected override void Start ()
    {
        base.Start();

        // Agregar los estados de este agente, chaser
        PrepareToPlay prepare = new PrepareToPlay(this);
        ChaseBall chase = new ChaseBall(this);
        SearchGoal search = new SearchGoal(this);
        EscortTeammate escort = new EscortTeammate(this);
        ChaseRival rival = new ChaseRival(this);

        fsm.AddState(ChaserStateID.PrepareToPlay, prepare);
        fsm.AddState(ChaserStateID.ChaseBall, chase);
        fsm.AddState(ChaserStateID.SearchGoal, search);
        fsm.AddState(ChaserStateID.EscortTeammate, escort);
        fsm.AddState(ChaserStateID.ChaseRival, rival);

        fsm.ChangeState(ChaserStateID.PrepareToPlay);
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
	}
}
