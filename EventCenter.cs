using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
public class PotionEvent : UnityEvent<float>
{ 
    
}
public class PlayerAttackDamage : UnityEvent<float>
{ 
    
}
public class RemoveItem : UnityEvent<int, int>
{ 

}
public class HealthControl : UnityEvent<string,float>
{ 

}
public class EquipThrowingWeapon : UnityEvent<int>
{ 

}
public class ActivateSkill : UnityEvent<string,GameObject,bool>
{ 

}
public class CanMove : UnityEvent<bool>
{ 
    
}
public class CancelHandAction : UnityEvent<string>
{ 
    
}
public class OpenMenu : UnityEvent<bool>
{ 

}
public class EventCenter : MonoBehaviour
{
    public static EventCenter instance;

    Dictionary<string,UnityAction<object>> events = new Dictionary<string, UnityAction<object>>();
    public PotionEvent potionEvents = new PotionEvent();
    public UnityEvent holsterWeapon = new UnityEvent();
    //public UnityEvent equipWeapon = new UnityEvent();
    public PlayerAttackDamage playerAttackDamage = new PlayerAttackDamage();
    public RemoveItem removeItem = new RemoveItem();
    public HealthControl healthControl = new HealthControl();
    public EquipThrowingWeapon equipThrowingWeapon = new EquipThrowingWeapon();
    public ActivateSkill activateSkill = new ActivateSkill();
    public UnityEvent stopSprinting = new UnityEvent();
    public CanMove canMove = new CanMove();
    public CancelHandAction cancelHandAction = new CancelHandAction();
    public OpenMenu openMenu = new OpenMenu();
    public UnityEvent itemAllDrop = new UnityEvent();

    private void Awake()
    {
        instance = this;
    }
    public void EventTrigger(string name, object info)
    { 
        if(events.ContainsKey(name))
        {
            events[name].Invoke(info);

        }
    }
    public void AddEventListener(string name, UnityAction<object> action)
    {
        if (events.ContainsKey(name))
        {
            events[name] += action;
        }
        else
        {
            events.Add(name, action);
        }
    }
    public void RemoveEventListener(string name, UnityAction<object> action)
    {
        if (events.ContainsKey(name))
        {
            events[name] -= action;
        }
    }
}
