using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaserRaspatitoStates;

public class ChaserRaspatito : Player
{
    public float ThrowStrength;

    // Como hereda de player, ya tiene un FSM y un Steering

    protected override void Start()
    {
        base.Start();

        // Agregar los estados de este agente, chaser
        ChaseBall chase = new ChaseBall(this);
        SearchGoal search = new SearchGoal(this);

        fsm.AddState(ChaserStateID.ChaseBall, chase);
        fsm.AddState(ChaserStateID.SearchGoal, search);

        fsm.ChangeState(ChaserStateID.ChaseBall);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
