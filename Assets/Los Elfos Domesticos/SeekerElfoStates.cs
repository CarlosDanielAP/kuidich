using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace SeekerElfoStates
{
    public enum SeekerStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ChaseBall,
        Grab
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
        float vel;
        
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
            vel = player.steering.maxSpeed;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Llegar hasta la snitch
                // Probar agarrarla
                if (GameManager.instancia.GrabSnitch(objeto))
                {
                    // la atrapé
                    ChangeState(SeekerStateID.Grab);
                    Debug.Log("La atrape, el juego termina");
                }
                else
                {
                    // no la atrapé
                    //Trato de , aumentando mi velocidad
                    player.steering.maxSpeed += 1f;//Aumenta su velocidad para asi intentar atraparla
                }
            
           
             if (Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) >= 80f)
             {
                 player.steering.maxSpeed = vel;
                 player.steering.seek = false;
                 player.steering.wander = true;
             }
             if (Vector3.Distance(
               player.transform.position,
               player.steering.Target.position) < 80f && Vector3.Distance(
               player.transform.position,
               player.steering.Target.position)>2f)
             {
                 if(player.steering.maxSpeed<30)
                 player.steering.maxSpeed +=.01f;
                 player.steering.seek = true;
                 player.steering.wander = false;
             }

        }
        public override void OnExit(GameObject objeto)
        {
           // player.steering.maxSpeed = vel;
            player.steering.seek = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// /////////////
    /// </summary>
    public class Grab : State
    {
        private Player player;

        public Grab(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.Target = GameManager.instancia.Snitch.transform;

            player.steering.wander = true;
            // la atrapé
           
            // Estoy cerca de la pelota, esto esta en el de quaffle pero lo puse aca para no cambiar el script general de la snitch
            player.steering.Target.GetComponent<Rigidbody>().velocity = Vector3.zero;//le quito la velocidad a la pelota
            player.steering.Target.GetComponent<Collider>().isTrigger = true;
            player.steering.Target.GetComponent<Rigidbody>().isKinematic = true;
            player.steering.Target.transform.parent = player.transform;
        }
        public override void Act(GameObject objeto)
        {
           
        }
        public override void Reason(GameObject objeto)
        {
           
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.wander = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }

}