using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;
using GlobalDanisStates;

namespace ChaserDanisStates
{
    public enum ChaserStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ChaseBall,
        SearchGoal,
        ChaseRival,
        EscortTeammate,
        TakePosition,
        nada
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
            Debug.Log("hola");
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
                // ChangeState(ChaserStateID.ChaseBall);
                ChangeState(GlobalStateID.Distancia);
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

            //player.steering.seek = true;
            player.steering.pursuit = true;
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
                    if (!GameManager.instancia.isQuaffleControlled())
                    {
                        // Si no esta controlada, yo puedo tomar posesión de ella
                        GameManager.instancia.ControlQuaffle(player.gameObject);

                        GameManager.instancia.
                            Quaffle.GetComponent<Quaffle>().
                                Control(player.transform);
                        // Busco anotar porque tengo la pelota
                        ChangeState(ChaserStateID.SearchGoal);
                    }

                }
            }
            else
            {
                // Si ya está controlada por otro jugador, hacer algo...
                // Si el que la controla es de mi equipo, puedo apoyarlo
                if ((player.myTeam as TeamDanis).isTeammate(
                    GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
                {
                     ChangeState(ChaserStateID.TakePosition);
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
            player.steering.pursuit = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }

    /// ////////////////take position/////////////////
    public class TakePosition : State
    {
        private Player player;

        // Variables del estado

        public TakePosition(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
         

            Debug.Log("porteria");
            int aro = Random.Range(0, 2);
            player.steering.Target = (player.myTeam as TeamDanis).rivalGoals[aro];

            player.steering.arrive = true;
        }

        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            //si me diero un pase escucho el evento pase
            if (player.myTeam.GetComponent<TeamDanis>().ClosestChaserToGoal == this.player.transform)
            {
                Debug.Log("voy por ella" + player.myTeam.GetComponent<TeamDanis>().ClosestChaserToGoal);
                ChangeState(ChaserStateID.ChaseBall);
            }

            //si la pelota la tiene el otro equipo voy por ella
            if (!(player.myTeam as TeamDanis).isTeammate(GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
            {
                Debug.Log("latienen los otros");
                ChangeState(ChaserStateID.ChaseRival);

            }
            }
        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
        }


    }


    /////////////search Goal//////////////
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
            player.steering.nearPlayersSensorRadius = 50;
            int aro = Random.Range(0, 2);
            player.steering.Target = (player.myTeam as TeamDanis).rivalGoals[aro];

            player.steering.arrive = true;
        }
        public override void Act(GameObject objeto)
        {


        }
        public override void Reason(GameObject objeto)
        {
            // si tiene a mas de cierto numero de jugadores 
            //a su alrededor hacer pase a otro jugador
            if (player.steering.NearRivals.Count >=1)
            {

                fsm.myMono.StartCoroutine(PaseFunction());
              
            }

            //si la pelota la tiene el otro equipo voy por ella
            if (!(player.myTeam as TeamDanis).isTeammate(GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
            {
                ChangeState(ChaserStateID.ChaseBall);
            }
        }
        public override void OnExit(GameObject objeto)
        {
           fsm.myMono.StopAllCoroutines();

            player.steering.arrive = false;
            player.steering.nearPlayersSensorRadius = 10;
        }

        IEnumerator PaseFunction()
        {
            yield return new WaitForSeconds(3f);
            Debug.Log("pase bola"+this.player.transform);
            ((ChaserDanis)player).ThrowStrength =50;
            // el nuevo target es el jugador   si soy yo  sigue siendo la porteria
            if (player.myTeam.GetComponent<TeamDanis>().FindClosestChaserToGoal() != this.player.transform)
            {
                
                player.steering.Target = player.myTeam.GetComponent<TeamDanis>().FindClosestChaserToGoal();
                //EventManagerDanis.TriggerEvent("pase");
            }

            GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                Throw(
                    player.steering.Target.position - player.transform.position,
                    ((ChaserDanis)player).ThrowStrength);

            GameManager.instancia.FreeQuaffle();

            ChangeState(ChaserStateID.nada);

        }
    }


    /////////////nada//////////////
    public class Nada : State
    {
        private Player player;

        // Variables del estado

        public Nada(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            Debug.Log("nadaaaaaaaaaaaaaa00");
            //cuando tira entra en wander
            player.steering.wander = true;

        }
        public override void Act(GameObject objeto)
        {


        }
        public override void Reason(GameObject objeto)
        {
           
            // si la pelota esta libre mas de sierto tiempo voy a buscarla y si  soy el mas cercano
            if (!GameManager.instancia.isQuaffleControlled())
            {
                fsm.myMono.StartCoroutine(waitFunction());
            }

            }
        public override void OnExit(GameObject objeto)
        {
            fsm.myMono.StopAllCoroutines();
            player.steering.wander = false;
        }
        IEnumerator waitFunction()
        {
            yield return new WaitForSeconds(2f);
            //si la pelota la tiene uno de mi equipo tomo mi posicion
            if ((player.myTeam as TeamDanis).isTeammate(GameManager.instancia.Quaffle.GetComponent<Quaffle>().CurrentBallOwner()))
            {
                ChangeState(ChaserStateID.TakePosition);
            }

            // si la pelota la tiene un rival voy a atacar
            else
            {
                ChangeState(ChaserStateID.ChaseRival);
            }

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
            if (!GameManager.instancia.isQuaffleControlled())
            {
                ChangeState(GlobalStateID.Distancia);
            }
        }

        public override void OnExit(GameObject objeto)
        {
            player.steering.seek = false;
        }
        }
        }