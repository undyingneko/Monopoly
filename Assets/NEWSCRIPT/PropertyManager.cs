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
        public int priceStage1; 
        public List<int> prices;
        public bool owned;
        public int ownerID;
        public int rent;
        
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
                    CalculatePropertyPrices(property, property.priceStage1); // Pass priceStage1 from JSON data
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







    // Get property data by waypoint index
    public PropertyData GetPropertyByWaypointIndex(int JSONwaypointIndex)
    {
        foreach (var property in properties)
        {
            if (property.JSONwaypointIndex  == JSONwaypointIndex)
            {
                return property;
            }
        }
        return null;
    }

    // Function to calculate property prices for different stages
    private void CalculatePropertyPrices(PropertyData property, int priceStage1)
    {
        property.prices = new List<int>();

        // Add stage 1 price
        property.prices.Add(priceStage1);

        // Calculate prices for subsequent stages
        for (int i = 1; i <= 4; i++) // Assuming there are 5 stages in total
        {
            float multiplier = 1f;
            if (i == 2)
                multiplier = 5f;
            else if (i == 3)
                multiplier = 5f * 2f;
            else if (i == 4)
                multiplier = 5f * 2f * 1.5f;
            else if (i == 5)
                multiplier = 5f * 2f * 1.5f * 2f;

            // Calculate the price for the current stage
            int stagePrice = (int)(priceStage1 * multiplier);

            // Add the price to the list
            property.prices.Add(stagePrice);
        }
    }

}
