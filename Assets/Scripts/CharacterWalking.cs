using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterWalking : MonoBehaviour
{
    public EditorPath pathToFollow;

    public int CurrentWayPointID = 0;

    //Amount to travel (in metres) per second
    public float speed = 1f;
    public float reachDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public bool allowRotation;


    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(pathToFollow.path_objs[CurrentWayPointID].position, transform.position);
        transform.position = Vector3.MoveTowards(transform.position, pathToFollow.path_objs[CurrentWayPointID].position, Time.deltaTime * speed);


        if (allowRotation)
        {
            var rotation = Quaternion.LookRotation(pathToFollow.path_objs[CurrentWayPointID].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }


        if(distance <= reachDistance)
        {
            CurrentWayPointID++;
        }

        if(CurrentWayPointID >= pathToFollow.path_objs.Count)
        {
            CurrentWayPointID = 0;
        }
    }
}
