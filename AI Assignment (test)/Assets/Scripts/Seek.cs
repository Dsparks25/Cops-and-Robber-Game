using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Seek : CriminalController
{
    public float maxDistanceThreshold = 150.0f;
    public Sprite fleeSprite;
    public Sprite wanderSprite;

    public Sprite LeftSprite;
    public Sprite RightSprite;
    public Sprite UpSprite;
    public Sprite DownSprite;

    public bool hasCommitCrime;

    private List<Vector2> currentPath;
    private int currentPathIndex;
   // private Intersection intersection;

    public GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnDisable()
    {
        // If there is a crime
        if (criminal.isCrime)
        {
            // Enable flee
            this.criminal.flee.Enable();
            this.gameObject.GetComponent<SpriteRenderer>().sprite = fleeSprite;
            Debug.Log("Fleeing");
        }
        else
        {
            // Enable wander
            this.criminal.wander.Enable();
            this.gameObject.GetComponent<SpriteRenderer>().sprite = wanderSprite;
            Debug.Log("Wandering");
        }
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
            float maxDistanceFromPlayer = float.MinValue;

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
            if (maxDistanceFromPlayer < maxDistanceThreshold)
            {
                this.criminal.seek.Disable();
            }
        }
    }

    private void Update()
    {
        // If there is no crime, disable seek
        if (!criminal.isCrime)
        {
            this.criminal.seek.Disable();
        }

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
                    this.criminal.seek.Disable();
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

    private void UpdateSprite()
    {
        if (this.criminal.movement.direction == Vector2.up)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = UpSprite;
        }
        else if (this.criminal.movement.direction == Vector2.left)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = LeftSprite;
        }
        else if (this.criminal.movement.direction == Vector2.right)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = RightSprite;
        }
        else if (this.criminal.movement.direction == Vector2.down)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = DownSprite;
        }
    }

    private void DrawPath()
    {
        for (int i = currentPathIndex; i < currentPath.Count - 1; i++)
        {
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.green);
        }
    }
}
