using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Gizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        Handles.Label(transform.position, gameObject.name);
    }
}
