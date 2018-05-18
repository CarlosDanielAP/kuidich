using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;
using GlobalDanisStates;
namespace BeaterDanisStates { 
    public enum ChaserStateID
    {
        //aqui van las claves
        PrepareToPlay,
        buscarBludgers
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
            //("hola");
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
                 ChangeState(ChaserStateID.buscarBludgers);
              
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
    //=================================================== buscarbuldgers
    public class buscarBludgers : State
    {
        private Player player;
        private Transform bludger;

        // Variables del estado

        public buscarBludgers(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
          
           if(this.player.name== "Golpeador Danis")
            {
               bludger = GameManager.instancia.Bludger[0].transform;
                player.steering.Target = bludger;

            }
            if (this.player.name == "Golpeador Danis 2")
            {
                bludger = GameManager.instancia.Bludger[1].transform;
                player.steering.Target = bludger;
            }

            player.steering.pursuit = true;
        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
           
            //si la distancia es corta lo golpeo
            if (Vector3.Distance(
                   player.transform.position,
                   bludger.transform.position) < 6f)
            {

                //SI ALGUIEN TIENE LA QUAFFLE LE MANDO LA BLUDGER A EL
                if (!(player.myTeam as TeamDanis).isTeammate(GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
                {

                    

                    bludger.GetComponent<Bludger>().BeaterIntervention(player.myTeam.GetComponent<TeamDanis>().FindClosestEnemyToQuaffle());
                }
                //("sakeseeeeeeeeeeeeee");
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.pursuit = false;
        }

        }
        }
