using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    private int currentPlayerIndex;
    public static bool GameOver = false;
    
    private static TextMeshProUGUI[] playerMoney;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        // Initialize the game
        currentPlayerIndex = 0;
        StartCoroutine(StartTurnCoroutine());
    }

    IEnumerator StartTurnCoroutine()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the first turn
        players[currentPlayerIndex].StartTurn();
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
    }
    // public void NextTurn()
    // {
    //     if (currentPlayer.InJail && currentPlayer.turnsInJail >= 3)
    //     {
    //         currentPlayer.InJail = false; // Release the player from jail
    //         StartCoroutine(currentPlayer.RollTheDice()); // Roll the dice normally
    //     }
    //     else
    //     {
    //         currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
    //         StartCoroutine(StartTurnCoroutine());
    //     }
    // }
}
