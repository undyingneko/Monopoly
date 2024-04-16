using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public static int currentPlayerIndex;
    public static bool GameOver = false;
    
    private static TextMeshProUGUI[] playerMoney;
    private PlayerController playerController;
    public static GameManager Instance;
    void Awake()
    {
        Instance = this; // Assign the current instance to the static property
    }
   

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();


        StartGame();
        
    }

    void StartGame()
    {
        // Initialize the game
        currentPlayerIndex = 0;

        // Assign team IDs to players
        for (int i = 0; i < players.Length; i++)
        {
            int teamID = (i == 0 || i == 3) ? 1 : 2; // Alternating between team 1 and team 2

            players[i].AssignTeamID(teamID); // Assign team ID to the player
            players[i].AssignPlayerID(i + 1); // Assign player ID
        }

        StartCoroutine(StartTurnCoroutine());
    }




    IEnumerator StartTurnCoroutine()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the first turn
        players[currentPlayerIndex].StartTurn();
        // playerController.EndTurn();
    }

    public void SetPlayers(PlayerController[] newPlayers)
    {
        players = newPlayers;
    }   

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
    }


}
