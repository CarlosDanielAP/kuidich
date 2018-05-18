using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnitchStates;

public class Snitch : Ball
{



	// Use this for initialization
	protected override void Start ()
    {
        base.Start();

        steering = GetComponent<SteeringCombined>();

        fsm = new FSM(gameObject, this);

        Waiting wait = new Waiting(this);
        Hovering hover = new Hovering(this);
        Escaping escape = new Escaping(this);
        Wandering wander = new Wandering(this);

        fsm.AddState(SnitchStateID.Wandering, wander);
        fsm.AddState(SnitchStateID.Escaping, escape);
        fsm.AddState(SnitchStateID.Hovering, hover);

        fsm.ChangeState(SnitchStateID.Wandering);

        fsm.Activate();

    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();
	}
}
