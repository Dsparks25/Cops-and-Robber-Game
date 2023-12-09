using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : BaseState
{
    private Criminal criminal;

    public Sprite WanderLeftSprite;
    public Sprite WanderRightSprite;
    public Sprite WanderUpSprite;
    public Sprite WanderDownSprite;

    private void Awake()
    {
        criminal = GetComponent<Criminal>();
    }

    public override BaseState CheckTransitions()
    {
        // Check conditions for transitioning to other states
        if (criminal.isCrime)
        {
            return criminal.Seek;
        }

        // No transition triggered
        return null;
    }

    public override void EnterState()
    {
        // Enter Wander state logic
        this.enabled = true;
    }

    public override void ExitState()
    {
        // Exit Wander state logic
        this.enabled = false;
    }

    public override void Update()
    {
        UpdateSprite();
    }

    public override void UpdateSprite()
    {
        if (criminal.movement.direction == Vector2.up)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = WanderUpSprite;
        }
        else if (criminal.movement.direction == Vector2.left)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = WanderLeftSprite;
        }
        else if (criminal.movement.direction == Vector2.right)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = WanderRightSprite;
        }
        else if (criminal.movement.direction == Vector2.down)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = WanderDownSprite;
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
            if (intersection.availableDirections[index] == -criminal.movement.direction)
            {
                // increment the index 
                index = (index + 1) % intersection.availableDirections.Count;
            }

            // Detects distance from the player
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach (Vector2 availableDirection in intersection.availableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance = (criminal.playerTarget.position - newPosition).sqrMagnitude;

                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            // Set the criminal's movement to the selected direction
            criminal.movement.SetDirection(intersection.availableDirections[index]);

            // If the distance from the player target is less than the threshold or if there is an active crime, wander is disabled
            //if (maxDistance < criminal.Wander.maxDistanceThreshold || criminal.isCrime)
            //{
            //    criminal.wander.Disable();
            //}
        }
    }
}
