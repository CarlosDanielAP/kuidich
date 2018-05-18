using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FinderRaspatitoStates;

public class FinderRaspatito : Player
{
    //public float ThrowStrength;
    private bool helpMe;

    // Como hereda de player, ya tiene un FSM y un Steering
   
    protected override void Start()
    {
        base.Start();
        helpMe = false;
      

        // Agregar los estados de este agente, chaser
        Wander wander = new Wander(this);
        PursuitSnitch pursuitS = new PursuitSnitch(this);
        EvadeBludger evade_ = new EvadeBludger(this);

        fsm.AddState(FinderStateID.Wander, wander);
        fsm.AddState(FinderStateID.PursuitSnitch, pursuitS);
        fsm.AddState(FinderStateID.EvadeBludger, evade_);

        fsm.ChangeState(FinderStateID.Wander);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public bool save()
    {
        return helpMe;
    }

    public void help(bool a)
    {
        helpMe = a;
    }
}
