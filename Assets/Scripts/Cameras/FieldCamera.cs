using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCamera : MonoBehaviour
{
    public Transform Target;

    public Vector3 OffsetToTarget;

    private

	// Use this for initialization
	void Start ()
	{
		Target = GameManager.instancia.Quaffle.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(Target.position+OffsetToTarget);
	}

    public void LookSnitchAtEnd()
    {
        Target = GameManager.instancia.Snitch.transform;
        GetComponent<Camera>().fieldOfView = 10f;
    }
}
