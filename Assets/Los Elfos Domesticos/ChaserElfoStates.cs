using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace ChaserElfoStates
{
    public enum ChaserStateID // Aqui agreguen las claves de cada estado que quieran
    {
        ChaseBall,
        SearchGoal,
        Support,
        Defense,
        Wait
    }

    //=============================================================
    //=================================================== ChaseBall
    public class ChaseBall : State
    {
        private Player player;

        // Variables del estado

        private bool quaff;
        public ChaseBall(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Tengo que buscar la Quaffle
            player.steering.Target = GameManager.instancia.Quaffle.transform;

            player.steering.seek = true;
            quaff = false;

			EventManagerElfo.StartListening(Events.Quaffle, Quaffle);
        }

        void Quaffle()
        {
            quaff = true;
        }

        public override void Act(GameObject objeto)
        {

        }

        public override void Reason(GameObject objeto)
        {

            if (quaff)
            {
                ChangeState(ChaserStateID.Support);
            }

            // Llegar hasta la pelota
            // Veo si estoy cerca de la Quaffle
            if (Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) < 5f)
            {
                // Estoy cerca de la pelota, checo si otro jugador tiene
                // posesión de ella
                if (!GameManager.instancia.isQuaffleControlled())
                {
                    // Si no esta controlada, yo puedo tomar posesión de ella
                    GameManager.instancia.ControlQuaffle(player.gameObject);

                    GameManager.instancia.
                        Quaffle.GetComponent<Quaffle>().
                            Control(player.transform);

                    // Cambiar de estado
                    ChangeState(ChaserStateID.SearchGoal);
                }
                else
                {
                    // Si ya está controlada por otro jugador, hacer algo...
                }
            }

            //Que pasa si el jugador es golpeado
            if (player.hitted)
            {
                //Cambio el aestado que me stunea
                InitBlipState(StateID.ReceiveHit);
            }

        }
        public override void OnExit(GameObject objeto)
        {
			EventManagerElfo.StopListening(Events.Quaffle, Quaffle);
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
        float lastTime, t = 10f;
        bool th;

        public SearchGoal(Player _player)
        {
            player = _player;


        }
        public override void OnEnter(GameObject objeto)
        {
            // Se supone que tengo la pelota, entonces decido ir tras 
            // el aro del rival
            int aro = Random.Range(0, 2);
            th = false;

			switch (player.transform.parent.GetComponent<TeamElfosDomesticos>().getTeamNumber())
            {
                case 1:

                    player.steering.Target = GameManager.instancia.team2Goals[aro];
                    break;

                case 2:
                    player.steering.Target = GameManager.instancia.team1Goals[aro];
                    break;

                default:
                    break;
            }


            player.steering.arrive = true;


			EventManagerElfo.TriggerEvent("Quaffle");
            lastTime = Time.timeSinceLevelLoad;

        }



        public override void Act(GameObject objeto)
        {
            
            if (Time.timeSinceLevelLoad > lastTime + t)
            {

				throwBall(player.transform.parent.GetComponent<TeamElfosDomesticos>().SupportPlayer().transform.position);
                th = true;
            }
        }


        void throwBall(Vector3 targ)
        {
            // estoy a rango de disparo
            GameManager.instancia.Quaffle.GetComponent<Quaffle>().
                Throw(targ - player.transform.position,
                    ((ChaserElfo)player).ThrowStrength);

            // Ya no tengo posesión de la pelota
            GameManager.instancia.ControlQuaffle(null);
        }


        public override void Reason(GameObject objeto)
        {

            if (th)
            {
                ChangeState(ChaserStateID.Wait);
            }

            // Si ya me encuentro a cierta distancia el objetivo, 
            // puedo tirar al aro
            if (Vector3.Distance(
                player.transform.position,
                player.steering.Target.position) < 15f) // calibrar

            {
                throwBall(player.steering.Target.position);

                // Cambiar de estado
                // pej. regresar a mi posicion inicial


            }

            //Que pasa si el jugador es golpeado
            if (player.hitted)
            {
                //Cambio el aestado que me stunea
                InitBlipState(StateID.ReceiveHit);
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





    public class Support : State
    {
        private Player player;

        float lastTime, t;

        public Vector3 offset = new Vector3(4f, 4f, 4f);
        Transform tr;


        // Variables del estado

        public Support(Player _player)
        {
            player = _player;


        }
        public override void OnEnter(GameObject objeto)
        {


            player.steering.Target = GameManager.instancia.Quaffle.transform.parent;

            player.steering.seek = false;
            lastTime = Time.timeSinceLevelLoad;
            t = 2f;



        }


        void DesFlee()
        {

            player.steering.pursuit = true;
            player.steering.pursuitWeight = 10f;
            Debug.Log("Flee");
        }

        Vector3 tempV;
        public override void Act(GameObject objeto)
        {

            if (player.steering.flee && Time.timeSinceLevelLoad >= lastTime + t)
            {
                DesFlee();
            }

            tr = GameManager.instancia.Quaffle.transform;
            tempV = tr.position;
            tempV+=  offset;
            
            player.steering.Target = tr;


        }



        public override void Reason(GameObject objeto)
        {




            //Que pasa si el jugador es golpeado
            if (player.hitted)
            {
                //Cambio el aestado que me stunea
                InitBlipState(StateID.ReceiveHit);
            }

        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.pursuit = false;
            player.steering.seek = false;
        }



        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }



    public class Defense : State
    {
        private Player player;
        int aro;
        Transform meta;
        // Variables del estado

        public Defense(Player _player)
        {
            player = _player;


        }
        public override void OnEnter(GameObject objeto)
        {
            aro = Random.Range(0, 3);
            player.transform.parent.GetComponent<TeamLosChidos>().getTeamNumber();
            meta = null;

            switch (player.transform.parent.GetComponent<TeamLosChidos>().getTeamNumber())
            {
                case 1:
                    Debug.Log(aro);
                    meta = GameManager.instancia.team1Goals[aro];
                    break;

                case 2:
                    meta = GameManager.instancia.team2Goals[aro];
                    break;
                default:
                    break;
            }

            player.steering.Target = meta;

            player.steering.arrive = true;

            Debug.Log("DEfense");
        }




        public override void Act(GameObject objeto)
        {

            //si la waffle está cerca, voy tras ella
            if (Vector3.Distance(player.transform.position, GameManager.instancia.Quaffle.transform.position) < 25f)
            {
                ChangeState(ChaserStateID.ChaseBall);
                /*
                // Llegar hasta la pelota
                // Veo si estoy cerca de la Quaffle
                if (Vector3.Distance(
                    player.transform.position,
                    GameManager.instancia.Quaffle.transform.position) < 2f)
                {
                    // Estoy cerca de la pelota, checo si otro jugador tiene
                    // posesión de ella
                    if (!GameManager.instancia.isQuaffleControlled())
                    {
                        // Si no esta controlada, yo puedo tomar posesión de ella
                        GameManager.instancia.ControlQuaffle(player.gameObject);

                        GameManager.instancia.
                            Quaffle.GetComponent<Quaffle>().
                                Control(player.transform);

                        // Cambiar de estado
                        ChangeState(ChaserStateID.SearchGoal);
                    }



                }*/
            }

        }



        public override void Reason(GameObject objeto)
        {

            //menor al aro me detengo
            if (Vector3.Distance(
               player.transform.position,
               meta.position) < 20f) // calibrar
            {

                player.steering.arrive = false;
            }



            //Que pasa si el jugador es golpeado
            if (player.hitted)
            {
                //Cambio el aestado que me stunea
                InitBlipState(StateID.ReceiveHit);
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


    public class Wait : State
    {
        private Player player;

        // Variables del estado

        public Wait(Player _player)
        {
            player = _player;


        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.ShutDownAll();
        }




        public override void Act(GameObject objeto)
        {

        }



        public override void Reason(GameObject objeto)
        {



            //Que pasa si el jugador es golpeado
            if (player.hitted)
            {
                //Cambio el aestado que me stunea
                InitBlipState(StateID.ReceiveHit);
            }

        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
        }



        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(2f);
        }
    }


}