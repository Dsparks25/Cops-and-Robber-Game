using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Wander : CriminalController
{
    public float maxDistanceThreshold = 150.0f;
    public Sprite fleeSprite;
    public Sprite seekSprite;

    private void OnDisable()
    {
        // If there is no crime
        if (!criminal.isCrime)
        {
            // Enable flee
            this.criminal.flee.Enable();
            this.gameObject.GetComponent<SpriteRenderer>().sprite = fleeSprite;
            Debug.Log("Fleeing");
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
            // Get a random index within the available directions list
            int index = Random.Range(0, intersection.availableDirections.Count);

            // Check if the randomly selected direction is opposite to the criminal's current direction
            if (intersection.availableDirections[index] == -this.criminal.movement.direction)
            {
                // increment the index 
                index = (index + 1) % intersection.availableDirections.Count;
            }

            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach (Vector2 availableDirection in intersection.availableDirections)
            {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance = (this.criminal.playerTarget.position - newPosition).sqrMagnitude;

                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            // Set the criminal's movement to the selected direction
            this.criminal.movement.SetDirection(intersection.availableDirections[index]);

            // If the distance from the player target is less than the threshold or if there is an active crime, wander is disabled
            if (maxDistance < maxDistanceThreshold || criminal.isCrime)
            {
                this.criminal.wander.Disable();
            }
        }
    }
}
