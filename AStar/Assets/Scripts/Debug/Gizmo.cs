using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Gizmo : MonoBehaviour
{
    public Color color;
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Handles.Label(transform.position, gameObject.name);
    }
}
