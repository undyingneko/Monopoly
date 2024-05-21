using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class StallManager : MonoBehaviour
{
    public StallData currentHotspotProperty = null;
    private GameManager gameManager;

    public Dictionary<int, string> playerIDToColor  = new Dictionary<int, string>
    {
        { 1, "pink" },
        { 2, "turquois" },
        { 3, "green" },
        { 4, "purple" }
    };

    public Transform canvasTransform;

    [System.Serializable]
    public class StallDataWrapper
    {
        public List<StallData> stalls;
    }
    
    [System.Serializable]
    public class StallData
    {
        public string name;
        public int JSONwaypointIndex;
        public int priceStallBase; 
        // public int numberOfStages;

        public List<int> stagePrices;
        public List<int> rentPrices;
        public List<int> buyoutPrices;
        public List<int> stageIndexes;

        public List<GameObject> stageImages;
        public List<GameObject> rentTagImages;

        public TextMeshProUGUI rentText;
        
        public bool owned;
        public int ownerID;
        public int teamownerID;

        public int buyoutMultiplier;
        public int buyoutCount;
 
        public int currentStageIndex; // Track the highest stage index that the player owns
        public int nextStageIndex;

        public bool isWelcomeEvent;
        public bool isFireWork;

        public void InitializePrices()
        {
            // stagePrices.Clear();
            // rentPrices.Clear();
            // stageIndexes.Clear();
            // stageImages.Clear();  
            // rentTagImages.Clear();   

            for (int i = 0; i < 5; i++)
            {
                int stagePrice = CalculateStagePrice(i);
                int rentPrice = stagePrice / 2; // Or calculate rent price differently

                stagePrices.Add(stagePrice);
                rentPrices.Add(rentPrice);
                stageIndexes.Add(i);
            }
            for (int i = 0; i < 5; i++)
            {
                int buyoutPrice = CalculateBuyoutPrice(i);
                // Add buyout price to the list
                buyoutPrices.Add(buyoutPrice);
            }
        }
        public int CalculateStagePrice(int stageIndex)
        {
            switch (stageIndex)
            {
                case 0:
                    return priceStallBase;
                case 1:
                    return 5 * priceStallBase;
                case 2:
                    return 10 * priceStallBase;
                case 3:
                    return 15 * priceStallBase;
                case 4:
                    return 30 * priceStallBase;
                default:
                    return 0; // Handle invalid stage index gracefully
            }
        }

        public int CalculateBuyoutPrice(int stageIndex)
        {
            int basePrice = stagePrices[stageIndex];
            int buyoutPrice = basePrice;

            // Calculate buyout price based on buyout count
            for (int i = 0; i < (buyoutCount +1 ); i++)
            {
                buyoutPrice *= 2;
            }

            return buyoutPrice;
        } 
    }

    public List<StallData> stalls = new List<StallData>();
    // Singleton instance
    public static StallManager instance;

    // public delegate void stallsLoadedCallback();
    // public event stallsLoadedCallback OnstallsLoaded;

    public static StallManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StallManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(StallManager).Name;
                    instance = obj.AddComponent<StallManager>();
                }
            }
            return instance;
        }
    }
    private void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
        if (canvasTransform == null)
        {
            Debug.LogError("Canvas transform reference not set. Please assign the Canvas transform in the Inspector.");
            return;
        }
        currentHotspotProperty = null;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.TileImagesLoaded += OnTileImagesLoaded;
        }
        DontDestroyOnLoad(gameObject);
        // Loadstalls();
        currentHotspotProperty = null;
    }

    private void OnTileImagesLoaded()
    {
        // Load stalls after tile images have been loaded
        LoadProperties();
    }

    private void LoadProperties()
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "JSON", "stallsinfo.json");

        if (File.Exists(jsonFilePath))
        {
            // File exists, try to read it
            string json = File.ReadAllText(jsonFilePath);
            Debug.Log("JSON Contents: " + json); // Print JSON contents

            // Deserialize JSON data into StallDataWrapper
            StallDataWrapper wrapper = JsonUtility.FromJson<StallDataWrapper>(json);
            
            if (wrapper != null && wrapper.stalls != null && wrapper.stalls.Count > 0)
            {
                // Properties loaded successfully
                Debug.Log("Properties loaded successfully. Count: " + wrapper.stalls.Count);

                // Assign stalls from wrapper to StallManager's stalls
                stalls = wrapper.stalls;

                // Calculate prices for each stall
                foreach (StallData stall in stalls)
                {
                    stall.InitializePrices();
                    LoadStageImagesForProperty(stall);
                    LoadRentTagImages(stall);
                    stall.currentStageIndex = -1;
                    stall.isWelcomeEvent = false;
                    stall.isFireWork = false;
                }
            }
            else
            {
                // No stalls data found
                Debug.LogWarning("No stalls data found.");
            }
        }
        else
        {
            // JSON file not found
            Debug.LogError("JSON file not found at path: " + jsonFilePath);
        }
        // OnPropertiesLoaded?.Invoke();
        
    }

    public StallData GetPropertyByWaypointIndex(int JSONwaypointIndex)
    {
        foreach (var stall in stalls)
        {
            if (stall.JSONwaypointIndex == JSONwaypointIndex)
            {
                Debug.Log("Property found: " + stall.name);

                return stall;
            }
        }
        return null;
    }

    private void LoadStageImagesForProperty(StallData stall)
    {   
    // Check if the waypointIndexToTileMap dictionary contains the specified key
    if (!gameManager.waypointIndexToTileMap.ContainsKey(stall.JSONwaypointIndex))
    {
        Debug.LogError("Tile image not found for waypoint index: " + stall.JSONwaypointIndex);
        return;
    }
        GameObject tileImage = gameManager.waypointIndexToTileMap[stall.JSONwaypointIndex];
  
        for (int i = 0; i < stall.stageIndexes.Count; i++)
        {
            string stageImageName = "P" + stall.JSONwaypointIndex + "_S" + i;
            Transform stageImageTransform = tileImage.transform.Find(stageImageName);
            if (stageImageTransform == null)
            {
                Debug.LogError("Stage image object not found with name: " + stageImageName);
                continue;
            }
            GameObject stageImageInstance = stageImageTransform.gameObject;
            stageImageInstance.SetActive(false);
            stall.stageImages.Add(stageImageInstance);
        }

        Debug.Log("Number of stage images after loaded for stall " + stall.name + ": " + stall.stageImages.Count);

        // Ensure the number of loaded images matches the number of stages
        if (stall.stageImages.Count == stall.stageIndexes.Count)
        {
            for (int i = 0; i < stall.stageImages.Count; i++)
            {
                // Assign the image to its corresponding stage
                GameObject stageImage = stall.stageImages[i];
                Debug.Log("Stage Image for stage " + i + " associated with stall " + stall.name);
                
            }
        }
        else
        {
            Debug.LogWarning("Number of loaded images does not match the number of stages for stall: " + stall.name);
        }
    }

    public void DeactivateOldStageImages(StallData stall)
    {
        // Ensure the stall has stage images
        if (stall.stageImages != null && stall.stageImages.Count > 0)
        {
            // Iterate through each stage image
            foreach (GameObject stageImage in stall.stageImages)
            {
                // Deactivate the stage image
                stageImage.SetActive(false);
            }
            Debug.Log("Old stage images deactivated for stall: " + stall.name);
        }
        else
        {
            Debug.LogWarning("No stage images found for stall: " + stall.name);
        }
    }

    private void LoadRentTagImages(StallData stall)
    {
        string[] colors = { "pink", "turquois", "green", "purple" };
        GameObject tileImage = gameManager.waypointIndexToTileMap[stall.JSONwaypointIndex];

        foreach (string color in colors)
        {
            string rentTagObjectName = "PriceTags_" + stall.JSONwaypointIndex + "_" + color;
            Transform rentTagObject = tileImage.transform.Find(rentTagObjectName);
            if (rentTagObject != null)
            {
                GameObject rentTagImageInstance = rentTagObject.gameObject;
                rentTagImageInstance.SetActive(false);
                stall.rentTagImages.Add(rentTagImageInstance);
            }
            else
            {
                Debug.LogWarning("Rent tag image object not found: " + rentTagObjectName);
            }
        }
        string rentTextObjectName = "RentText_" + stall.JSONwaypointIndex;
        Transform rentTextObject = tileImage.transform.Find(rentTextObjectName);
        if (rentTextObject != null)
        {
            TextMeshProUGUI rentTextInstance = rentTextObject.GetComponent<TextMeshProUGUI>();

            if (rentTextInstance != null)
            {
                stall.rentText = rentTextInstance;
                rentTextInstance.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Rent text component not found on object: " + rentTextObjectName);
            }
        }
        else
        {
            Debug.LogWarning("Rent text object not found: " + rentTextObjectName);
        }
    }

    public void DeactivateRentTagImage(StallData stall)
    {
            foreach (GameObject rentTagImage in stall.rentTagImages)
            {
                rentTagImage.SetActive(false);
            }
    }
    
    public void ActivateRentTagImage(StallData stall)
    {
        DeactivateRentTagImage(stall);

        // Get the color associated with the player ID
        string color = playerIDToColor[stall.ownerID];

        // Find the rent tag image corresponding to the color
        foreach (GameObject rentTagImage in stall.rentTagImages)
        {
            // Get the color variation of the rent tag image
            string rentTagColor = rentTagImage.name.Split('_')[2]; 
            rentTagColor = rentTagColor.Replace("(Clone)", "");
            Debug.Log("Rent tag color: " + rentTagColor + ", Expected color: " + color);

            // Compare the color variation with the player's color
            if (rentTagColor.Equals(color))
            {
                // Activate the rent tag image
                rentTagImage.SetActive(true);
                Debug.Log("Rent tag image activated for color: " + color);
                return; // Exit the loop once the rent tag image is activated
            }
        }

        Debug.LogWarning("Rent tag image not found for color: " + color);
    }

    public void UpdateRentText(StallData stall, int stageIndex)
    {
        Debug.Log("Updating rent text for stall: " + stall.name + " at stage index: " + stageIndex);
        Debug.Log(System.Environment.StackTrace);
        if (stall.rentText != null)
        {
            Debug.Log("Rent text is assigned for stall: " + stall.name);
            stall.rentText.text = FormatPrice(stall.rentPrices[stageIndex]);
            stall.rentText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rent text not found for stall: " + stall.name);
        }
    }
    
    public string FormatPrice(int price)
    {
        if (price >= 1000000)
        {
            return (price / 1000f).ToString("0,0K");
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

}
