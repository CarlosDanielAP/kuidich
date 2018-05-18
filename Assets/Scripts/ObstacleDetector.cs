using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour 
{
    SteeringCombined steering;

    void OnTriggerEnter(Collider col)
    {
        // Si el detector esta chocando con objetos obstaculo
        if(col.tag.Equals("Obstacle"))
        {
            // Agrego este a la lista de obstaculos
            steering.Obstacles.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Si dejo de detectar un obstaculo, lo quito de la lista
        if(steering.Obstacles.Contains(col.gameObject))
        {
            steering.Obstacles.Remove(col.gameObject);
        }
    }
    
	void Start () 
    {
        steering = transform.parent.GetComponent<SteeringCombined>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
