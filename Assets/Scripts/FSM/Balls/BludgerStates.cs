using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BludgerStates
{
    public enum BludgerStateID // Aqui agreguen las claves de cada estado que quieran
    {
        Prepare,
        SelectTarget,
        ChaseTarget
    }

    //=============================================================
    //=================================================== Prepare
    public class Prepare : State
    {
        private Ball ball;

        // Variables del estado


        public Prepare(Ball _ball)
        {
            ball = _ball;
        }
        public override void OnEnter(GameObject objeto)
        {
            ball.steering.enabled = false;
            objeto.GetComponent<Rigidbody>().velocity = Vector3.up * 50f;
        }
        public override void Act(GameObject objeto)
        {
            
        }
        public override void Reason(GameObject objeto)
        {
            if (objeto.transform.position.y > 150f)
                objeto.GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (GameManager.instancia.isGameStarted())
                ChangeState(BludgerStateID.SelectTarget);
        }
        public override void OnExit(GameObject objeto)
        {
            ball.steering.enabled = true;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }

    //=============================================================
    //=================================================== SelectTarget
    public class SelectTarget : State
    {
        private Ball ball;

        // Variables del estado
        int playerNum;
        int teamNum;

        public SelectTarget(Ball _ball)
        {
            ball = _ball;
        }
        public override void OnEnter(GameObject objeto)
        {
        }
        public override void Act(GameObject objeto)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            if (((Bludger)ball).GetLastBeaterIntervention() == 1)
            {
                teamNum = 1;
                ((Bludger)ball).TurnOffBeaterIntervention();
            }
            else if (((Bludger)ball).GetLastBeaterIntervention() == 2)
            {
                teamNum = 0;
                ((Bludger)ball).TurnOffBeaterIntervention();
            }
            else
            {
                teamNum = Random.Range(0, 2);
            }
            playerNum = Random.Range(0, 7);

            if (teamNum == 0)
            {
                ball.steering.Target = GameManager.instancia.team1Players[playerNum];
            }
            else if (teamNum == 1)
            {
                ball.steering.Target = GameManager.instancia.team2Players[playerNum];
            }
        }
        public override void Reason(GameObject objeto)
        {
            if (ball.steering.Target != null)
                ChangeState(BludgerStateID.ChaseTarget);
        }
        public override void OnExit(GameObject objeto)
        {
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
    //=============================================================
    //=================================================== ChaseTarget
    public class ChaseTarget : State
    {
        private Ball ball;

        // Variables del estado

        public ChaseTarget(Ball _ball)
        {
            ball = _ball;
        }
        public override void OnEnter(GameObject objeto)
        {
            ball.steering.seek = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Preguntar si golpeó a un jugador, entonces
            // que elija otro
            if( ((Bludger)ball).HitPlayer() )
            {
                ((Bludger)ball).ResetPlayerHitted();
                ChangeState(BludgerStateID.SelectTarget);
            }

            // Preguntamos si un golpeador (Beater) intercepta o le pega 
            // a la bludger
            if(((Bludger)ball).GetLastBeaterIntervention()!=0)
            {
                ChangeState(BludgerStateID.SelectTarget);
            }

        }
        public override void OnExit(GameObject objeto)
        {
            ball.steering.seek = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
