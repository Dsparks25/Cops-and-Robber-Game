using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : BaseState
{
    private Criminal criminal;

    public float maxDistanceThreshold = 150.0f;
    private float maxDistance;

    public Sprite FleeLeftSprite;
    public Sprite FleeRightSprite;
    public Sprite FleeUpSprite;
    public Sprite FleeDownSprite;

    private void Awake()
    {
        criminal = GetComponent<Criminal>();
    }

    public override BaseState CheckTransitions()
    {
        if (criminal.isCrime && IsPlayerNotNearby())
        {
            return criminal.Seek;
        }
        else if (!criminal.isCrime)
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
        UpdateSprite();
    }

    public override void UpdateSprite()
    {
        if (criminal.movement.direction == Vector2.up)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = FleeUpSprite;
        }
        else if (criminal.movement.direction == Vector2.left)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = FleeLeftSprite;
        }
        else if (criminal.movement.direction == Vector2.right)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = FleeRightSprite;
        }
        else if (criminal.movement.direction == Vector2.down)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = FleeDownSprite;
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
            maxDistance = float.MinValue;

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
            //if (maxDistance > maxDistanceThreshold)
            //{
            //    this.criminal.flee.Disable();
            //}
        }
    }

    private bool IsPlayerNotNearby()
    {
        bool isNotNearby = maxDistance > maxDistanceThreshold;
        Debug.Log("IsPlayerNearby: " + isNotNearby + ", MaxDistance: " + maxDistance);
        return isNotNearby;
    }

    // maxDistance < 0.01 &&
}
