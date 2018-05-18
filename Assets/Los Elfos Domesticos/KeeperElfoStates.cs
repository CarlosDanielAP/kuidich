using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace KeeperElfoStates
{
    public enum KeeperStateID // Aqui agreguen las claves de cada estado que quieran
    {
       PrepareToPlay,
       Guard,
       Interpose,
       Pass
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
                ChangeState(KeeperStateID.Guard);
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
    //=================================================== Guard
    public class Guard : State
    {
        private Player player;
        int aro;
        int tiempo=0;
        // Variables del estado

        public Guard(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            //Buscar los aros/area a proteger
            aro = Random.Range(0, 2);
            

        }
        public override void Act(GameObject objeto)
        {

            player.steering.Target = GameManager.instancia.team1Goals[aro];
            player.steering.seek = true;
            // player.steering.wander = true;
            if (tiempo == 600)
            {
                aro = Random.Range(0, 2);
                tiempo = 0;
            }
            else
                tiempo++; ;
        }
        public override void Reason(GameObject objeto)
        {
           
            //si esta a cierta distacia de los aros entonces solo de vueltas por ahi con wander
            if (Vector3.Distance(player.transform.position, player.steering.Target.position) < 30f)
            {
                player.steering.wander = true; //aqui seria mejor manejar el peso
                player.steering.seek = false;
               
            }
            //si se aleja de cierta distancia cambiar a seek para volver a acercarse
            else if (Vector3.Distance(player.transform.position, player.steering.Target.position) > 30f)
            {
                player.steering.seek = true;
                player.steering.wander = false;
            }
            //No esta en posesion de algun jugador de mi equipo

			if (GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner() != null && !player.myTeam.isTeammate(
				GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
            //if (!player.myTeam.isTeammate(
                  //GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
            {
                //si detecta la pelota cerca entonces interponerse entre la pelota y el aro en un nuevo State
                if (Vector3.Distance(GameManager.instancia.team1Goals[0].gameObject.transform.position, GameManager.instancia.Quaffle.transform.position) < 40f)
                {
                    player.steering.Target = GameManager.instancia.team1Goals[0];
                    ChangeState(KeeperStateID.Interpose);
                }
                if (Vector3.Distance(GameManager.instancia.team1Goals[1].gameObject.transform.position, GameManager.instancia.Quaffle.transform.position) < 40f)
                {
                    player.steering.Target = GameManager.instancia.team1Goals[1];
                    ChangeState(KeeperStateID.Interpose);
                }
                if (Vector3.Distance(GameManager.instancia.team1Goals[2].gameObject.transform.position, GameManager.instancia.Quaffle.transform.position) < 40f)
                {
                    player.steering.Target = GameManager.instancia.team1Goals[2];
                    ChangeState(KeeperStateID.Interpose);
                }

            }


        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.seek = false;
            player.steering.wander = false;
        }

        IEnumerator IdleFunction()
        {
           
            yield return new WaitForSeconds(5f);

        }
    }

    //=============================================================
    //=================================================== Interpose
    public class Interpose : State
    {
        //mandar un mensaje a los otros jugadores los cazadores de que va interceptar el balon con la ayuda del script PlayerStates
        private Player player;
        int aro;
        
        // Variables del estado

        public Interpose(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.agent1 = player.steering.Target.gameObject;
            player.steering.agent2 = GameManager.instancia.Quaffle;
            //activar steering interpose
            player.steering.interpose = true;
        }
        public override void Act(GameObject objeto)
        {


        }
        public override void Reason(GameObject objeto)
        {
            //si el balon esta a cierta distancia  entonces lo toma
            if (Vector3.Distance(player.transform.position, GameManager.instancia.Quaffle.transform.position) < 30f)
            {
                // Si no esta controlada, yo puedo tomar posesión de ella
                if (GameManager.instancia.ControlQuaffle(player.gameObject))
                {
                    // Busco pasarsela a uno de mis compañeros porque tengo la pelota
                   ChangeState(KeeperStateID.Pass);
                }
            }
            else
                ChangeState(KeeperStateID.Guard);
               

        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.interpose = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);

        }
    }

    public class Pass : State
    {
        private Player player;
        int aro;
        // Variables del estado

        public Pass(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            //le pasa una direccion a la pelota hacia el jugador al que se la quiere pasar
            //busca si hay uno de sus compañeros cerca
            for (int j = 0; j < (int)player.GetComponentInParent<TeamLosChidos>().LosChidos.Count; j++)
            {
                if (Vector3.Distance(player.transform.right, player.GetComponentInParent<TeamLosChidos>().LosChidos[j].gameObject.transform.position) < 50f)//el jugador esta cerca
                {

                    for (int i = 0; i < (int)player.GetComponentInParent<TeamLosChidos>().Chafas.Count; i++)
                    {
                        //no esta cerca  de el un enemigo
                        if (Vector3.Distance(player.GetComponentInParent<TeamLosChidos>().LosChidos[j].gameObject.transform.position, player.GetComponentInParent<TeamLosChidos>().Chafas[i].gameObject.transform.position) < 50f)
                        {
                            player.steering.Target = player.GetComponentInParent<TeamLosChidos>().LosChidos[j].gameObject.transform;
                            player.steering.arrive = true;
                        }
                    }
                }
            }
        }
        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            // Si ya me encuentro a cierta distancia el objetivo, 
            // puedo tirarla a mi compañero
            if (Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) < 30f) // calibrar
            {
                // estoy a rango de disparo
                GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                    Throw(
                        player.steering.Target.position - player.transform.position,
                        ((ChaserChido)player).ThrowStrength
                    );

                // Ya no tengo posesión de la pelota
                GameManager.instancia.FreeQuaffle();

                //cambio a otro estado
                ChangeState(KeeperStateID.Guard);
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
}