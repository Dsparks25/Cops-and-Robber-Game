using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Criminal : MonoBehaviour
{
    public Movement movement { get; private set; }

    public Wander wander { get; private set; }

    public Flee flee { get; private set; }

    public Seek seek { get; private set; }

    public WanderState Wander { get; private set; }
    public FleeState Flee { get; private set; }
    public SeekState Seek { get; private set; }

    private BaseState currentState;
    private BaseState initialState;


   // public CriminalController ininitialBehavior;

    public Transform playerTarget;

    public Transform target;

    public bool isCrime;

    private Player playerScript;
    private CrimeSpot crimeSpotScript;

    public int points = 10;

    public GridManager gridManager;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.Wander = GetComponent<WanderState>();
        this.Flee = GetComponent<FleeState>();
        this.Seek = GetComponent<SeekState>();
        //this.wander = GetComponent<Wander>();
       // this.flee = GetComponent<Flee>(); 
        //this.seek = GetComponent<Seek>();

        initialState = Wander;
        currentState = initialState;
        currentState.EnterState();
    }

    private void Start()
    {
        ResetCriminal();

        crimeSpotScript = FindObjectOfType<CrimeSpot>();
        playerScript = FindObjectOfType<Player>();
    }

    public void ResetCriminal()
    {
        this.gameObject.SetActive(true);
       // this.movement.ResetState();

       // this.wander.Enable();
       // this.flee.Disable();
        //this.seek.Disable();

        //if (this.ininitialBehavior != null)
        //{
        //    this.ininitialBehavior.Enable();
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isCrime = !isCrime;
        }

        playerTarget = playerScript.GetPlayerTransform();
        target = crimeSpotScript.GetPreviousCrimeSpotTransform();

        //currentState.Update();
       // Debug.Log("Current state: " + currentState);

    
    }

    private void FixedUpdate()
    {

    }

    private void SwitchState(BaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has an Intersection component
        Intersection intersection = other.GetComponent<Intersection>();

        // If the intersection component exists and the script is enabled
        if (intersection != null && this.enabled)
        {
            // Check for transitions within the current state
            BaseState nextState = currentState.CheckTransitions();

            // If a transition is triggered, switch to the new state
            if (nextState != null && nextState != currentState)
            {
                SwitchState(nextState);
            }
        }
    }
}
