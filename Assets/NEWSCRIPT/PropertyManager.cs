using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PropertyManager : MonoBehaviour
{
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
        public List<int> prices;
        public bool owned;
        public int ownerID;
        public int teamownerID;
        public int rent;
        public int buyoutMultiplier;
        public int buyoutCount;
        public int buyoutPrice;
        public int currentStageIndex; // Track the highest stage index that the player owns

    
        public void CalculateRent(int stageIndex)
        {
            // Initialize rent to 0
            rent = 0;

            // Check if prices list is not null and stageIndex is within its bounds
            if (prices != null && stageIndex >= 0 && stageIndex < prices.Count - 1)
            {
                // Rent is half of the price of the next stage
                rent = prices[stageIndex] / 2;
            }
            else
            {
                // Unable to calculate rent if the next stage doesn't exist or prices list is null
                Debug.LogError("Unable to calculate rent. Invalid stage index or no next stage.");
            }
        }

        public int CalculateBuyoutPrice(int stageIndex)
        {
            int basePrice = prices[stageIndex];
            buyoutPrice = basePrice;

            // Calculate buyout price based on buyout count
            for (int i = 0; i < buyoutCount; i++)
            {
                buyoutPrice *= 2;
            }

            return buyoutPrice;
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
                    CalculatePropertyPrices(property, property.priceStallBase); // Pass priceStage0 from JSON data
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

    // Function to calculate property prices for different stages
    private void CalculatePropertyPrices(PropertyData property, int priceStallBase)
    {
        property.prices = new List<int>();

        // Add stage 0 price (priceStallBase)
        property.prices.Add(priceStallBase);

        // Calculate prices for subsequent stages using multipliers
        float[] multipliers = { 5f, 10f, 15f, 30f };
        foreach (float multiplier in multipliers)
        {
            // Calculate the price for the current stage
            int stagePrice = (int)(priceStallBase * multiplier);

            // Add the price to the list
            property.prices.Add(stagePrice);
        }
    }
}
