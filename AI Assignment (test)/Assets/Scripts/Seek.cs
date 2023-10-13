using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : CriminalController
{
    public float maxDistanceThreshold = 150.0f;
    public Sprite fleeSprite;
    public Sprite wanderSprite;

    public bool hasCommitCrime;
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
            // Initialize variables
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // Find the direction with minimum distance from the player target
            foreach (Vector2 availableDirection in intersection.availableDirections)
            {
                // Calculate the new position by adding the direction's offset
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);

                // Calculate the squared distance from the player target to the new position
                float distance = (this.criminal.target.position - newPosition).sqrMagnitude;

                // Check if the new distance is less than the min distance
                if (distance < minDistance)
                {
                    // If so, update the direction and min distance
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

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

            this.criminal.movement.SetDirection(direction);

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
    }
}
