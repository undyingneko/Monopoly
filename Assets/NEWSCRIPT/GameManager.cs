using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int winningTeamID;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winningTeamText;
    public Image[] playerIcons;

    private readonly List<int[]> sides = new List<int[]>
    {
        new[] { 1, 2, 3, 5, 6, 7 },
        new[] { 9, 10, 11, 14, 15 },
        new[] { 17, 19, 21, 22 },
        new[] { 26, 27, 29, 30 }
    };

    private readonly List<int[]> packs = new List<int[]>
    {
        new[] { 1, 2, 3 },
        new[] { 5, 6, 7 },
        new[] { 9, 10, 11 },
        new[] { 14, 15 },
        new[] { 17, 19 },
        new[] { 21, 22 },
        new[] { 26, 27 },
        new[] { 29, 30 }
    };

    private readonly int[] hotsprings = { 4, 13, 18, 25 };

    public PlayerController[] players;

    public Properties currentHostingFireWork;
    public Dictionary<int, GameObject> dice1Sides;
    public Dictionary<int, GameObject> dice2Sides;
    private Coroutine turnCoroutine;

    private Dictionary<GameObject, Properties> tileToPropertyMap = new Dictionary<GameObject, Properties>();
    public Dictionary<int, GameObject> waypointIndexToTileMap;

    public Properties selectedProperty;
    public List<Properties> selectedPropertiestoSell;
    
    
    public static int currentPlayerIndex;
    public static bool GameOver;
    
    // private PlayerController playerController;
    public static GameManager Instance;

    public bool buyPropertyDecisionMade = false;
    public bool buyOutDecisionMade = false;
    public bool OnsenDecisionMade = false;
    
    public bool EndedAllInteraction  = false;
    public bool FireworkPlaceIsSet  = false;
    public event Action TileImagesLoaded;
    // public bool isAvenueDemolitionActive = false;
    // public bool isPropertySeizureActive = false;
    public bool isCardEffect = false;
    public bool ChanceSelectionMade = false;
    public TextMeshProUGUI plus300Text;
    public bool isSelling = false;
    public bool SellSelectionMade = false;

    public int rentToPay;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure GameManager persists between scenes if needed
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
        }
    }
   
    void Start()
    {
        GameOver = false;
        // playerController = FindObjectOfType<PlayerController>();
        StartCoroutine(LoadTileImages());
        LoadDiceSides();
        StartGame();
        currentHostingFireWork = null;
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
        turnCoroutine = StartCoroutine(StartTurnCoroutine());
    }

    IEnumerator StartTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);
        players[currentPlayerIndex].StartTurn();
    }

    public void SetPlayers(PlayerController[] newPlayers)
    {
        players = newPlayers;
    }  

    public PlayerController GetCurrentPlayerController()
    {
        // Return the PlayerController of the current player
        return players[currentPlayerIndex];
    }

    public void NextTurn()
    {
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
        
        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        } while (players[currentPlayerIndex].isBankRupt);

        turnCoroutine = StartCoroutine(StartTurnCoroutine());
    }


    private void LoadDiceSides()
    {
        dice1Sides = new Dictionary<int, GameObject>();
        dice2Sides = new Dictionary<int, GameObject>();

        for (int i = 1; i <= 6; i++)
        {
            string dice1ImageName = "Dice_1_Image_" + i;
            GameObject dice1Image = GameObject.Find(dice1ImageName);
            if (dice1Image != null)
            {
                dice1Sides.Add(i, dice1Image);
                dice1Image.SetActive(false);
                Debug.Log("found Dice_1_Image_" + i); // Initially set all dice images to inactive
            }
            else
            {
                Debug.LogError("Dice_1 image not found: " + dice1ImageName);
            }

            string dice2ImageName = "Dice_2_Image_" + i;
            GameObject dice2Image = GameObject.Find(dice2ImageName);
            if (dice2Image != null)
            {
                dice2Sides.Add(i, dice2Image);
                dice2Image.SetActive(false); // Initially set all dice images to inactive
            }
            else
            {
                Debug.LogError("Dice_2 image not found: " + dice2ImageName);
            }
        }

        if (dice1Sides.Count == 0 || dice2Sides.Count == 0)
        {
            Debug.LogError("No dice images found in the hierarchy.");
        }
        else
        {
            Debug.Log("Loaded dice images from the hierarchy.");
        }
    }
    public string FormatPrice(int price)
    {
        if (price >= 1000000)
        {
            float millionValue = price / 1000000f;
            return millionValue.ToString("0.###") + "M";
        }
        else if (price >= 1000)
        {
            return (price / 1000f).ToString("0.#") + "K";
        }
        else
        {
            return price.ToString();
        }
    }

   public void AssignTileToWaypointIndex(int waypointIndex, GameObject tileImage)
    {
        if (waypointIndexToTileMap == null)
        {
            waypointIndexToTileMap = new Dictionary<int, GameObject>();
        } 

        if (!waypointIndexToTileMap.ContainsKey(waypointIndex))
        {
            waypointIndexToTileMap.Add(waypointIndex, tileImage);
        }
        else
        {
            Debug.LogWarning($"Waypoint index {waypointIndex} already has a tile assigned.");
        }
    }

    public GameObject GetTileForWaypointIndex(int waypointIndex)
    {
        if (waypointIndexToTileMap.ContainsKey(waypointIndex))
        {
            return waypointIndexToTileMap[waypointIndex];
        }
        else
        {
            Debug.LogWarning($"No tile assigned for waypoint index {waypointIndex}.");
            return null;
        }
    } 
    public void AssignPropertyToTile(GameObject tile, Properties property)
    {
        if (!tileToPropertyMap.ContainsKey(tile))
        {
            tileToPropertyMap.Add(tile, property);
        }
        else
        {
            tileToPropertyMap[tile] = property;
        }
        Debug.Log("Property assigned to tile: " + property.name);
    }

    public Properties GetPropertyFromTile(GameObject tile)
    {
        if (tileToPropertyMap.TryGetValue(tile, out var property))
        {
            return property;
        }
        return null;
    }

    public void HandleTileClick(Properties property)
    {
        // Update the selected property based on the clicked tile
        selectedProperty = property;
        // Perform any additional logic based on the selected property...
    } 

    IEnumerator LoadTileImages()
    {       
        GameObject[] tileImages = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tileImage in tileImages)
        {
            // Extract the TileWaypointIndex from the tile's name
            int waypointIndex;
            if (int.TryParse(tileImage.name.Replace("tile_", ""), out waypointIndex))
            {
                // Assign the tile image to its corresponding TileWaypointIndex
                AssignTileToWaypointIndex(waypointIndex, tileImage);
                Debug.Log($"Tile image assigned for waypoint index: {waypointIndex}");
            }
            else
            {
                Debug.LogWarning($"Failed to parse waypoint index from tile name: {tileImage.name}");
            }
        } 
        yield return null;
        TileImagesLoaded?.Invoke();
    }

    public void DisplayPlus300()
    {
        plus300Text.gameObject.SetActive(true);
        StartCoroutine(HidePlus300Text(plus300Text));
    }

    private IEnumerator HidePlus300Text(TextMeshProUGUI plus300Text)
    {
        yield return new WaitForSecondsRealtime(2f);
        plus300Text.gameObject.SetActive(false); // Destroy the Plus300Text object after 2 seconds
    }
    
    public IEnumerator HideObject(GameObject ObjectToHide)
    {
        yield return new WaitForSecondsRealtime(2f);
        ObjectToHide.gameObject.SetActive(false);
    } 

    public void CheckWinningConditions()
    {
        winningTeamID = -1;
        if (CheckSideMonopoly(ref winningTeamID) || CheckTripleMonopoly(ref winningTeamID) || CheckHotspringMonopoly(ref winningTeamID))
        {
            Debug.Log("We have a winner!");
            GameOver = true;
            DisplayGameOverUI(winningTeamID);
        }
    }

     private void DisplayGameOverUI(int winningTeamID)
    {
        gameOverPanel.SetActive(true);
        winningTeamText.text = "Team " + winningTeamID + " Wins!";

        for (int i = 0; i < playerIcons.Length; i++)
        {
            playerIcons[i].gameObject.SetActive(players[i].teamID == winningTeamID);
            playerIcons[i].sprite = players[i].playerIcon.sprite;
        }
    }

    private bool CheckSideMonopoly(ref int winningTeamID)
    {
        foreach (PlayerController player in players)
        {
            int teamID = player.teamID;
            foreach (int[] side in sides)
            {
                if (OwnsAllStalls(side, teamID))
                {
                    winningTeamID = teamID; // Set the winning team ID
                    Debug.Log($"Team {teamID} wins with a Side Monopoly!");
                    return true;
                }
            }
        }
        return false;
    }

    // Method to check Triple Monopoly
    private bool CheckTripleMonopoly(ref int winningTeamID)
    {
        winningTeamID = -1;
        foreach (PlayerController player in players)
        {
            int teamID = player.teamID;
            int packsOwned = 0;

            foreach (int[] pack in packs)
            {
                if (OwnsAllStalls(pack, teamID))
                {
                    packsOwned++;
                }
                if (packsOwned >= 3)
                {
                    winningTeamID = teamID;
                    Debug.Log($"Team {teamID} wins with a Triple Monopoly!");
                    return true;
                }
            }
        }
        return false;
    }

    // Method to check Hotspring Monopoly
    private bool CheckHotspringMonopoly(ref int winningTeamID)
    {
        winningTeamID = -1;
        foreach (PlayerController player in players)
        {
            int teamID = player.teamID;
            if (OwnsAllOnsen(hotsprings, teamID))
            {
                winningTeamID = teamID;
                Debug.Log($"Team {teamID} wins with a Hotspring Monopoly!");
                return true;
            }
        }
        return false;
    }

    // Helper method to check if a team owns all stalls in a given set of waypoint indices
    private bool OwnsAllStalls(int[] waypoints, int teamID)
    {
        foreach (int waypoint in waypoints)
        {
            StallManager.StallData stall = StallManager.Instance.GetStallByWaypointIndex(waypoint);
            if (stall == null || stall.teamownerID != teamID)
            {
                return false;
            }
        }
        return true;
    }

    private bool OwnsAllOnsen(int[] waypoints, int teamID)
    {
        foreach (int waypoint in waypoints)
        {
            OnsenManager.OnsenData onsen = OnsenManager.Instance.GetOnsenByWaypointIndex(waypoint);

            if (onsen == null ) return false;
            if (onsen == null || onsen.teamownerID != teamID)
            {
                return false;
            }
        }
        return true;
    }

}
