using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeeperElfoStates;

public class KeeperElfo : Player
{

    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
        Guard guard = new Guard(this);
        Interpose inter = new Interpose(this);
       // Pass pass = new Pass(this);

        fsm.AddState(KeeperStateID.PrepareToPlay, prepare);
        fsm.AddState(KeeperStateID.Guard, guard);
        fsm.AddState(KeeperStateID.Interpose, inter);
       // fsm.AddState(KeeperStateID.Pass, pass);

        fsm.ChangeState(KeeperStateID.PrepareToPlay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
