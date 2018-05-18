using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;


namespace KeeperDanisStates
{
    public enum KeeperStateID
    {
        //aqui van las claves
        PrepareToPlay,
        getposition,
        porterear,
        atajar
    }

    //=============================================================
    //=================================================== PrepareToPlay
    public class PrepareToPlay : State
    {
        private Player player;
        public Transform incauto;

        // Variables del estado

        public PrepareToPlay(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            Debug.Log("hola");
        }
        public override void Act(GameObject objeto)
        {
            if (player.myStartingPosition != null)
            {
                // El jugador se pone en posición de inicio de juego
                player.steering.Target = player.myStartingPosition;
                player.steering.arrive = true;
            }
        }
        public override void Reason(GameObject objeto)
        {
            if (GameManager.instancia.isGameStarted())
            {
                ChangeState(KeeperStateID.getposition);

            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }


    //=============================================================
    //=================================================== get position
    public class getposition : State
    {
        private Player player;


        // Variables del estado

        public getposition(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.arrive = true;
            player.steering.Target = (player.myTeam as TeamDanis).myGoals[1];

        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            if (Vector3.Distance(player.transform.position,
               player.steering.Target.position) < 30f)
            {
                player.steering.arrive = false;
                ChangeState(KeeperStateID.porterear);

            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
        }
        }

    //=============================================================
    //=================================================== porterear
    public class porterear : State
    {
        private Player player;

        private Transform neargoal;
        // Variables del estado

        public porterear(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.arrive = true ;
           

        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            float less = float.MaxValue;
            float dist;

            if (Vector3.Distance(player.transform.position,
               GameManager.instancia.Quaffle.transform.position) < 80f)
            {
                Debug.Log("ahi viene");
                ChangeState(KeeperStateID.atajar);
            }
            foreach (Transform goal in (player.myTeam as TeamDanis).myGoals)
            {
                dist = Vector3.Distance(goal.position, GameManager.instancia.Quaffle.transform.position);
                if (dist < less)
                {
                    less = dist;
                    neargoal = goal;

                }

            }
            player.steering.Target = neargoal;
            

        }

        //si la distancia es muy corta atajar


        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
        }
    }

    //=============================================================
    //=================================================== atajar
    public class atajar : State
    {
        private Player player;

        private Transform neargoal;
        // Variables del estado

        public atajar(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.interpose = true;


        }
        public override void Act(GameObject objeto)
        {


        }
        public override void Reason(GameObject objeto)
        {
            float less = float.MaxValue;
            float dist;
            foreach (Transform goal in (player.myTeam as TeamDanis).myGoals)
            {
                dist = Vector3.Distance(goal.position, GameManager.instancia.Quaffle.transform.position);
                if (dist < less)
                {
                    less = dist;
                    neargoal = goal;

                }

            }
            player.steering.Target = neargoal;

            player.steering.agent1 = neargoal.gameObject;
            player.steering.agent2 = GameManager.instancia.Quaffle;

            if (Vector3.Distance(
                   player.transform.position,
                   player.steering.Target.position) < 6f)
            {
                if (!GameManager.instancia.isQuaffleControlled())
                {
                    // Si no esta controlada, yo puedo tomar posesión de ella
                    GameManager.instancia.ControlQuaffle(player.gameObject);

                    GameManager.instancia.
                        Quaffle.GetComponent<Quaffle>().
                            Control(player.transform);
                    player.steering.Target = (player.myTeam as TeamDanis).rivalGoals[1];

                   ((ChaserDanis)player).ThrowStrength = 50;
                    GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                   Throw(
                       player.steering.Target.position - player.transform.position,
                       ((ChaserDanis)player).ThrowStrength);

                    GameManager.instancia.FreeQuaffle();

                    ChangeState(KeeperStateID.porterear);
                }
            }

            if (Vector3.Distance(
                  player.transform.position,
                  player.steering.Target.position) < 6f || (Vector3.Distance(
                   player.transform.position,
                   player.steering.Target.position) > 6f))
            {
                ChangeState(KeeperStateID.porterear);

            }

            }

        //si la distancia es muy corta atajar
        public override void OnExit(GameObject objeto)
        {
            player.steering.interpose = false;
        }
    }
}