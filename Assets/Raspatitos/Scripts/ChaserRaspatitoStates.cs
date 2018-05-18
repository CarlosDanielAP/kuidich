using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace ChaserRaspatitoStates
{
    public enum ChaserStateID // Aqui agreguen las claves de cada estado que quieran
    {
        ChaseBall,
        SearchGoal,
        ChaseRival
    }

    //=============================================================
    //=================================================== ChaseBall
    public class ChaseBall : State
    {
        private Player player;

        // Variables del estado

        public ChaseBall(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Tengo que buscar la Quaffle
            player.steering.Target = GameManager.instancia.Quaffle.transform;

            player.steering.seek = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Llegar hasta la pelota
            // Veo si estoy cerca de la Quaffle
            if(Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) < 6f)
            {
                // Estoy cerca de la pelota, checo si otro jugador tiene
                // posesión de ella
                if( ! GameManager.instancia.isQuaffleControlled() )
                {
                    // Si no esta controlada, yo puedo tomar posesión de ella
                    GameManager.instancia.ControlQuaffle(player.gameObject);

                    GameManager.instancia.
                        Quaffle.GetComponent<Quaffle>().
                            Control(player.transform);

                    // Cambiar de estado
                    ChangeState(ChaserStateID.SearchGoal);
                }
                else
                {
                    // Si ya está controlada por otro jugador, hacer algo...
                    // Si el que la controla es de mi equipo, puedo apoyarlo
                    if(player.myTeam.isTeammate(
                        GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()) )
                    {

                    }
                    // Si el que la controla es del equipo contrario
                    else
                    {
                        //ChangeState(ChaserStateID.ChaseRival);
                    }

                }
            }

            
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.seek = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }

    //=============================================================
    //=================================================== SearchGoal
    public class SearchGoal : State
    {
        private Player player;

        // Variables del estado

        public SearchGoal(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Se supone que tengo la pelota, entonces decido ir tras 
            // el aro del rival
            int aro = Random.Range(0, 2);
            player.steering.Target = GameManager.instancia.team1Goals[aro];

            player.steering.arrive = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Si ya me encuentro a cierta distancia el objetivo, 
            // puedo tirar al aro
            if(Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) < 15f) // calibrar
            {
                // estoy a rango de disparo
                GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                    Throw(
                        player.steering.Target.position - player.transform.position,
                        ((ChaserRaspatito)player).ThrowStrength
                    );

                // Ya no tengo posesión de la pelota
                GameManager.instancia.ControlQuaffle(null);

                // Cambiar de estado
                // pej. regresar a mi posicion inicial
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
    //=================================================== ChaseRival
    public class ChaseRival : State
    {
        private Player player;

        // Variables del estado

        public ChaseRival(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Tengo que buscar al rival que tiene la pelota
            player.steering.Target = GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner().transform;

            player.steering.seek = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Quizas la quaffle no tenga ya dueño
            if(player.steering.Target == null)
            {
                ChangeState(ChaserStateID.ChaseBall);
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.seek = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}