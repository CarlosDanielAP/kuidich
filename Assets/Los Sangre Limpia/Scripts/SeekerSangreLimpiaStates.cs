using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerSangreLimpiaStates
{
    public enum SeekerStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ChaseBall,
        GameOver,
        Evade
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
                player.steering.Target.GetComponent<SteeringCombined>().maxSpeed = player.steering.maxSpeed;
                GameManager.instancia.GrabSnitch(objeto);
                player.steering.Target.GetComponent<SteeringCombined>().maxSpeed = 0;
                player.steering.Target.GetComponent<SteeringCombined>().maxForce = 0;
                player.steering.Target.GetComponent<Rigidbody>().isKinematic = true;

                //("Agarra Snitch");
                player.steering.Target.transform.parent = player.transform;

            }
            if(player.steering.NearRivals.Count > 0 || player.steering.NearTeammates.Count > 0)
            {
                ChangeState(SeekerStateID.Evade);
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
    //=================================================== End
    public class GameOver : State
    {
        private Player player;

        // Variables del estado

        public GameOver(Player _player)
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

        }
        public override void OnExit(GameObject objeto)
        {

        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
    public class Evade : State
    {
        private Player player;

        // Variables del estado

        public Evade(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.separation = true;
            player.steering.separationWeight = 1;
        }
        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            // si no tiene jugadores cerca
            if (player.steering.NearPlayers.Count == 0)
            {
                // Puede deambular por el campo
                ChangeState(SeekerStateID.ChaseBall);
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.separation = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }

}