using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeaterSangreLimpiaStates
{
    public enum BeaterStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        Protect,
        Hit
    }
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
                ChangeState(BeaterStateID.Protect);
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
    //=================================================== Protect
    public class Protect : State
    {
        private Player player;
        private Vector3 playerCazador;

        // Variables del estado

        public Protect(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
        }
        public override void Act(GameObject objeto)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < GameManager.instancia.team1Players.Count; i++)
                {
                    if (Vector3.Distance(GameManager.instancia.Bludger[j].transform.position, GameManager.instancia.team1Players[i].transform.position) <= 2f)
                    {

                        //("Entro aqui!!!" + GameManager.instancia.Bludger[j].transform.position);
                        player.steering.Target = GameManager.instancia.Bludger[j].transform;
                        player.steering.arrive = true;
                    }
                }
            }
        }
        public override void Reason(GameObject objeto)
        {
            if (Vector3.Distance(player.steering.Target.position, player.transform.position) <= 1f)
            {
                //("Va a pegar!!");
                ChangeState(BeaterStateID.Hit);
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
    //.................................................Hit
    public class Hit : State
    {
        private Player player;
        private Vector3 playerCazador;
        private bool beaterIntervention;

        // Variables del estado

        public Hit(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            beaterIntervention = false;
        }
        public override void Act(GameObject objeto)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Vector3.Distance(GameManager.instancia.Bludger[i].transform.position, player.transform.position) < 2f)
                {
                    GameManager.instancia.Bludger[i].GetComponent<Bludger>().BeaterIntervention(objeto);
                    beaterIntervention = true;
                    /*int num = Random.Range(0, GameManager.instancia.team2Players.Count);
                    player.steering.Target.GetComponent<Ball>().Throw(GameManager.instancia.team2Players[0].position, 3f);
                    player.steering.Target.GetComponent<Bludger>().BeaterIntervention(player);*/
                }
            }
        }
        public override void Reason(GameObject objeto)
        {
            if (beaterIntervention == true)
            {
                ChangeState(BeaterStateID.Protect);
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.arrive = false;
            beaterIntervention = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
