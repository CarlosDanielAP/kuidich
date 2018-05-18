using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public enum StateID // Aqui agreguen las claves de cada estado que quieran
    {
        GlobalMessageState,
        Waiting,
        ReceiveHit
    }

    //=============================================================
    //=================================================== GlobalMessageState
    /// <summary>
    /// Hay muchas condiciones en todos los estados de los jugadores que se repiten, para evitar
    /// esto el estado GlobalMessage se encargará de verificar estas condiciones todo el tiempo.
    /// No es un estado al que se pueda cambiar, más bien es una especie de controlador.
    /// Ver FSM, ya que se modificó un poco para permitir esto.
    /// </summary>
    public class GlobalMessageState : State
    {
        private Player player;

        // Variables del estado

        public GlobalMessageState(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            

            // Si el jugador es golpeado
            if (player.hitted && !isCurrentState(StateID.ReceiveHit))
            {
                // Cambio al estado que me "estunea"
                InitBlipState(StateID.ReceiveHit);
            }

            // Alguien acaba de anotar
            int scoringTeam = GameManager.instancia.IsRecovering();
            if (scoringTeam != 0)
            {

            }

            // Si el juego termina
            if(GameManager.instancia.isGameOver())
            {
                ChangeState(StateID.Waiting);
            }
        }
        public override void OnExit(GameObject objeto)
        {
        }
    }

    //=============================================================
    //=================================================== Waiting
    public class Waiting : State
    {
        private Player player;

        // Variables del estado

        public Waiting(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            objeto.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
    //=============================================================
    //=================================================== ReceiveHit
    public class ReceiveHit : State
    {
        private Player player;

        // Variables del estado
        bool stunEnds;
        bool loseControl;   // El jugador tenia la pelota, la puede perder

        public ReceiveHit(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // En este estado el jugador recibe un golpe

            loseControl = false;
            stunEnds = false;
            fsm.myMono.StartCoroutine(StunFunction());
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            if (stunEnds)
                RevertBlipState();
        }
        public override void OnExit(GameObject objeto)
        {
            // Ya sea que haya sido incapacitado o no, me aseguro de que su steering esté activo
            player.GetComponent<SteeringCombined>().enabled = true;

            player.hitted = false;
        }

        IEnumerator StunFunction()
        {
            // NO siempre que sea golpeado queda incapacitado o pierde el control de la pelota
            float lose = Random.value;
            if(lose > player.resistence)
            {
                // Si tenia la pelota
                GameObject owner = GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner();
                if (owner != null)
                {
                    if (owner.Equals(player.gameObject))
                    {
                        // Pierde control de la pelota
                        GameManager.instancia.FreeQuaffle();
                    }
                }
                // Lo desbalancea un instante
                player.GetComponent<SteeringCombined>().enabled = false;

                yield return new WaitForSeconds(2f);
            }
            
            stunEnds = true;
        }
    }
}