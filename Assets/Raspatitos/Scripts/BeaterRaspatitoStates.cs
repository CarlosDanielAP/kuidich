using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace BeaterRaspatitoStates
{
    public enum BeaterStateID
    {
        Idle,
        Hit
    }

    public class Idle: State
    {
        private Player player;
        bool help;
        Transform [] helpTarget;
        GameObject [] a;
        int numberBludger;
       
        

        public Idle(Player _player)
        {
            player = _player;
            help = false;
            helpTarget = new Transform[2];
            a = new GameObject [2];
            numberBludger = 3;
        }
        public override void OnEnter(GameObject objeto)
        {
            player.steering.wander = true;

            numberBludger = ((player.myTeam as TeamRaspatitos).BludgerNumber(player.gameObject)) - 1;

            //(numberBludger);
        }
        public override void Act(GameObject objeto)
        {
           
        }
        public override void Reason(GameObject objeto)
        {
            
            helpTarget[numberBludger] = GameManager.instancia.Bludger[numberBludger].GetComponent<Bludger>().GetTarget();

            //(helpTarget[numberBludger]);

            a[numberBludger] = helpTarget[numberBludger].gameObject;

            //("Objeto: " + a[numberBludger]);

            if(player.myTeam.isTeammate(a[numberBludger]))
            {
                //("Bludger" + numberBludger + "es companero");
                player.steering.Target = GameManager.instancia.Bludger[numberBludger].transform;
                ChangeState(BeaterStateID.Hit);
            }
            else
            {
                //("Bludger" + numberBludger + "es rival");
            }


               
             
        }//fin reason

        public override void OnExit(GameObject objeto)
        {
            player.steering.wander= false;
        }

        IEnumerator VerFunction()
        {
            yield return new WaitForSeconds(0.4f);
        }
  

    }//idle

    public class Hit : State
    {
        //en este estado seguira a la bludger y la 
        //peara :v
        //que usamos? seek o pursuit?pursuit
        // quesea su etsado original no, perseguir la bludger?
        //no respondiste mi pregunta
        //puede ser, pero ahorita lo cambio
        private Player player;
        bool help;
        Transform helpTarget;
        GameObject a;
        TeamRaspatitos compa;
        int numberBludger;

        public Hit(Player _player)
        {
            player = _player;
            help = false;
            numberBludger = ((player.myTeam as TeamRaspatitos).BludgerNumber(player.gameObject)) - 1;
        }
        public override void OnEnter(GameObject objeto)
        {
            ////("Hit");
            player.steering.seek = true;
            player.steering.maxForce = 3;
            player.steering.maxSpeed = 25;
        }
        public override void Act(GameObject objeto)
        {

        }
        public override void Reason(GameObject objeto)
        {
            if(Vector3.Distance(player.transform.position, // pregunta si esta cerca de la bludger para golpearla 
                player.steering.Target.position) < 2f)
            {
               /* if (player.steering.Target == GameManager.instancia.Bludger[0].transform)
                //aqui debe golpear a la bludger
                {
                    GameManager.instancia.Bludger[0].GetComponent<Bludger>().BeaterIntervention(player.gameObject);
                }
                else if(player.steering.Target == GameManager.instancia.Bludger[1].transform)
                {
                    GameManager.instancia.Bludger[1].GetComponent<Bludger>().BeaterIntervention(player.gameObject);
                }*/

                GameManager.instancia.Bludger[numberBludger].GetComponent<Bludger>().BeaterIntervention(player.gameObject);
                //despues de golearla debe volver a idle
                ChangeState(BeaterStateID.Idle);
            }
           
        }//fin reason

        public override void OnExit(GameObject objeto)
        {
        }

        IEnumerator VerFunction()
        {
            yield return new WaitForSeconds(0.4f);
        }

    }//fin hit
}
