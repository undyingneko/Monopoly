using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public bool GameOver;
    // public bool checksidefinished;

    
    // public bool sideMonopolyFound;
    // public bool tripleMonopolyFound;
    // public bool hotspringMonopolyFound;
    // public bool ownsallstalls;
    // public bool ownsallonsens;

    public int winningTeamID;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winningTeamText;
    public Image[] playerIcons;

    // private readonly List<int[]> sides = new List<int[]>
    // {
    //     new[] { 1, 2, 3, 5, 6, 7 },
    //     new[] { 9, 10, 11, 14, 15 },
    //     new[] { 17, 19, 21, 22 },
    //     new[] { 26, 27, 29, 30 }
    // };

    // private readonly List<int[]> packs = new List<int[]>
    // {
    //     new[] { 1, 2, 3 },
    //     new[] { 5, 6, 7 },
    //     new[] { 9, 10, 11 },
    //     new[] { 14, 15 },
    //     new[] { 17, 19 },
    //     new[] { 21, 22 },
    //     new[] { 26, 27 },
    //     new[] { 29, 30 }
    // };

    // private readonly int[] hotsprings = { 4, 13, 18, 25 };

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

        // ownsallstalls= false;
        // ownsallonsens= false;    
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
                // Debug.Log("found Dice_1_Image_" + i); // Initially set all dice images to inactive
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
            // Debug.Log("Loaded dice images from the hierarchy.");
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
        // Debug.Log("Property assigned to tile: " + property.name);
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
                // Debug.Log($"Tile image assigned for waypoint index: {waypointIndex}");
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

    public bool CheckSideMonopoly(int teamID)
    {
        // Define the waypoint indices for each side
        int[] side1Indices = { 1, 2, 3, 5, 6, 7 };
        int[] side2Indices = { 9, 10, 11, 14, 15 };
        int[] side3Indices = { 17, 19, 21, 22 };
        int[] side4Indices = { 26, 27, 29, 30 };

        // Check if the player owns all properties in any side
        bool side1Monopoly = CheckStallOwned(teamID, side1Indices);
        bool side2Monopoly = CheckStallOwned(teamID, side2Indices);
        bool side3Monopoly = CheckStallOwned(teamID, side3Indices);
        bool side4Monopoly = CheckStallOwned(teamID, side4Indices);
        Debug.Log("All properties checked for side monopoly.");
        // Return true if any side monopoly condition is met
        return side1Monopoly || side2Monopoly || side3Monopoly || side4Monopoly;

    }

    public bool CheckTripleMonopoly(int teamID)
    {
        // Define the waypoint indices for each pack
        int[] pack1Indices = { 1, 2, 3 };
        int[] pack2Indices = { 5, 6, 7 };
        int[] pack3Indices = { 9, 10, 11 };
        int[] pack4Indices = { 14, 15 };
        int[] pack5Indices = { 17, 19 };
        int[] pack6Indices = { 21, 22 };
        int[] pack7Indices = { 26, 27 };
        int[] pack8Indices = { 29, 30 };

        // Check if the player owns all properties in any pack
        bool pack1Monopoly = CheckStallOwned(teamID, pack1Indices);
        bool pack2Monopoly = CheckStallOwned(teamID, pack2Indices);
        bool pack3Monopoly = CheckStallOwned(teamID, pack3Indices);
        bool pack4Monopoly = CheckStallOwned(teamID, pack4Indices);
        bool pack5Monopoly = CheckStallOwned(teamID, pack5Indices);
        bool pack6Monopoly = CheckStallOwned(teamID, pack6Indices);
        bool pack7Monopoly = CheckStallOwned(teamID, pack7Indices);
        bool pack8Monopoly = CheckStallOwned(teamID, pack8Indices);

        // Return true if any triple monopoly condition is met
        return pack1Monopoly || pack2Monopoly || pack3Monopoly || pack4Monopoly ||
               pack5Monopoly || pack6Monopoly || pack7Monopoly || pack8Monopoly;
    }

    public bool CheckHotspringMonopoly(int teamID)
    {
        // Check if the player owns all hotsprings
        bool hotspringMonopoly = CheckOnsenOwned(teamID, new int[] { 4, 13, 18, 25 });

        return hotspringMonopoly;
    }

    public bool CheckBankruptcy()
    {
        // Check if all opponents have gone bankrupt
        bool allOpponentsBankrupt = true;
        foreach (PlayerController opponent in players)
        {
            if (!opponent.isBankRupt)
            {
                allOpponentsBankrupt = false;
                break;
            }
        }

        return allOpponentsBankrupt;
    }

    public bool CheckAcceptWinnings()
    {
        // Check if everyone has left the match
        bool everyoneLeft = true;
        foreach (PlayerController player in players)
        {
            if (player.gameObject.activeSelf)
            {
                everyoneLeft = false;
                break;
            }
        }

        return everyoneLeft;
    }

    private bool CheckStallOwned(int teamID, int[] waypointIndices)
    {
        foreach (int waypointIndex in waypointIndices)
        {
            StallManager.StallData stall = StallManager.Instance.GetStallByWaypointIndex(waypointIndex);

            if (stall == null || stall.teamownerID != teamID)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckOnsenOwned(int teamID, int[] waypointIndices)
    {
        foreach (int waypointIndex in waypointIndices)
        {
            OnsenManager.OnsenData onsen = OnsenManager.Instance.GetOnsenByWaypointIndex(waypointIndex);

            if (onsen == null || onsen.teamownerID != teamID)
            {
                return false;
            }
        }

        return true;
    }
    public void DisplayGameOverUI(int winningTeamID)
    {
        gameOverPanel.SetActive(true);
        winningTeamText.text = "Team " + winningTeamID + " Wins!";

        for (int i = 0; i < playerIcons.Length; i++)
        {
            playerIcons[i].gameObject.SetActive(players[i].teamID == winningTeamID);
            playerIcons[i].sprite = players[i].playerIcon.sprite;
        }
    }
       
    public IEnumerator CheckWinningConditions()
    {
        winningTeamID = -1;
        foreach (PlayerController player in players)
        {
        // Check if any winning condition is met
            bool sideMonopolyFound = CheckSideMonopoly(player.teamID);
            // bool tripleMonopolyFound = gameManager.CheckTripleMonopoly(this.teamID);
            // bool hotspringMonopolyFound = gameManager.CheckHotspringMonopoly(this.teamID);
            if (sideMonopolyFound)
            {
                Debug.Log("We have a winner!");
                GameOver = true;
                winningTeamID = player.teamID;
                DisplayGameOverUI(player.teamID);
            }
            else
            {
                Debug.Log("No side monopoly found.");
            }
        }
        yield return null;
        Debug.Log("Done checking all");
    } 
}
