using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    static public bool isGrounded;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" && other.tag != "HitBox")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player" && other.tag != "HitBox")
        {
            isGrounded = false;
        }
    }
}
