using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeaterRaspatitoStates;

public class BeaterRaspatito: Player
{
   
   
    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Agregar los estados de este agente, chaser
        Idle idle = new Idle(this);
        Hit hit = new Hit(this);

        fsm.AddState(BeaterStateID.Idle, idle);
        fsm.AddState(BeaterStateID.Hit, hit);

        fsm.ChangeState(BeaterStateID.Idle);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }


}
