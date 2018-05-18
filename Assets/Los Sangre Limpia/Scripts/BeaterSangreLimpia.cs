using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeaterSangreLimpiaStates;

public class BeaterSangreLimpia : Player
{
    public Transform target;
  
    protected override void Start()
    {
        base.Start();

        PrepareToPlay prepare = new PrepareToPlay(this);
        Protect chase = new Protect(this);
        Hit hit = new Hit(this);
        //GameOver gameOver = new GameOver(this);

        fsm.AddState(BeaterStateID.PrepareToPlay, prepare);
        fsm.AddState(BeaterStateID.Protect, chase);
        fsm.AddState(BeaterStateID.Hit, hit);
        //fsm.AddState(SeekerStateID.GameOver, gameOver);

        fsm.ChangeState(BeaterStateID.PrepareToPlay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
