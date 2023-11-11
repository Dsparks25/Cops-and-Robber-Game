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

    public CriminalController ininitialBehavior;

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
        this.wander = GetComponent<Wander>();
        this.flee = GetComponent<Flee>(); 
        this.seek = GetComponent<Seek>();
    }

    private void Start()
    {
        ResetState();

        crimeSpotScript = FindObjectOfType<CrimeSpot>();
        playerScript = FindObjectOfType<Player>();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
       // this.movement.ResetState();

        this.wander.Enable();
        this.flee.Disable();
        this.seek.Disable();

        if (this.ininitialBehavior != null)
        {
            this.ininitialBehavior.Enable();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isCrime = !isCrime;
        }

        playerTarget = playerScript.GetPlayerTransform();
        target = crimeSpotScript.GetPreviousCrimeSpotTransform(); 
    }

    private void FixedUpdate()
    {
       // Vector2 nearestNode = gridManager.GetNearestNode(transform.position);
        //Debug.Log($"AI snapped to nearest node: {nearestNode}");
    }
}
