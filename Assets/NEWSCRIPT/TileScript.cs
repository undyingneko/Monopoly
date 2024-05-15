using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TileScript : MonoBehaviour
{
    // public int Tilepopup_waypointIndex;
    public GameObject TilepopupPrefab; // Reference to the prefab variant of the popup window
    public Canvas canvas;
    private GameObject popupInstance;

    // public TextMeshProUGUI Tilepopup_propertyNameText;
    public TextMeshProUGUI[] Tilepopup_stagePriceTexts;
    // public TextMeshProUGUI Tilepopup_RentPrice;
    public Button Tilepopup_closeButton;

    private PropertyManager propertyManager;
    private static GameObject activePopupInstance;
    private GameManager gameManager;

    private void Start()
    {
        propertyManager = PropertyManager.Instance;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameManager.isCardEffect)
        {
            // Do nothing if Avenue Demolition is active
            return;
        }       
        BuyPropertyPopup012 buyPopup = FindObjectOfType<BuyPropertyPopup012>();
        if (buyPopup != null && buyPopup.isActiveAndEnabled)
        {
            return;
        }
        CloseActivePopup();
        // Perform raycasting to detect mouse click
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has a collider and is a tile
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                // Get the JSON waypoint index of the clicked tile
                int Tilepopup_waypointIndex = GetWaypointIndexFromName(hit.collider.gameObject.name);

                // Get property data based on the JSON waypoint index
                PropertyManager.PropertyData propertyData = propertyManager.GetPropertyByWaypointIndex(Tilepopup_waypointIndex);

                // Check if property data is not null
                if (propertyData != null)
                {
                    // Instantiate the popup window prefab variant
                    popupInstance = Instantiate(TilepopupPrefab, canvas.transform);
                    Button Tilepopup_closeButton = popupInstance.transform.Find("Tilepopup_closeButton").GetComponent<Button>();
                    Tilepopup_closeButton.onClick.AddListener(CloseActivePopup);

                    // Populate the popup window with property information
                    UpdatePopupContent(popupInstance, propertyData);
                    activePopupInstance = popupInstance;
                }
                else
                {
                    Debug.Log("No property found for this tile.");
                }
            }
        }
    }
    private void CloseActivePopup()
    {
        // Close the currently active popup if it exists
        if (activePopupInstance != null)
        {
            Destroy(activePopupInstance);
            activePopupInstance = null;
        }
    }
    private int GetWaypointIndexFromName(string gameObjectName)
    {
        int waypointIndex;
        string[] nameParts = gameObjectName.Split('_');
        if (nameParts.Length >= 2 && int.TryParse(nameParts[1], out waypointIndex))
        {
            return waypointIndex;
        }
        else
        {
            Debug.LogError("Invalid GameObject name format: " + gameObjectName);
            return -1; // Return -1 if unable to extract waypoint index
        }
    }
    private void UpdatePopupContent(GameObject popupInstance, PropertyManager.PropertyData propertyData)
    {
        gameManager = FindObjectOfType<GameManager>();
        // // Get references to the Text elements within the popup window

        TextMeshProUGUI Tilepopup_propertyNameText = popupInstance.transform.Find("Tilepopup_propertyNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ownerText = popupInstance.transform.Find("OwnerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_RentPrice = popupInstance.transform.Find("Tilepopup_RentPrice").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_BuyOutPrice = popupInstance.transform.Find("Tilepopup_BuyOutPrice").GetComponent<TextMeshProUGUI>();
        

        TextMeshProUGUI priceStage0Text = popupInstance.transform.Find("Tile_PriceStage0").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceStage4Text = popupInstance.transform.Find("Tile_PriceStage4").GetComponent<TextMeshProUGUI>();



        Tilepopup_propertyNameText.text = propertyData.name;
        ownerText.text = "Owned By: " + (propertyData.owned ? "Player " + propertyData.ownerID : "None");
        if (propertyData.owned)
        {
            // If property is owned, display current rent price
            string formattedStagePrice = gameManager.FormatPrice(propertyData.rentPrices[propertyData.currentStageIndex]);
            Tilepopup_RentPrice.text = "Current Rent Price: " + formattedStagePrice;

            string formattedBuyOutPrice = gameManager.FormatPrice(propertyData.buyoutPrices[propertyData.currentStageIndex]);
            Tilepopup_BuyOutPrice.text = "Buy Out Price: " + formattedBuyOutPrice;
           
            // Tilepopup_BuyOutPrice.text = "Buy Out Price: " + propertyData.buyoutPrices[propertyData.currentStageIndex];
        }
        else
        {
            // If property is not owned, display rent price as 0
            Tilepopup_RentPrice.text = "";
            Tilepopup_BuyOutPrice.text = "" ;
        }
        

        // string stagePricesText = "Price Each Stage:\n";
        // for (int i = 0; i < propertyData.prices.Count; i++)
        // {
        //     stagePricesText += "Stage " + i + ": " + propertyData.prices[i] + "\n";
        // }
        string formattedStage0Price = gameManager.FormatPrice(propertyData.stagePrices[0]);
        priceStage0Text.text = "Price to buy Land: " + formattedStage0Price;

        string formattedStage4Price = gameManager.FormatPrice(propertyData.stagePrices[4]);
        priceStage4Text.text = "Price to buy Hotel: " + formattedStage4Price;

        // priceStage0Text.text = "Price to buy Land: " + propertyData.stagePrices[0];
        // priceStage4Text.text = "Price to buy Hotel: " + propertyData.stagePrices[4];

        // priceEachStageText.text = stagePricesText;    
        for (int i = 1; i <= 3; i++)
        {
            if (i < propertyData.stagePrices.Count)
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                string formattedStagePrice = gameManager.FormatPrice(propertyData.stagePrices[i]);
                stagePriceText.text = "Price stage " + i + ": " + formattedStagePrice;
                stagePriceText.gameObject.SetActive(true);
                // TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                // stagePriceText.text = "Price stage " + i + ": " + propertyData.stagePrices[i];
                // stagePriceText.gameObject.SetActive(true);
            }
            else
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                stagePriceText.gameObject.SetActive(false);
            }
        }       



    }
    // public void Decline()
    // {
    //     // Close the popup window when the close button is pressed
    //     if (popupInstance != null)
    //     {
    //         Destroy(popupInstance);
    //     }
    // }   
}
