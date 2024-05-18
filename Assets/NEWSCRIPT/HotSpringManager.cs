using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class HotSpringManager : MonoBehaviour
{
    public HotSpringData currentHotspotProperty = null;
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
    public class HotSpringDataWrapper
    {
        public List<HotSpringData> properties;
    }
    
    [System.Serializable]
    public class HotSpringData
    {
        public string name;
        public int HOTSPRINGwaypointIndex;

        public int priceHotSpring;
        public int rentPriceSHotSpring;
        public TextMeshProUGUI hotspringRentText;

        public GameObject HotSpringImage;

        public List<GameObject> rentTagImages = new List<GameObject>();

        public bool owned;
        public int ownerID;
        public int teamownerID;

        public bool isComplimentaryMeal;
        public bool isHotSpot;

        public void InitializePrices()
        {
            // priceHotSpring.Clear();
            // rentPriceSHotSpring.Clear();
            // stageIndexes.Clear();
            // stageImages.Clear();  
            // rentTagImages.Clear();   
            priceHotSpring = CalculatePriceHotSpring();
            rentPriceSHotSpring = CalculateRentPriceHotSpring();
        }
        public int CalculatePriceHotSpring()
        {
            int basehotspringprice = 50000;
            priceHotSpring = basehotspringprice;
            return priceHotSpring;
        }
        public int CalculateRentPriceHotSpring()
        {
            // int basePrice = priceHotSpring[stageIndex];
            // int buyoutPrice = basePrice;

            // // Calculate buyout price based on buyout count
            // for (int i = 0; i < (buyoutCount +1 ); i++)
            // {
            //     buyoutPrice *= 2;
            // }

            return rentPriceSHotSpring;
        }
    }

    public List<HotSpringData> hotsprings = new List<HotSpringData>();
    // Singleton instance
    public static HotSpringManager instance;

    // public delegate void hotspringsLoadedCallback();
    // public event hotspringsLoadedCallback OnhotspringsLoaded;

    public static HotSpringManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HotSpringManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(HotSpringManager).Name;
                    instance = obj.AddComponent<HotSpringManager>();
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
        // Loadhotsprings();
        currentHotspotProperty = null;
        InitializeHotSprings();
    }

    private void OnTileImagesLoaded()
    {
        // Load hotsprings after tile images have been loaded
        LoadHotSprings();
    }
    private void InitializeHotSprings()
    {
        hotsprings.Add(new HotSpringData { name = "Onsen 1", HOTSPRINGwaypointIndex = 4 });
        hotsprings.Add(new HotSpringData { name = "Onsen 2", HOTSPRINGwaypointIndex = 13 });
        hotsprings.Add(new HotSpringData { name = "Onsen 3", HOTSPRINGwaypointIndex = 18 });
        hotsprings.Add(new HotSpringData { name = "Onsen 4", HOTSPRINGwaypointIndex = 25 });
    }

    private void LoadHotSprings()
    {
        foreach (HotSpringData hotspring in hotsprings)
        {
            hotspring.isComplimentaryMeal = false;
            hotspring.isHotSpot = false;
            LoadImageForHotSpring(hotspring);
            LoadRentTagImagesHotSpring(hotspring);
        }
    }

    public HotSpringData GetHotSpringByWaypointIndex(int HOTSPRINGwaypointIndex)
    {
        foreach (var hotspring in hotsprings)
        {
            if (hotspring.HOTSPRINGwaypointIndex == HOTSPRINGwaypointIndex)
            {
                Debug.Log("HotSpring found: " + hotspring.name);

                return hotspring;
            }
        }
        return null;
    }

    private void LoadImageForHotSpring(HotSpringData hotspring)
    {
        if (!gameManager.waypointIndexToTileMap.ContainsKey(hotspring.HOTSPRINGwaypointIndex))
        {
            Debug.LogError("Tile image not found for waypoint index: " + hotspring.HOTSPRINGwaypointIndex);
            return;
        }
        GameObject tileImage = gameManager.waypointIndexToTileMap[hotspring.HOTSPRINGwaypointIndex];
        string HotSpringImageName = "Onsen_" + hotspring.HOTSPRINGwaypointIndex;
        Transform HotSpringImageTransform = tileImage.transform.Find(HotSpringImageName);
        if (HotSpringImageTransform == null)
        {
            Debug.LogError("HotSpring image object not found with name: " + HotSpringImageName);
            return;
        }
        hotspring.HotSpringImage = HotSpringImageTransform.gameObject;
        hotspring.HotSpringImage.SetActive(false);
    }

    public void DeactivateHotSpringImages(HotSpringData hotspring)
    {
        if (hotspring.HotSpringImage != null)
        {
            hotspring.HotSpringImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No HotSpring image found for property: " + hotspring.name);
        }
    }

    private void LoadRentTagImagesHotSpring(HotSpringData hotspring)
    {
        string[] colors = { "pink", "turquois", "green", "purple" };

        // Check if gameManager or waypointIndexToTileMap is null

        GameObject tileImage = gameManager.waypointIndexToTileMap[hotspring.HOTSPRINGwaypointIndex];

        // Ensure rentTagImages list is initialized
        // if (hotspring.rentTagImages == null)
        // {
        //     hotspring.rentTagImages = new List<GameObject>();
        // }

        foreach (string color in colors)
        {
            string rentTagObjectName = "PriceTags_" + hotspring.HOTSPRINGwaypointIndex + "_" + color;
            Transform rentTagObject = tileImage.transform.Find(rentTagObjectName);

            if (rentTagObject != null)
            {
                GameObject rentTagImageInstance = rentTagObject.gameObject;
                rentTagImageInstance.SetActive(false);
                hotspring.rentTagImages.Add(rentTagImageInstance);
            }
        }

        string hotspringRentTextObjectName = "RentText_" + hotspring.HOTSPRINGwaypointIndex;
        Transform hotspringRentTextObject = tileImage.transform.Find(hotspringRentTextObjectName);
        if (hotspringRentTextObject != null)
        {
            TextMeshProUGUI hotspringRentTextInstance = hotspringRentTextObject.GetComponent<TextMeshProUGUI>();

            if (hotspringRentTextInstance != null)
            {
                hotspring.hotspringRentText = hotspringRentTextInstance;
                hotspringRentTextInstance.gameObject.SetActive(false);
            }
        }

    }



    public void DeactivateHotSpringRentTagImage(HotSpringData property)
    {
        foreach (GameObject rentTagImage in property.rentTagImages)
        {
            rentTagImage.SetActive(false);
        }
    }
    
    public void ActivateRentTagImage(HotSpringData hotspring)
    {
        DeactivateHotSpringRentTagImage(hotspring);

        // Get the color associated with the player ID
        string color = playerIDToColor[hotspring.ownerID];

        // Find the rent tag image corresponding to the color
        foreach (GameObject rentTagImage in hotspring.rentTagImages)
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

    public void UpdatehotspringRentText(HotSpringData hotspring)
    {
        Debug.Log("Updating rent text for property: " + hotspring.name);
        Debug.Log(System.Environment.StackTrace);
        if (hotspring.hotspringRentText != null)
        {
            Debug.Log("Rent text is assigned for property: " + hotspring.name);
            hotspring.hotspringRentText.text = FormatPrice(hotspring.rentPriceSHotSpring);
            hotspring.hotspringRentText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rent text not found for property: " + hotspring.name);
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
