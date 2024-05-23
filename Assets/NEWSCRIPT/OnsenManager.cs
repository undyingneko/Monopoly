using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class OnsenManager : MonoBehaviour
{
    // public OnsenData currentHostingFireWork = null;
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
    public class OnsenDataWrapper
    {
        public List<OnsenData> properties;
    }
    
    [System.Serializable]
    public class OnsenData
    {
        public string name;
        public int ONSENwaypointIndex;

        public int priceOnsen;
        public int rentPriceOnsen;
        public TextMeshProUGUI onsenRentText;

        public GameObject OnsenImage;
        public int currentStageIndex;

        public List<GameObject> rentTagImages = new List<GameObject>();

        public bool owned;
        public int ownerID;
        public int teamownerID;

        public bool isWelcomeEvent;
        public bool isFireWork;

        public void InitializePrices()
        {
            // priceOnsen.Clear();
            // rentPriceOnsen.Clear();
            // stageIndexes.Clear();
            // stageImages.Clear();  
            // rentTagImages.Clear();   
            priceOnsen = CalculatePriceOnsen();
            rentPriceOnsen = CalculateRentPriceOnsen();
        }
        public int CalculatePriceOnsen()
        {
            int baseonsenprice = 250000;
            priceOnsen = baseonsenprice;
            return priceOnsen;
        }
        public int CalculateRentPriceOnsen()
        {
            // int basePrice = priceOnsen[stageIndex];
            // int buyoutPrice = basePrice;

            // // Calculate buyout price based on buyout count
            // for (int i = 0; i < (buyoutCount +1 ); i++)
            // {
            //     buyoutPrice *= 2;
            // }
            rentPriceOnsen = priceOnsen/10;
            return rentPriceOnsen;
        }
    }

    public List<OnsenData> onsens = new List<OnsenData>();
    // Singleton instance
    public static OnsenManager instance;

    // public delegate void onsensLoadedCallback();
    // public event onsensLoadedCallback OnonsensLoaded;

    public static OnsenManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OnsenManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(OnsenManager).Name;
                    instance = obj.AddComponent<OnsenManager>();
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
        // currentHostingFireWork = null;
        
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
        // Loadonsens();
        // currentHostingFireWork = null;
        InitializeOnsens();
    }

    private void OnTileImagesLoaded()
    {
        // Load onsens after tile images have been loaded
        LoadOnsens();
    }
    private void InitializeOnsens()
    {
        onsens.Add(new OnsenData { name = "Onsen 1", ONSENwaypointIndex = 4 });
        onsens.Add(new OnsenData { name = "Onsen 2", ONSENwaypointIndex = 13 });
        onsens.Add(new OnsenData { name = "Onsen 3", ONSENwaypointIndex = 18 });
        onsens.Add(new OnsenData { name = "Onsen 4", ONSENwaypointIndex = 25 });
    }

    private void LoadOnsens()
    {
        foreach (OnsenData onsen in onsens)
        {
            onsen.InitializePrices();
            LoadImageForOnsen(onsen);
            LoadRentTagImagesOnsens(onsen);
            onsen.currentStageIndex = -1;
            onsen.isWelcomeEvent = false;
            onsen.isFireWork = false;
        }
    }

    public OnsenData GetOnsenByWaypointIndex(int ONSENwaypointIndex)
    {
        foreach (var onsen in onsens)
        {
            if (onsen.ONSENwaypointIndex == ONSENwaypointIndex)
            {
                Debug.Log("Onsen found: " + onsen.name);

                return onsen;
            }
        }
        return null;
    }

    private void LoadImageForOnsen(OnsenData onsen)
    {
        if (!gameManager.waypointIndexToTileMap.ContainsKey(onsen.ONSENwaypointIndex))
        {
            Debug.LogError("Tile image not found for waypoint index: " + onsen.ONSENwaypointIndex);
            return;
        }
        GameObject tileImage = gameManager.waypointIndexToTileMap[onsen.ONSENwaypointIndex];
        string OnsenImageName = "Onsen_" + onsen.ONSENwaypointIndex;
        Transform OnsenImageTransform = tileImage.transform.Find(OnsenImageName);
        if (OnsenImageTransform == null)
        {
            Debug.LogError("Onsen image object not found with name: " + OnsenImageName);
            return;
        }
        onsen.OnsenImage = OnsenImageTransform.gameObject;
        onsen.OnsenImage.SetActive(false);
    }

    public void DeactivateOnsenImages(OnsenData onsen)
    {
        if (onsen.OnsenImage != null)
        {
            onsen.OnsenImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No Onsen image found for property: " + onsen.name);
        }
    }

    private void LoadRentTagImagesOnsens(OnsenData onsen)
    {
        string[] colors = { "pink", "turquois", "green", "purple" };

        // Check if gameManager or waypointIndexToTileMap is null

        GameObject tileImage = gameManager.waypointIndexToTileMap[onsen.ONSENwaypointIndex];

        // Ensure rentTagImages list is initialized
        // if (onsen.rentTagImages == null)
        // {
        //     onsen.rentTagImages = new List<GameObject>();
        // }

        foreach (string color in colors)
        {
            string rentTagObjectName = "PriceTags_" + onsen.ONSENwaypointIndex + "_" + color;
            Transform rentTagObject = tileImage.transform.Find(rentTagObjectName);

            if (rentTagObject != null)
            {
                GameObject rentTagImageInstance = rentTagObject.gameObject;
                rentTagImageInstance.SetActive(false);
                onsen.rentTagImages.Add(rentTagImageInstance);
            }
        }

        string onsenRentTextObjectName = "RentText_" + onsen.ONSENwaypointIndex;
        Transform onsenRentTextObject = tileImage.transform.Find(onsenRentTextObjectName);
        if (onsenRentTextObject != null)
        {
            TextMeshProUGUI onsenRentTextInstance = onsenRentTextObject.GetComponent<TextMeshProUGUI>();

            if (onsenRentTextInstance != null)
            {
                onsen.onsenRentText = onsenRentTextInstance;
                onsenRentTextInstance.gameObject.SetActive(false);
            }
        }

    }



    public void DeactivateOnsenRentTagImage(OnsenData property)
    {
        foreach (GameObject rentTagImage in property.rentTagImages)
        {
            rentTagImage.SetActive(false);
        }
    }
    
    public void ActivateRentTagImage(OnsenData onsen)
    {
        DeactivateOnsenRentTagImage(onsen);

        // Get the color associated with the player ID
        string color = playerIDToColor[onsen.ownerID];

        // Find the rent tag image corresponding to the color
        foreach (GameObject rentTagImage in onsen.rentTagImages)
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

    public void UpdateonsenRentText(OnsenData onsen)
    {
        Debug.Log("Updating rent text for property: " + onsen.name);
        Debug.Log(System.Environment.StackTrace);
        if (onsen.onsenRentText != null)
        {
            Debug.Log("Rent text is assigned for property: " + onsen.name);
            onsen.onsenRentText.text = FormatPrice(onsen.rentPriceOnsen);
            onsen.onsenRentText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rent text not found for property: " + onsen.name);
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
