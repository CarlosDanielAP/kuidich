using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeekerElfoStates;

public class SeekerElfo : Player
{

    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
        ChaseBall chase = new ChaseBall(this);
        Grab grab = new Grab(this);

        fsm.AddState(SeekerStateID.PrepareToPlay, prepare);
        fsm.AddState(SeekerStateID.ChaseBall, chase);
        fsm.AddState(SeekerStateID.Grab, grab);

        fsm.ChangeState(SeekerStateID.PrepareToPlay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
