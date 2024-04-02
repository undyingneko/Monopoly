using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PropertyManager : MonoBehaviour
{
    [System.Serializable]
    public class PropertyData
    {
        public string name;
        public int waypointIndex;
        public List<int> prices;
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

    // Load properties from JSON file
    private void LoadProperties()
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "JSON", "propertiesinfo.json");
        string json = File.ReadAllText(jsonFilePath);
        properties = JsonUtility.FromJson<List<PropertyData>>(json);

        // Calculate prices for each property
        foreach (PropertyData property in properties)
        {
            CalculatePropertyPrices(property);
        }
    }

    // Get property data by waypoint index
    public PropertyData GetPropertyByWaypointIndex(int waypointIndex)
    {
        foreach (var property in properties)
        {
            if (property.waypointIndex == waypointIndex)
            {
                return property;
            }
        }
        return null;
    }

    // Function to calculate property prices for different stages
    private void CalculatePropertyPrices(PropertyData property)
    {
        property.prices = new List<int>();

        // Set the initial price for stage 1
        int stage1Price = 100; // Example: Initial price for stage 1

        // Add stage 1 price
        property.prices.Add(stage1Price);

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
            int stagePrice = (int)(stage1Price * multiplier);

            // Add the price to the list
            property.prices.Add(stagePrice);
        }
    }
}
