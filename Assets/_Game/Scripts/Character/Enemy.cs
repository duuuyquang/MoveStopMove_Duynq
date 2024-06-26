using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Enemy : Character
{
    [SerializeField] Canvas indicator;

    private string curState;

    protected override void OnInit()
    {
        base.OnInit();
        ToggleIndicator(false);
    }

    public override void ToggleIndicator(bool value)
    {
        indicator.enabled = value;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag(Const.TAG_NAME_BULLET))
        {
            ToggleIndicator(false);
        }
    }
}
