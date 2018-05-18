using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaserDanisStates;
using GlobalDanisStates;

public class ChaserDanis : Player
{
    public float ThrowStrength;
    public float distanceToShoot;
    public GameObject Team;
    // Como hereda de player, ya tiene un FSM y un Steering

	protected override void Start ()
    {
        base.Start();

        // Agregar los estados de este agente, chaser
        PrepareToPlay prepare = new PrepareToPlay(this);
        Nada nada = new Nada(this);
        ChaseBall chase = new ChaseBall(this);
         SearchGoal search = new SearchGoal(this);
        // EscortTeammate escort = new EscortTeammate(this);
        ChaseRival rival = new ChaseRival(this);
        Distancia distancia = new Distancia(this);
        TakePosition take = new TakePosition(this);

        fsm.AddState(ChaserStateID.nada, nada);
        fsm.AddState(ChaserStateID.PrepareToPlay, prepare);
       fsm.AddState(ChaserStateID.ChaseBall, chase);
        fsm.AddState(ChaserStateID.TakePosition, take);
         fsm.AddState(ChaserStateID.SearchGoal, search);
       // fsm.AddState(ChaserStateID.EscortTeammate, escort);
        fsm.AddState(ChaserStateID.ChaseRival, rival);
        fsm.AddState(GlobalStateID.Distancia, distancia);

        fsm.ChangeState(ChaserStateID.PrepareToPlay);
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
	}
}
