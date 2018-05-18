using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;
using ChaserDanisStates;


namespace GlobalDanisStates
{
    public enum GlobalStateID
    {
        Distancia
    }

    public class Distancia : State
    {
        private Player player;
       
      
        public Distancia(Player _player)
        {
            player = _player;
        }

        public override void OnEnter(GameObject objeto)
        {
          
            
            player.myTeam.GetComponent<TeamDanis>().FindClosestChaserToQuaffle();
            Debug.Log(player.myTeam.GetComponent<TeamDanis>().ClosestChaserToQuaffle);
          

        }
        public override void Act(GameObject objeto)
        {
           
        }

        public override void Reason(GameObject objeto)
        {

            //si es el mas cercano va a buscar la pelota.
            if (player.myTeam.GetComponent<TeamDanis>().ClosestChaserToQuaffle == objeto.transform)
            {
                Debug.Log("soy yo");
                ChangeState(ChaserStateID.ChaseBall);
            }

            else
            {
                ChangeState(ChaserStateID.TakePosition);
            }

        }

        public override void OnExit(GameObject objeto)
        {
        }

    }
}