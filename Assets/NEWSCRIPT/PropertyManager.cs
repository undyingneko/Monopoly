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
        public List<int> rentPrices = new List<int>(); 
        public List<int> buyoutPrices = new List<int>();// Stores rent prices for each stage
        public List<int> stageIndexes = new List<int>();
        
        // public List<GameObject> tiles = new List<GameObject>(); 
        public List<GameObject> stageImages = new List<GameObject>();
        public List<GameObject> rentTagImages = new List<GameObject>();


        public TextMeshProUGUI rentText;
        // public List<GameObject> stageImages;
        
        public bool owned;
        public int ownerID;
        public int teamownerID;

        public int buyoutMultiplier;
        public int buyoutCount;
 
        public int currentStageIndex; // Track the highest stage index that the player owns
        public int nextStageIndex;



        public void InitializePrices()
        {
            stagePrices.Clear();
            rentPrices.Clear();
            stageIndexes.Clear();
            stageImages.Clear();  
            rentTagImages.Clear();   


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
                    property.currentStageIndex = 0;
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
            
            string prefabPath = "StageImages/P" + property.JSONwaypointIndex + "_S" + i;

          
            GameObject stageImagePrefab = Resources.Load<GameObject>(prefabPath);

            if (stageImagePrefab == null)
            {
                Debug.LogError("Stage image prefab not found at path: " + prefabPath);
                continue;
            }

           
            GameObject stageImageInstance = Instantiate(stageImagePrefab);

            stageImageInstance.transform.SetParent(tileImage.transform, false);
            stageImageInstance.transform.localPosition = Vector3.zero;
            Canvas parentCanvas = tileImage.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                stageImageInstance.transform.SetSiblingIndex(tileImage.transform.GetSiblingIndex());
            }
            Debug.Log("Instantiated Object Position: " + stageImageInstance.transform.position);

            // stageImageInstance.SetActive(false);
            
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
        // Get the list of all color variations available for rent tag images
        string[] colors = new string[] { "pink", "turquois", "green", "purple" };

        // Iterate through each color
        foreach (string color in colors)
        {
            // Construct the path to the rent tag image based on the color and JSON waypoint index
            string rentTagImagePath = "RentTagImages/PriceTags_" + color + "_" + property.JSONwaypointIndex;

            // Load the rent tag image prefab from the Resources folder
            GameObject rentTagImagePrefab = Resources.Load<GameObject>(rentTagImagePath);
            

            if (rentTagImagePrefab != null)
            {
                // Create a GameObject for the rent tag image
                GameObject rentTagImageInstance = Instantiate(rentTagImagePrefab);
                rentTagImageInstance.transform.SetParent(canvasTransform, false);
                
                rentTagImageInstance.SetActive(false);
                property.rentTagImages.Add(rentTagImageInstance);
                
            }
            else
            {
                Debug.LogWarning("Rent tag image not found at path: " + rentTagImagePath);
            }
        }

        string rentTextPath = "RentTagImages/RentText_" + property.JSONwaypointIndex;
        TextMeshProUGUI rentTextPrefab = Resources.Load<TextMeshProUGUI>(rentTextPath);

        TextMeshProUGUI rentTextInstance = Instantiate(rentTextPrefab);
        rentTextInstance.transform.SetParent(canvasTransform, false);
        property.rentText = rentTextInstance;
        rentTextInstance.gameObject.SetActive(false);
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
            string rentTagColor = rentTagImage.name.Split('_')[1]; // Assuming the name format is "PriceTags_color_waypointIndex"

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
