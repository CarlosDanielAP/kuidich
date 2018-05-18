using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public FSM fsm;
    public SteeringCombined steering;

    protected Vector3 oldPosition;

    protected GameObject Owner;     // El jugador que tiene el control de la pelota

    protected Rigidbody ballRigidbody;

    public void Throw(Vector3 direction, float force)
    {
        // No esta emparentada
        transform.parent = null;
        // No es trigger
        GetComponent<Collider>().isTrigger = false;
        // No es cinematico
        ballRigidbody.isKinematic = false;

        ballRigidbody.AddForce((direction.normalized) * force, ForceMode.Impulse);
    }

    public void GrabBall(GameObject Player)
    {
        // Le asignamos el jugador que la tiene
        Owner = Player;
    }

    public GameObject CurrentBallOwner()
    {
        return Owner;
    }

	protected virtual void Start () 
    {
        ballRigidbody = GetComponent<Rigidbody>();

        
	}

	protected virtual void Update () 
    {
		if(fsm != null && fsm.IsActive())
        {
            fsm.UpdateFSM();
        }
	}
}
