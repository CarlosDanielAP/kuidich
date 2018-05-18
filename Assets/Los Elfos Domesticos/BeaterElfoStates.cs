using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;

namespace BeaterElfoStates
{
    public enum BeaterStateID // Aqui agreguen las claves de cada estado que quieran
    {
        PrepareToPlay,
        ProtectTeammate,
        EscortTeammate,
		ScoutArea
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
            if(player.myStartingPosition != null)
            {
                // El jugador se pone en posición de inicio de juego
                player.steering.Target = player.myStartingPosition;
                player.steering.arrive = true;
            }
        }
        public override void Reason(GameObject objeto)
        {
            if(GameManager.instancia.isGameStarted())
            {
				ChangeState(BeaterStateID.ScoutArea);
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
    public class ProtectTeammate : State
    {
        private  Player player;

        // Variables del estado
		Transform CurrentBludger1Target;

		public ProtectTeammate(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
			// ("Voy a proteger compañero");
			//CurrentBludger1Target= GameObject.Find ("Bludger1").GetComponent<Bludger> ().GetTarget ();
			player.steering.Target = GameObject.Find ("Bludger1").GetComponent<Bludger> ().transform;
			player.steering.seek=true;
			player.steering.maxSpeed = 30f;
			player.steering.maxForce = 20f;

        }
        public override void Act(GameObject objeto)
        {
			if (Vector3.Distance (player.transform.position, GameObject.Find ("Bludger1").transform.position) < 5f) {
				GameObject.Find ("Bludger1").GetComponent<Bludger> ().BeaterIntervention(player.gameObject);
			}

        }
        public override void Reason(GameObject objeto)
        {

			if ((player.myTeam as TeamElfosDomesticos).isRival (CurrentBludger1Target)) {
				ChangeState (BeaterStateID.ScoutArea);
			}
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.seek = false;
			player.steering.maxSpeed = 15f;
			player.steering.maxForce = 10f;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }


	//=============================================================
	//=================================================== ScoutArea
	public class ScoutArea : State
	{
		private Player player;

		// Variables del estado
		int ClosestBludger=0;
		Transform Bludger1;
		Transform Bludger2;
		Transform CurrentBludger1Target;
		Transform CurrentBludger2Target;

		public ScoutArea(Player _player)
		{
			player = _player;
		}
		public override void OnEnter(GameObject objeto)
		{
			// Un compañero tiene la pelota, tratemos de acompañarlo en grupo
			player.steering.wander=true;
	


		}
		public override void Act(GameObject objeto)
		{

		
		}
		public override void Reason(GameObject objeto)
		{

			Bludger1 = GameObject.Find ("Bludger1").transform;
			Bludger2 = GameObject.Find ("Bludger2").transform;

			//calcular la bludger mas cercana al golpeador
			if (Vector3.Distance (player.transform.position, Bludger1.position) < 10f) {
				ClosestBludger = 1;
			}
			if (Vector3.Distance (player.transform.position, Bludger2.position) < 10f) {
				ClosestBludger = 2;
			}

			if (ClosestBludger != 0) {

				if (ClosestBludger == 1) {
					
					CurrentBludger1Target= GameObject.Find ("Bludger1").GetComponent<Bludger> ().GetTarget ();
					if ((player.myTeam as TeamElfosDomesticos).isTeammate (CurrentBludger1Target)) {
						player.steering.Target = Bludger1;
						player.steering.seek = true;
						player.steering.maxSpeed = 30f;
						player.steering.maxForce = 20f;

					} else {
						player.steering.Target = null;
						player.steering.seek = false;
						player.steering.maxSpeed = 15f;
						player.steering.maxForce = 10f;
					}

				}
				if (ClosestBludger == 2) {
					CurrentBludger2Target= GameObject.Find ("Bludger2").GetComponent<Bludger> ().GetTarget ();
					if ((player.myTeam as TeamElfosDomesticos).isTeammate(CurrentBludger2Target)) {
						player.steering.Target = Bludger2;
						player.steering.seek = true;
						player.steering.maxSpeed = 30f;
						player.steering.maxForce = 20f;
					}
					else {
						player.steering.Target = null;
						player.steering.seek = false;
						player.steering.maxSpeed = 15f;
						player.steering.maxForce = 10f;
					}
				}
			}





			// Quizás la pelota esté suelta, hay que buscarla
			if( ! GameManager.instancia.isQuaffleControlled())
			{
				//ChangeState(ChaserStateID.ChaseBall);
			}

			//Preungtar su alguna bluger tiene de obnejtivo un jugador amigo

		}
		public override void OnExit(GameObject objeto)
		{
			player.steering.wander = false;
			player.steering.seek = false;
			player.steering.maxSpeed = 15f;
			player.steering.maxForce = 10f;
		}

		IEnumerator IdleFunction()
		{
			yield return new WaitForSeconds(1f);
		}
	}


    //=============================================================
    //=================================================== EscortTeammate
    public class EscortTeammate : State
    {
        private Player player;

        // Variables del estado

        public EscortTeammate(Player _player)
        {
            player = _player;
        }
        public override void OnEnter(GameObject objeto)
        {
            // Un compañero tiene la pelota, tratemos de acompañarlo en grupo
            player.steering.teamSeparation = true;
            player.steering.teamCohesion = true;
            player.steering.teamAlignment = true;

        }
        public override void Act(GameObject objeto)
        {
        }
        public override void Reason(GameObject objeto)
        {
            // Quizás la pelota esté suelta, hay que buscarla
            if( ! GameManager.instancia.isQuaffleControlled())
            {
                //ChangeState(ChaserStateID.ChaseBall);
            }
        }
        public override void OnExit(GameObject objeto)
        {
            player.steering.teamSeparation = false;
            player.steering.teamCohesion = false;
            player.steering.teamAlignment = false;
        }

        IEnumerator IdleFunction()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}