using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : CriminalController
{
    public float maxDistanceThreshold = 150.0f;
    public Sprite wanderSprite;
    public Sprite seekSprite;

    private void OnDisable()
    {
        // If there is no crime
        if (!criminal.isCrime)
        {
            // Enable wander
            this.criminal.wander.Enable();
            this.gameObject.GetComponent<SpriteRenderer>().sprite = wanderSprite;
            Debug.Log("Wandering");
        }
        else
        {
            // Enable seek
            this.criminal.seek.Enable();
            this.gameObject.GetComponent<SpriteRenderer>().sprite = seekSprite;
            Debug.Log("Seeking");
        }  
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has an Intersection component
        Intersection intersection = other.GetComponent<Intersection>();

        // If the intersection component exists and the script is enabled
        if (intersection != null && this.enabled)
        {
            // Initialize variables
            Vector2 direction = Vector2.zero; 
            float maxDistance = float.MinValue;

            // Find the direction with maximum distance from the player target
            foreach (Vector2 availableDirection in intersection.availableDirections)
            {
                // Calculate the new position by adding the direction's offset
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);

                // Calculate the squared distance from the player target to the new position
                float distance = (this.criminal.playerTarget.position - newPosition).sqrMagnitude;

                // Check if the new distance is greater than the max distance
                if (distance > maxDistance)
                {
                    // If so, update the direction and max distance
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            // Set the criminal's movement direction to the best direction
            this.criminal.movement.SetDirection(direction);

            // If the maxDistance is greater than the threshold, the criminal will no longer flee
            if (maxDistance > maxDistanceThreshold)
            {
                this.criminal.flee.Disable();
            }
        }
    }
}
