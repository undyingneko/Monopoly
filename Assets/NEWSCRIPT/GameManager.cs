using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    private int currentPlayerIndex;
    public static bool GameOver = false;
    
    private static GameObject[] playerMoveTexts;
    private static TextMeshProUGUI[] playerMoney;

    void Start()
    {
        playerMoveTexts = new GameObject[4];
        // Find and store references to player move texts and money texts
        for (int i = 0; i < 4; i++)
        {
            playerMoveTexts[i] = GameObject.Find("Player" + (i + 1) + "MoveText");

        }

        StartGame();
    }

    void StartGame()
    {
        // Initialize the game
        currentPlayerIndex = 0;
        StartCoroutine(StartTurnCoroutine());
        for (int i = 0; i < players.Length; i++)
        {
            players[i].rollButton.interactable = (i == 0);
        }
    }

    IEnumerator StartTurnCoroutine()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the first turn
        for (int i = 0; i < 4; i++)
        {
            playerMoveTexts[i].SetActive(i == currentPlayerIndex);
        }
        players[currentPlayerIndex].StartTurn();
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        StartCoroutine(StartTurnCoroutine());

        // Disable roll button for all players except the current player
        for (int i = 0; i < players.Length; i++)
        {
            players[i].rollButton.interactable = (i == currentPlayerIndex);
        }
    }

}
