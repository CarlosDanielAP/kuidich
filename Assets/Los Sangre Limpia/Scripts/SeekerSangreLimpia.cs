using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeekerSangreLimpiaStates;

public class SeekerSangreLimpia : Player
{

    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
        ChaseBall chase = new ChaseBall(this);
        GameOver gameover = new GameOver(this);
        Evade evade = new Evade(this);

        fsm.AddState(SeekerStateID.PrepareToPlay, prepare);
        fsm.AddState(SeekerStateID.ChaseBall, chase);
        fsm.AddState(SeekerStateID.GameOver, gameover);
        fsm.AddState(SeekerStateID.Evade, evade);

        fsm.ChangeState(SeekerStateID.PrepareToPlay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
