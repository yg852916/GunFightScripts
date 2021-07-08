using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    public float speed = 0.2f;
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * speed);
    }
}
