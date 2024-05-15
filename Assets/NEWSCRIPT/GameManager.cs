using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    private Coroutine turnCoroutine;

    public Dictionary<GameObject, PropertyManager.PropertyData> tileToPropertyMap = new Dictionary<GameObject, PropertyManager.PropertyData>();

    public Dictionary<int, GameObject> waypointIndexToTileMap = new Dictionary<int, GameObject>();

    public PropertyManager.PropertyData selectedProperty;
    public PlayerController[] players;
    public static int currentPlayerIndex;
    public static bool GameOver = false;
    
    private static TextMeshProUGUI[] playerMoney;
    private PlayerController playerController;
    public static GameManager Instance;

    public bool buyPropertyDecisionMade = false;
    public bool buyOutDecisionMade = false;
    public bool EndedAllInteraction  = false;
    public event Action TileImagesLoaded;
    public bool isAvenueDemolitionActive = false;
    public bool isPropertySeizureActive = false;

    public GameObject buyPropertyPopupPrefab;


    private BuyPropertyPopup012 buyPropertyPopup012Prefab;
    private string buyPropertyPopup012PrefabPath = "BuyPropertyPopup012Prefab"; // Path to the prefab in the Resources folder
    // private BuyPropertyPopup012 BuypopupInstance;

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
        
        if (buyPropertyPopup012Prefab == null)
        {
            // Load and cache the buyPropertyPopup012Prefab
            GameObject prefab = LoadPrefab(buyPropertyPopup012PrefabPath);
            if (prefab != null)
            {
                // Cast the loaded GameObject to BuyPropertyPopup012
                buyPropertyPopup012Prefab = prefab.GetComponent<BuyPropertyPopup012>();
                if (buyPropertyPopup012Prefab == null)
                {
                    Debug.LogError("Failed to find BuyPropertyPopup012 component on prefab: " + buyPropertyPopup012PrefabPath);
                }
            }
            else
            {
                Debug.LogError("Failed to load BuyPropertyPopup012 prefab from Resources folder at path: " + buyPropertyPopup012PrefabPath);
            }
        }


        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas GameObject not found.");
            return;
        }     
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                GameObject popupInstance = Instantiate(buyPropertyPopupPrefab, canvas.transform);
                popupInstance.SetActive(false);
                BuyPropertyPopup012 popupComponent = popupInstance.GetComponent<BuyPropertyPopup012>();
                if (popupComponent != null)
                {
                    player.buyPropertyPopup = popupComponent;
                }
                else
                {
                    Debug.LogError("BuyPropertyPopup012 component not found on prefab.");
                }
            }
        }

        StartCoroutine(LoadTileImages());
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
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        turnCoroutine = StartCoroutine(StartTurnCoroutine());
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
    public void AssignPropertyToTile(GameObject tile, PropertyManager.PropertyData property)
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

    public PropertyManager.PropertyData GetPropertyFromTile(GameObject tile)
    {
        if (tileToPropertyMap.TryGetValue(tile, out var property))
        {
            return property;
        }
        return null;
    }

    public void HandleTileClick(PropertyManager.PropertyData property)
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
    public void ShowBuyPropertyPopup(PlayerController player, PropertyManager.PropertyData property)
    {
        if (player != null && player.buyPropertyPopup != null)
        {
            player.buyPropertyPopup.gameObject.SetActive(true);
            player.buyPropertyPopup.Display012(property);
        }
        else
        {
            Debug.LogError("BuyPropertyPopup012 instance is not created or player is null.");
        }
    }
    private GameObject LoadPrefab(string prefabPath)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab from Resources folder at path: " + prefabPath);
        }
        return prefab;
    }     
}
