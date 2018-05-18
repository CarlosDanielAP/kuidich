using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace FinderRaspatitoStates
{
    public enum FinderStateID // Aqui agreguen las claves de cada estado que quieran
    {
        Wander,
        EvadeBludger,
        PursuitSnitch
    }

    public class Wander : State
    {
        private Player player;
        float distBludger1;
        float distBludger2;
        //Finder saveMe;
        bool ayuda;
        

        public Wander(Player _player)
        {
            player = _player;
            ayuda = false;
        }
        public override void OnEnter(GameObject objeto)
        {
            Debug.Log("Wander");
            
            player.steering.wander = true;
            distBludger1=0f;
            distBludger2=0f;
        }
        public override void Act(GameObject objeto)
        {
            
        }
        public override void Reason(GameObject objeto)
        {
            if (Vector3.Distance(player.transform.position,
                GameManager.instancia.Snitch.transform.position)<100f)
            {
                player.steering.Target = GameManager.instancia.Snitch.transform;
                ChangeState(FinderStateID.PursuitSnitch);
            }


            if (Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[0].transform.position) < Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[1].transform.position) && 
                Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[0].transform.position) < 10f)
             {
                ayuda = true;
                //saveMe.help(ayuda);
                 player.steering.Target = GameManager.instancia.Bludger[0].transform;
                 ChangeState(FinderStateID.EvadeBludger);
             }
             else if (Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[1].transform.position )> Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[0].transform.position) && Vector3.Distance(player.transform.position,
                GameManager.instancia.Bludger[1].transform.position) < 10f)
             {
                ayuda = true;
                //saveMe.help(ayuda);
                player.steering.Target = GameManager.instancia.Bludger[1].transform;
                 ChangeState(FinderStateID.EvadeBludger);
             }
        }
        public override void OnExit(GameObject _object)
        {
            Debug.Log("Sali de Wander");
            player.steering.wander = false;
        }
    }
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////PURSUITSNITCH
    /// </summary>
    public class PursuitSnitch : State
    {
        private Player player;

        public PursuitSnitch(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
           // player.steering.Target = GameManager.instancia.Snitch.transform;
            player.steering.pursuit = true;
            player.steering.maxSpeed = 30f;
            player.steering.maxForce = 5f;
            Debug.Log("Estoy persiguiendo a la snitch");
        }
        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            
            if(Vector3.Distance(player.transform.position,player.steering.Target.position)<2f)
            {
               // GameManager.instancia.Snitch.GetComponent
               //Aqui la agarras
            }
            //Instrucciones para tomar la snitch

        }
        public override void OnExit(GameObject _object)
        {
            player.steering.pursuit = false;
        }
    }
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////EVADEBLUDGER
    /// </summary>
    public class EvadeBludger : State
    {
        private Player player;

        public EvadeBludger(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
           // player.steering.Target = GameManager.instancia.Snitch.transform;
            player.steering.flee = true;
        }
        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            Debug.Log("Estoy huyendo");
        }
        public override void OnExit(GameObject _object)
        {
            player.steering.flee = false;
        }
    }
}