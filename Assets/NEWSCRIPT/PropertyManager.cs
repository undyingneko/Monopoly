using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class PropertyManager : MonoBehaviour
{
    
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
    public class PropertyDataWrapper
    {
        public List<PropertyData> properties;
    }
    
    [System.Serializable]
    public class PropertyData
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

        public bool isComplimentaryMeal;

        public void InitializePrices()
        {
            stagePrices.Clear();
            rentPrices.Clear();
            stageIndexes.Clear();
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

    public List<PropertyData> properties = new List<PropertyData>();
    // Singleton instance
    public static PropertyManager instance;

    // public delegate void PropertiesLoadedCallback();
    // public event PropertiesLoadedCallback OnPropertiesLoaded;

    public static PropertyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PropertyManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PropertyManager).Name;
                    instance = obj.AddComponent<PropertyManager>();
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
        // LoadProperties();
    }

    private void OnTileImagesLoaded()
    {
        // Load properties after tile images have been loaded
        LoadProperties();
    }

    private void LoadProperties()
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "JSON", "propertiesinfo.json");

        if (File.Exists(jsonFilePath))
        {
            // File exists, try to read it
            string json = File.ReadAllText(jsonFilePath);
            Debug.Log("JSON Contents: " + json); // Print JSON contents

            // Deserialize JSON data into PropertyDataWrapper
            PropertyDataWrapper wrapper = JsonUtility.FromJson<PropertyDataWrapper>(json);
            
            if (wrapper != null && wrapper.properties != null && wrapper.properties.Count > 0)
            {
                // Properties loaded successfully
                Debug.Log("Properties loaded successfully. Count: " + wrapper.properties.Count);

                // Assign properties from wrapper to PropertyManager's properties
                properties = wrapper.properties;

                // Calculate prices for each property
                foreach (PropertyData property in properties)
                {
                    property.InitializePrices();
                    LoadStageImagesForProperty(property);
                    LoadRentTagImages(property);
                    property.currentStageIndex = -1;
                    property.isComplimentaryMeal = false;
                }
            }
            else
            {
                // No properties data found
                Debug.LogWarning("No properties data found.");
            }
        }
        else
        {
            // JSON file not found
            Debug.LogError("JSON file not found at path: " + jsonFilePath);
        }
        // OnPropertiesLoaded?.Invoke();
        
    }

    public PropertyData GetPropertyByWaypointIndex(int JSONwaypointIndex)
    {
        foreach (var property in properties)
        {
            if (property.JSONwaypointIndex == JSONwaypointIndex)
            {
                Debug.Log("Property found: " + property.name);

                return property;
            }
        }
        return null;
    }

    private void LoadStageImagesForProperty(PropertyData property)
    {   
    // Check if the waypointIndexToTileMap dictionary contains the specified key
    if (!gameManager.waypointIndexToTileMap.ContainsKey(property.JSONwaypointIndex))
    {
        Debug.LogError("Tile image not found for waypoint index: " + property.JSONwaypointIndex);
        return;
    }
        GameObject tileImage = gameManager.waypointIndexToTileMap[property.JSONwaypointIndex];
  
        for (int i = 0; i < property.stageIndexes.Count; i++)
        {
            string stageImageName = "P" + property.JSONwaypointIndex + "_S" + i;
            Transform stageImageTransform = tileImage.transform.Find(stageImageName);
            if (stageImageTransform == null)
            {
                Debug.LogError("Stage image object not found with name: " + stageImageName);
                continue;
            }
            GameObject stageImageInstance = stageImageTransform.gameObject;
            stageImageInstance.SetActive(false);
            property.stageImages.Add(stageImageInstance);
        }

        Debug.Log("Number of stage images after loaded for property " + property.name + ": " + property.stageImages.Count);

        // Ensure the number of loaded images matches the number of stages
        if (property.stageImages.Count == property.stageIndexes.Count)
        {
            for (int i = 0; i < property.stageImages.Count; i++)
            {
                // Assign the image to its corresponding stage
                GameObject stageImage = property.stageImages[i];
                Debug.Log("Stage Image for stage " + i + " associated with property " + property.name);
                
            }
        }
        else
        {
            Debug.LogWarning("Number of loaded images does not match the number of stages for property: " + property.name);
        }
    }

    public void DeactivateOldStageImages(PropertyData property)
    {
        // Ensure the property has stage images
        if (property.stageImages != null && property.stageImages.Count > 0)
        {
            // Iterate through each stage image
            foreach (GameObject stageImage in property.stageImages)
            {
                // Deactivate the stage image
                stageImage.SetActive(false);
            }
            Debug.Log("Old stage images deactivated for property: " + property.name);
        }
        else
        {
            Debug.LogWarning("No stage images found for property: " + property.name);
        }
    }

    private void LoadRentTagImages(PropertyData property)
    {
        string[] colors = { "pink", "turquois", "green", "purple" };
        GameObject tileImage = gameManager.waypointIndexToTileMap[property.JSONwaypointIndex];

        foreach (string color in colors)
        {
            string rentTagObjectName = "PriceTags_" + property.JSONwaypointIndex + "_" + color;
            Transform rentTagObject = tileImage.transform.Find(rentTagObjectName);
            if (rentTagObject != null)
            {
                GameObject rentTagImageInstance = rentTagObject.gameObject;
                rentTagImageInstance.SetActive(false);
                property.rentTagImages.Add(rentTagImageInstance);
            }
            else
            {
                Debug.LogWarning("Rent tag image object not found: " + rentTagObjectName);
            }
        }
        string rentTextObjectName = "RentText_" + property.JSONwaypointIndex;
        Transform rentTextObject = tileImage.transform.Find(rentTextObjectName);
        if (rentTextObject != null)
        {
            TextMeshProUGUI rentTextInstance = rentTextObject.GetComponent<TextMeshProUGUI>();

            if (rentTextInstance != null)
            {
                property.rentText = rentTextInstance;
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

    public void DeactivateRentTagImage(PropertyData property)
    {
            foreach (GameObject rentTagImage in property.rentTagImages)
            {
                rentTagImage.SetActive(false);
            }
    }
    
    public void ActivateRentTagImage(PropertyData property)
    {
        DeactivateRentTagImage(property);

        // Get the color associated with the player ID
        string color = playerIDToColor[property.ownerID];

        // Find the rent tag image corresponding to the color
        foreach (GameObject rentTagImage in property.rentTagImages)
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

    public void UpdateRentText(PropertyData property, int stageIndex)
    {
        // Ensure the rent text is not null
        if (property.rentText != null)
        {
            property.rentText.text = FormatPrice(property.rentPrices[stageIndex]);

            // property.rentText.text = property.rentPrices[stageIndex].ToString(); 
            property.rentText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rent text not found for property: " + property.name);
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
