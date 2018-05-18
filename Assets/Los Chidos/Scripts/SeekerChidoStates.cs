using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerChidoStates
{
    public enum SeekerStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ChaseBall
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
                ChangeState(SeekerStateID.ChaseBall);
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
            // Tengo que buscar la Snitch
            player.steering.Target = GameManager.instancia.Snitch.transform;

            player.steering.seek = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Llegar hasta la snitch
            // Veo si estoy cerca
            if (Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) <= 2f)
            {
                // Probar agarrarla
                if (GameManager.instancia.GrabSnitch(objeto))
                {
                    // la atrapé
                }
                else
                {
                    // no la atrapé
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

    
}