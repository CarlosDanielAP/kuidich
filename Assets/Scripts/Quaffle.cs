using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quaffle : Ball
{
    public float freeTime;
    private float totalFreeTime = 1f;

    public bool Control(Transform player)
    {
        if (!freeMe)
        {
            // Le quito la velocidad que traiga
            ballRigidbody.velocity = Vector3.zero;
            GetComponent<Collider>().isTrigger = true;
            ballRigidbody.isKinematic = true;

            transform.parent = player;
            GrabBall(player.gameObject);

            return true;
        }
        return false;
    }

    private bool freeMe = false;
    public void Free()
    {
        GetComponent<Collider>().isTrigger = false;
        ballRigidbody.isKinematic = false;

        transform.parent = null;
        Owner = null;

        // sale volando para que otros la persigan
        freeMe = true;
        
    }

	// Use this for initialization
	protected override void Start () 
    {
        base.Start();

        freeTime = totalFreeTime;
    }
	
	// Update is called once per frame
	protected override void Update () 
    {
        //base.Update();
	}

    private void FixedUpdate()
    {
        // Encantamiento para que caiga más lento
        ballRigidbody.AddForce(ballRigidbody.mass * -Physics.gravity*0.7f);
        if(freeMe)
        {
            //GetComponent<Rigidbody>().AddForce(Vector3.up * 10f);
            //GetComponent<Rigidbody>().velocity= Vector3.up* 10f;
            ballRigidbody.AddForce(Vector3.up * 10f);

            freeTime -= Time.fixedDeltaTime;
            if (freeTime <= 0f)
            {
                freeMe = false;
                freeTime = totalFreeTime;
            }

            /// TODO:
            /// Agregar un tiempo de libreSoy para que los jugadores pregunten si la pueden agarrar
            /// para tratar de darle tiempo de volar y ser libre, como un pájaro

        }
    }
}
