using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalDanisStates;
using BeaterDanisStates;

public class BeaterDanis : Player
{

    protected override void Start()
    {
        base.Start();
        PrepareToPlay prepare = new PrepareToPlay(this);
        buscarBludgers buscar = new buscarBludgers(this);

        fsm.AddState(ChaserStateID.PrepareToPlay, prepare);
        fsm.AddState(ChaserStateID.buscarBludgers, buscar);


        fsm.ChangeState(ChaserStateID.PrepareToPlay);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
