using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject objectToFollow;

    public Vector3 distanceToObject;

    public float speed = 2.0f;

    void Update()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = transform.position;
        position.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y + distanceToObject.y, interpolation);
        position.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x + distanceToObject.x, interpolation);
        position.z = Mathf.Lerp(transform.position.z, objectToFollow.transform.position.z + distanceToObject.z, interpolation);

        transform.position = position;
    }
}
