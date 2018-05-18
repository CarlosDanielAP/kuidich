using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallStates
{
    public enum StateID // Aqui agreguen las claves de cada estado que quieran
    {
        Waiting
    }

    //=============================================================
    //=================================================== Waiting
    public class Waiting : State
    {
        private Ball ball;

        // Variables del estado

        public Waiting(Ball _ball)
        {
            ball = _ball;
        }
        public override void OnEnter(GameObject objeto)
        {
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
        }
        public override void OnExit(GameObject objeto)
        {
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}