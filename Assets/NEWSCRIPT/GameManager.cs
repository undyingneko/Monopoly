using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
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

    [SerializeField]
    private CoroutineManager coroutineManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        coroutineManager = FindObjectOfType<CoroutineManager>();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        coroutineManager.StartTrackedCoroutine("LoadTileImages", LoadTileImages());
        StartGame();
    }

    void OnDestroy()
    {
        coroutineManager.StopAllTrackedCoroutines();
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

        coroutineManager.StartTrackedCoroutine("StartTurn", StartTurnCoroutine());
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

    public PlayerController GetCurrentPlayerController()
    {
        // Return the PlayerController of the current player
        return players[currentPlayerIndex];
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
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
            int waypointIndex;
            if (int.TryParse(tileImage.name.Replace("tile_", ""), out waypointIndex))
            {
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
}
