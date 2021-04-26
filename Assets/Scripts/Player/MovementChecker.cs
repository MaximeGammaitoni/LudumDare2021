using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementChecker : MonoBehaviour
{
    public Vector3? CheckMovement(Vector2 direction, float distanceToMove, LayerMask layer)
    {
        RaycastHit hit;
        Vector3 Direction3D = new Vector3(direction.x, 0, direction.y);
        if (Physics.Raycast(transform.position, Direction3D, out hit, distanceToMove, layer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            return null;
        }
        //Debug.Log("Move possible is " + isMovementPossible);
        //return isMovementPossible;
    }

    public Vector3? CheckMovement(Vector3 direction, float distanceToMove, LayerMask layer)
    {
        RaycastHit hit;
        Vector3 Direction3D = new Vector3(direction.x, 0, direction.y);
        if (Physics.Raycast(transform.position, Direction3D, out hit, distanceToMove, layer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            return null;
        }
    }
}
