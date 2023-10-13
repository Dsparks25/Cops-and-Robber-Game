using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Criminal[] criminals;

    public Player player;

    public int score { get; private set; }

    public int lives { get; private set; }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        for (int i = 0; i < this.criminals.Length; i++) {
            this.criminals[i].ResetState();
        }

        this.player.ResetState();   
    }

    private void GameOver()
    {
        for (int i = 0; i < this.criminals.Length; i++)
        {
            this.criminals[i].gameObject.SetActive(false);
        }

        this.player.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void CriminalEscaped()
    {
        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            //
        }
        else
        {
            GameOver();
        }
    }

    public void CriminalCaptured(Criminal criminal)
    {
        SetScore(this.score + criminal.points);
    }

}
