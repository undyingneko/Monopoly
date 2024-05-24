using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public SellableItem currentHostingFireWork;
    public Dictionary<int, GameObject> dice1Sides;
    public Dictionary<int, GameObject> dice2Sides;
    private Coroutine turnCoroutine;

    private Dictionary<GameObject, SellableItem> tileToPropertyMap = new Dictionary<GameObject, SellableItem>();
    public Dictionary<int, GameObject> waypointIndexToTileMap;

    public SellableItem selectedProperty;
    public List<SellableItem> selectedPropertiestoSell;
    public PlayerController[] players;
    
    public static int currentPlayerIndex;
    public static bool GameOver = false;
    
    private PlayerController playerController;
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
        playerController = FindObjectOfType<PlayerController>();
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
    public void AssignPropertyToTile(GameObject tile, SellableItem property)
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

    public SellableItem GetPropertyFromTile(GameObject tile)
    {
        if (tileToPropertyMap.TryGetValue(tile, out var property))
        {
            return property;
        }
        return null;
    }

    public void HandleTileClick(SellableItem property)
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

}
