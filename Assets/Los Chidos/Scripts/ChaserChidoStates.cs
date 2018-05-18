using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace ChaserChidoStates
{
    public enum ChaserStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ChaseBall,
        SearchGoal,
        ChaseRival,
        EscortTeammate
    }
    //=============================================================
    //=================================================== PrepareToPlay
    public class PrepareToPlay : State
    {
        private Player player;

        // Variables del estado

        public PrepareToPlay(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
        }
        public override void Act(GameObject objeto)
        {
            if(player.myStartingPosition != null)
            {
                // El jugador se pone en posición de inicio de juego
                player.steering.Target = player.myStartingPosition;
                player.steering.arrive = true;
            }
        }
        public override void Reason(GameObject objeto)
        {
            if(GameManager.instancia.isGameStarted())
            {
                ChangeState(ChaserStateID.ChaseBall);
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

            // Estoy tras de la pelota, checo si otro jugador tiene
            // posesión de ella
            if (!GameManager.instancia.isQuaffleControlled())
            {
                // Llegar hasta la pelota
                // Veo si estoy cerca de la Quaffle
                if (Vector3.Distance(
                    player.transform.position,
                    player.steering.Target.position) < 6f)
                {
                    // Si no esta controlada, yo puedo tomar posesión de ella
                    if (GameManager.instancia.ControlQuaffle(player.gameObject))
                    {
                        // Busco anotar porque tengo la pelota
                        ChangeState(ChaserStateID.SearchGoal);
                    }

                }
            }
            else
            {
                // Si ya está controlada por otro jugador, hacer algo...
                // Si el que la controla es de mi equipo, puedo apoyarlo
				if ((player.myTeam as TeamLosChidos).isTeammate(
                    GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
                {
                    ChangeState(ChaserStateID.EscortTeammate);
                }
                // Si el que la controla es del equipo contrario
                else
                {
                    ChangeState(ChaserStateID.ChaseRival);
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
			player.steering.Target = (player.myTeam as TeamLosChidos).rivalGoals[aro];

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
                player.steering.Target.position) < (player as ChaserChido).distanceToShoot) // calibrar
            {
                // estoy a rango de disparo
                GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                    Throw(
                        player.steering.Target.position - player.transform.position,
                        ((ChaserChido)player).ThrowStrength
                    );

                // Ya no tengo posesión de la pelota
                GameManager.instancia.FreeQuaffle();

                // Cambiar de estado
                // pej. regresar a mi posicion inicial
            }

            // Podría perder el control de pelota 
            if( ! GameManager.instancia.isQuaffleControlled())
            {
                ChangeState(ChaserStateID.ChaseBall);
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
            if (GameManager.instancia.isQuaffleControlled())
            {
                player.steering.Target = GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner().transform;

                player.steering.seek = true;
            }
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Quizas la quaffle no tenga ya dueño
            if(!GameManager.instancia.isQuaffleControlled())
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

    //=============================================================
    //=================================================== EscortTeammate
    public class EscortTeammate : State
    {
        private Player player;

        // Variables del estado

        public EscortTeammate(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Un compañero tiene la pelota, tratemos de acompañarlo en grupo
            player.steering.teamSeparation = true;
            player.steering.teamCohesion = true;
            player.steering.teamAlignment = true;

        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Quizás la pelota esté suelta, hay que buscarla
            if( ! GameManager.instancia.isQuaffleControlled())
            {
                ChangeState(ChaserStateID.ChaseBall);
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.teamSeparation = false;
            player.steering.teamCohesion = false;
            player.steering.teamAlignment = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}