using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BludgerCamera : MonoBehaviour
{
    public Transform Target;

    public Vector3 DistanceToTarget;

	// Use this for initialization
	void Start ()
    {
        transform.parent = Target;
        transform.localPosition = DistanceToTarget;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
