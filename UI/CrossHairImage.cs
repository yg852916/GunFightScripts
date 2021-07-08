using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairImage : MonoBehaviour
{
    public RectTransform crossHairRect;
    public RectTransform hitCrossHairRect;
    float hitDuration = 0.1f;

    public float originalSize;
    public float maxSize;
    public float currentSize;
    public float speed;
    void Start()
    {
        originalSize = crossHairRect.sizeDelta.x;
        EventCenter.instance.playerAttackDamage.AddListener(ChangeImage);
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = MoveSpeed();

        if (multiplier > 0)
        {
            if (currentSize < maxSize)
            {
                currentSize += Time.deltaTime * speed * multiplier;
            }
            else
            {
                currentSize = maxSize;
            }
        }
        else
        {
            if (currentSize > originalSize)
            {
                currentSize -= Time.deltaTime * speed;
            }
            else
            {
                currentSize = originalSize;
            }
        }
        crossHairRect.sizeDelta = new Vector2(currentSize, currentSize);

        //Change Image
        if(hitDuration > 0)
        {
            hitDuration -= Time.deltaTime;
            if (hitDuration <= 0)
            {
                hitDuration = 0;
                hitCrossHairRect.gameObject.SetActive(false);
            }
        }
    }
    float MoveSpeed()
    {
        float x = Mathf.Abs(Input.GetAxis("Horizontal"));
        float y = Mathf.Abs(Input.GetAxis("Vertical"));
        if (x > 0 || y > 0)
        {
            if (x >= y)
            {
                return x;
            }
            else if (x < y)
            {
                return y;
            }
        }
        return 0;
    }
    void ChangeImage(float useless)
    {
        hitDuration = 0.1f;
        hitCrossHairRect.gameObject.SetActive(true);
    }
}
