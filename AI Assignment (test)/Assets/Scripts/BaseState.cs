using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public abstract BaseState CheckTransitions();
    public abstract void EnterState();
    public abstract void ExitState();

    public abstract void Update();

    public abstract void UpdateSprite();
}
