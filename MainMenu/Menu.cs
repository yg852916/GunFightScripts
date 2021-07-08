using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open = false;
    private void Start()
    {
        //if active then open set true
        open = true;
    }
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
