using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameControl : MonoBehaviour
{
    private static GameObject[] playerMoveTexts;
    private static TextMeshProUGUI[] playerMoney;
    private static GameObject[] players;
    public static int diceSideThrown = 0;
    public static int[] playerStartWaypoints;
    public static bool gameOver = false;
    void Start()
    {  
        playerMoveTexts = new GameObject[4];
        playerMoney = new TextMeshProUGUI[4];
        for (int i = 0; i < 4; i++)
        {
            playerMoveTexts[i] = GameObject.Find("Player" + (i + 1) + "MoveText");
            playerMoney[i] = GameObject.Find("Player" + (i + 1) + "Money").GetComponent<TextMeshProUGUI>();
        }
        players = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {          
            players[i] = GameObject.Find("Player" + (i + 1));

            players[i].GetComponent<FollowThePath>().moveAllowed = false;
        }     
        for (int i = 0; i < 4; i++)
        {
            playerMoveTexts[i].gameObject.SetActive(true);
        }
        playerStartWaypoints = new int[4];
    }
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i].GetComponent<FollowThePath>().waypointIndex == ( (playerStartWaypoints[i] + diceSideThrown + 1 ) % players[i].GetComponent<FollowThePath>().waypoints.Length ) )
            {
                players[i].GetComponent<FollowThePath>().moveAllowed = false;
                playerMoveTexts[i].gameObject.SetActive(false);
                playerMoveTexts[(i + 1) % 4].gameObject.SetActive(true);
                playerStartWaypoints[i] = players[i].GetComponent<FollowThePath>().waypointIndex - 1;
            }
            UpdateMoneyText(playerMoney[i], players[i].GetComponent<FollowThePath>().Money);
            
        }
    }
    void UpdateMoneyText(TextMeshProUGUI moneyText, int moneyValue) {
        moneyText.text = "$" + moneyValue.ToString(); 
    }

    public static void MovePlayer(int playerToMove){
        players[playerToMove - 1].GetComponent<FollowThePath>().moveAllowed = true;
    }
 
}