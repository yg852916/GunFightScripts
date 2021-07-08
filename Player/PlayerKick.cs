using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    //Kick
    public GameObject kickObj;
    public float kickPower;
    public float kickDistance;
    public float kickTime;
    float accumulation;
    Animator anim;
    bool isKick = false;

    Vector3 originalPos,destinationPos;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isKick)
        {
            anim.SetTrigger("isKick");
            Kick();
        }
        if (isKick)
        {

            //kickObj.transform.position = Vector3.SmoothDamp(originalPos, destinationPos, ref velocity, kickTime);
            kickObj.transform.position += transform.forward * Time.deltaTime * kickPower;
            accumulation += Time.deltaTime;
            if (accumulation >= kickTime)
            {
                isKick = false;
                kickObj.transform.position = originalPos;
            }
            
        }
    }
    void Kick()
    {
        isKick = true;
        accumulation = 0;
        originalPos = kickObj.transform.position;
        destinationPos = transform.forward * kickDistance;
    }
}
