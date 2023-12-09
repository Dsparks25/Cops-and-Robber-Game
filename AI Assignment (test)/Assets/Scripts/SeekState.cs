using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : BaseState
{
    private Criminal criminal;

    public float maxDistanceThreshold = 150.0f;
    private float maxDistanceFromPlayer;

    private List<Vector2> currentPath;
    private int currentPathIndex;

    public GridManager gridManager;

    public Sprite SeekLeftSprite;
    public Sprite SeekRightSprite;
    public Sprite SeekUpSprite;
    public Sprite SeekDownSprite;

    private void Awake()
    {
        criminal = GetComponent<Criminal>();
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has an Intersection component
        Intersection intersection = other.GetComponent<Intersection>();

        // If the intersection component exists and the script is enabled
        if (intersection != null && this.enabled && criminal.isCrime)
        {
            if (gridManager != null)
            {
                // Calculate the A* path from the current position to the crime target
                Vector2 currentPosition = transform.position;
                currentPath = gridManager.FindPath(currentPosition, criminal.target.position);

                // If a valid path is found, start moving along the path
                if (currentPath != null && currentPath.Count > 0)
                {
                    currentPathIndex = 0;
                }
            }
            else
            {
                Debug.Log("No Gridmanager found in seek");
            }

            // Detects distance to the player
            Vector2 directionToPlayer = Vector2.zero;
            maxDistanceFromPlayer = float.MinValue;

            foreach (Vector2 availableDirection in intersection.availableDirections)
            {
                Vector3 newPosition1 = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance1 = (this.criminal.playerTarget.position - newPosition1).sqrMagnitude;

                if (distance1 > maxDistanceFromPlayer)
                {
                    directionToPlayer = availableDirection;
                    maxDistanceFromPlayer = distance1;
                }
            }


            //this.criminal.movement.SetDirection(direction);

            // If the threshold is greater than the distance from the player, seek is disabled
            //if (maxDistanceFromPlayer < maxDistanceThreshold)
            //{
            //    //
            //}
        }
    }

    private bool IsPlayerNearby()
    {
        bool isNearby = maxDistanceFromPlayer > 0.01 && maxDistanceFromPlayer < maxDistanceThreshold;
        Debug.Log("IsPlayerNearby: " + isNearby + ", MaxDistanceFromPlayer: " + maxDistanceFromPlayer);
        return isNearby;
    }

    public override BaseState CheckTransitions()
    {
        if (criminal.isCrime && IsPlayerNearby())
        {
            //Debug.Log("MaxDistanceFromPlayer: " + currentValue);
            return criminal.Flee;
        }
        if (!criminal.isCrime)
        {
            return criminal.Wander;
        }
        return null;
    }

    public override void EnterState()
    {
        // Enter Seek state logic
        this.enabled = true;
    }

    public override void ExitState()
    {
        // Exit Seek state logic
        this.enabled = false;
    }
    

    public override void Update()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            // Check if the AI has reached the current waypoint
            if (Vector2.Distance(transform.position, currentPath[currentPathIndex]) < 0.1f)
            {
                // Move to the next waypoint
                currentPathIndex++;

                // If the AI has reached the end of the path, disable seek
                if (currentPathIndex >= currentPath.Count)
                {
                    //ExitState();
                    criminal.isCrime = !criminal.isCrime;
                    return;
                }
            }

            // Set the movement direction towards the next waypoint
            Vector2 direction = (currentPath[currentPathIndex] - (Vector2)transform.position).normalized;

            // Ensure the direction is a cardinal direction
            direction = GetClosestUnitVector(direction);

            this.criminal.movement.SetDirection(direction);
            UpdateSprite();
            DrawPath();
        }
    }

    private Vector2 GetClosestUnitVector(Vector2 direction)
    {
        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        float minAngle = float.MaxValue;
        Vector2 closestDirection = Vector2.zero;

        foreach (Vector2 possibleDir in possibleDirections)
        {
            float angle = Vector2.Angle(direction, possibleDir);

            if (angle < minAngle)
            {
                minAngle = angle;
                closestDirection = possibleDir;
            }
        }

        return closestDirection;
    }

    private void DrawPath()
    {
        for (int i = currentPathIndex; i < currentPath.Count - 1; i++)
        {
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.green);
        }
    }

    public override void UpdateSprite()
    {
        if (criminal.movement.direction == Vector2.up)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = SeekUpSprite;
        }
        else if (criminal.movement.direction == Vector2.left)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = SeekLeftSprite;
        }
        else if (criminal.movement.direction == Vector2.right)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = SeekRightSprite;
        }
        else if (criminal.movement.direction == Vector2.down)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = SeekDownSprite;
        }
    }
}