using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]

public class Player : MonoBehaviour
{
    public Sprite LeftSprite;
    public Sprite RightSprite;
    public Sprite UpSprite;
    public Sprite DownSprite;

    public Movement movement { get; private set; }

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.movement.SetDirection(Vector2.up);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = UpSprite;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.movement.SetDirection(Vector2.left);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = LeftSprite;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.movement.SetDirection(Vector2.down);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = DownSprite;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.movement.SetDirection(Vector2.right);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = RightSprite;
        }
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
    }
}
