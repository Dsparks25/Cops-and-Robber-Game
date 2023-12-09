using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Criminal))]
public class CriminalController : MonoBehaviour
{
    public Criminal criminal { get; private set; }

    private void Awake()
    {
        this.criminal = GetComponent<Criminal>();
        this.enabled = false;
    }

    public void Enable()
    {
        this.enabled = true;
    }

    public void Disable()
    {
        this.enabled = false;
    }
}