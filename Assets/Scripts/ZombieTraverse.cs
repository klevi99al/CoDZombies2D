using System;
using System.Collections;
using UnityEngine;
using GizmoDebugger;

public class ZombieTraverse : MonoBehaviour
{
    private Transform targetNode;

    public enum TRAVERSE_DIRECTION
    {
        LEFT,
        RIGHT
    }

    private TRAVERSE_DIRECTION DIRECTION;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        targetNode = transform.GetChild(0);
        
        if(transform.position.x < targetNode.transform.position.x)
        {
            DIRECTION = TRAVERSE_DIRECTION.RIGHT;
        }
        else
        {
            DIRECTION = TRAVERSE_DIRECTION.LEFT;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        string tag = other.gameObject.tag.ToLower();
        if (tag.Equals("zombie"))
        {
            if (other.transform.rotation.y == 0 && DIRECTION.Equals(TRAVERSE_DIRECTION.RIGHT) || (other.transform.rotation.y != 0 && DIRECTION.Equals(TRAVERSE_DIRECTION.LEFT)))
            {
                ZombieMovements zombieMovements = other.gameObject.GetComponent<ZombieMovements>();
                zombieMovements.lastTraversalDirection = DIRECTION;
                zombieMovements.StartZombieTraversalMovement(targetNode.position);
            }
        }
        if (tag.Equals("player"))
        {
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }
    }


    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        GameObject startPoint = transform.gameObject;
        Vector3 endPoint = transform.GetChild(0).transform.position;

        Gizmos.color = Color.yellow;

        Vector3 direction = (endPoint - startPoint.transform.position).normalized * Vector3.Distance(startPoint.transform.position, endPoint);

        // starting position, direction, arrow head length, arrow head angle 
        RayGizmoDebugger.DrawGizmoTowardsTarget2D(startPoint.transform.position, direction, 0.3f, 30);
    }
}
