using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AnimationEvent : UnityEvent { 

}
public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvent weaponAnimationEvent = new AnimationEvent();
    public void OnAnimationEvent()
    {
        weaponAnimationEvent.Invoke();
    }
}
