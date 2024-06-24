using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] GameObject indicator;

    protected override void OnInit()
    {
        base.OnInit();
        ToggleIndicator(false);
    }

    public void ToggleIndicator(bool value)
    {
        indicator.SetActive(value);
    }

}
