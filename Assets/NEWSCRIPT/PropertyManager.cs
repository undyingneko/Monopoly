using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class PropertyManager : MonoBehaviour
{
    public Color[] playerColors;
    private Dictionary<PropertyData, Image> priceTags = new Dictionary<PropertyData, Image>();
    private PlayerController playerController;

    [SerializeField]
    private Transform canvasTransform;

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

        public List<int> stagePrices = new List<int>(); // Stores prices for each stage
        public List<int> rentPrices = new List<int>(); // Stores rent prices for each stage
        public List<int> stageIndexes = new List<int>();
        public List<GameObject> stageImages = new List<GameObject>();

        // public List<GameObject> stageImages;
        
        public bool owned;
        public int ownerID;
        public int teamownerID;

        public int buyoutMultiplier;
        public int buyoutCount;
        public int buyoutPrice;
        public int currentStageIndex; // Track the highest stage index that the player owns
        public int nextStageIndex;



        public void InitializePrices()
        {
            stagePrices.Clear();
            rentPrices.Clear();
            stageIndexes.Clear();
            stageImages.Clear();    

            for (int i = 0; i < 5; i++)
            {
                int stagePrice = CalculateStagePrice(i);
                int rentPrice = stagePrice / 2; // Or calculate rent price differently

                stagePrices.Add(stagePrice);
                rentPrices.Add(rentPrice);
                stageIndexes.Add(i);
            }
        }
        
        private int CalculateStagePrice(int stageIndex)
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

        // public int CalculateBuyoutPrice(int stageIndex)
        // {
        //     int basePrice = prices[stageIndex];
        //     buyoutPrice = basePrice;

        //     // Calculate buyout price based on buyout count
        //     for (int i = 0; i < buyoutCount; i++)
        //     {
        //         buyoutPrice *= 2;
        //     }

        //     return buyoutPrice;
        // } 
        public Image GetPriceTag()
        {
            // Implement this method to return the price tag associated with the property
            return null;
        }              
    }


    public List<PropertyData> properties = new List<PropertyData>();

    // Singleton instance
    private static PropertyManager instance;

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
        playerController = FindObjectOfType<PlayerController>();
        if (canvasTransform == null)
        {
            Debug.LogError("Canvas transform reference not set. Please assign the Canvas transform in the Inspector.");
            return;
        }
        // Call any method that requires canvasTransform here
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
        DontDestroyOnLoad(gameObject);
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
                    Image priceTag = property.GetPriceTag();
                    if (priceTag != null)
                    {
                        // Set the initial color of the price tag to black
                        priceTag.color = Color.black;
                        // Store the property and its price tag in the dictionary
                        priceTags[property] = priceTag;
                    }                   
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
        
    }
    
    public void UpdatePriceTagColor(PropertyData property, PlayerController player)
    {
        // Check if the property exists in the dictionary
        if (priceTags.ContainsKey(property))
        {
            // Deactivate all price tags first
            DeactivateAllPriceTags();
            // Activate the price tag prefab corresponding to the player's index
            ActivatePriceTagPrefab(player.GetPlayerIndex());
        }
        else
        {
            Debug.LogWarning("Price tag not found for property: " + property.name);
        }
    }

    private void DeactivateAllPriceTags()
    {
        // Deactivate all price tags on the screen
        foreach (var kvp in priceTags)
        {
            kvp.Value.gameObject.SetActive(false);
        }
    }

    private void ActivatePriceTagPrefab(int playerIndex)
    {
        // Instantiate and activate the price tag prefab based on the player's index
        // Replace "YourPrefabName" with the name of your prefab
        GameObject priceTagPrefab = Resources.Load<GameObject>("YourPrefabName_Player" + playerIndex);
        if (priceTagPrefab != null)
        {
            GameObject priceTagInstance = Instantiate(priceTagPrefab, canvasTransform);
            priceTagInstance.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Price tag prefab not found for player index: " + playerIndex);
        }
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
        Debug.LogWarning("No property found for waypoint index: " + JSONwaypointIndex);
        return null;
    }

    private void LoadStageImagesForProperty(PropertyData property)
    {
        // Iterate through each stage index
        for (int i = 0; i < property.stageIndexes.Count; i++)
        {
            // Construct the prefab path using JSON waypoint index and stage index
            string prefabPath = "StageImages/P" + property.JSONwaypointIndex + "_S" + i;

            // Load the stage image prefab from the Resources folder
            GameObject stageImagePrefab = Resources.Load<GameObject>(prefabPath);

            if (stageImagePrefab == null)
            {
                Debug.LogError("Stage image prefab not found at path: " + prefabPath);
                continue;
            }

            // Instantiate the stage image prefab
            GameObject stageImageInstance = Instantiate(stageImagePrefab);
            stageImageInstance.transform.SetParent(canvasTransform, false);
            stageImageInstance.SetActive(false);

            // Add the instantiated stage image to the property's stage images list
            property.stageImages.Add(stageImageInstance);
        }

        Debug.Log("Number of stage images after loaded for property " + property.name + ": " + property.stageImages.Count);

        // Ensure the number of loaded images matches the number of stages
        if (property.stageImages.Count == property.stageIndexes.Count)
        {
            for (int i = 0; i < property.stageImages.Count; i++)
            {

                GameObject stageImage = property.stageImages[i];
                Debug.Log("Stage Image for stage " + i + " associated with property " + property.name);
                
            }
        }
        else
        {
            Debug.LogWarning("Number of loaded images does not match the number of stages for property: " + property.name);
        }
    }
    

}
