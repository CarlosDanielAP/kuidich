using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamStates
{
    public enum StateID // Aqui agreguen las claves de cada estado que quieran
    {
        Prepare,
        Defending,
        Attacking
    }

    //=============================================================
    //=================================================== Prepare
    public class Prepare : State
    {
        private Team team;

        // Variables del estado

        public Prepare(Team _team)
        {
            team = _team;
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