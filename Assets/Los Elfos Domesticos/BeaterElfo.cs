using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeaterElfoStates;

public class BeaterElfo: Player
{

	public float ThrowStrength;



	// Como hereda de player, ya tiene un FSM y un Steering

	protected override void Start ()
	{
		base.Start();

		// Agregar los estados de este agente, chaser
		PrepareToPlay prepare = new PrepareToPlay(this);
		ProtectTeammate protect = new ProtectTeammate(this);
		EscortTeammate escort = new EscortTeammate(this);
		ScoutArea scout = new ScoutArea (this);

		fsm.AddState(BeaterStateID.PrepareToPlay, prepare);
		fsm.AddState(BeaterStateID.ProtectTeammate, protect);
		fsm.AddState(BeaterStateID.EscortTeammate, escort);
		fsm.AddState (BeaterStateID.ScoutArea, scout);

		fsm.ChangeState(BeaterStateID.PrepareToPlay);
	}

	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
	}
}
