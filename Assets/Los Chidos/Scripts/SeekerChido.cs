using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeekerChidoStates;

public class SeekerChido : Player
{

    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
        ChaseBall chase = new ChaseBall(this);

        fsm.AddState(SeekerStateID.PrepareToPlay, prepare);
        fsm.AddState(SeekerStateID.ChaseBall, chase);

        fsm.ChangeState(SeekerStateID.PrepareToPlay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
