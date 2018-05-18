using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeeperDanisStates;

public class KeeperDanis : Player
{

    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
         getposition position = new getposition(this);
        porterear _porterear = new porterear(this);
        atajar _atajar = new atajar(this);

        fsm.AddState(KeeperStateID.PrepareToPlay, prepare);
        fsm.AddState(KeeperStateID.getposition, position);
        fsm.AddState(KeeperStateID.porterear, _porterear);
        fsm.AddState(KeeperStateID.atajar, _atajar);


        fsm.ChangeState(KeeperStateID.PrepareToPlay);


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
