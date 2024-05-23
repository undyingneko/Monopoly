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

    private StallManager stallManager;
    private OnsenManager onsenManager;
    private static GameObject activePopupInstance;
    private GameManager gameManager;

    private void Start()
    {
        stallManager = StallManager.Instance;
        onsenManager = OnsenManager.Instance;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameManager.isCardEffect || gameManager.isSelling)
        {
            // Do nothing if Avenue Demolition is active
            return;
        }       
        BuyPropertyPopup012 buyPopup = FindObjectOfType<BuyPropertyPopup012>();
        if (buyPopup != null && buyPopup.isActiveAndEnabled)
        {
            return;
        }
        BuyOutPopUp buyoutPopup = FindObjectOfType<BuyOutPopUp>();
        if (buyoutPopup != null && buyoutPopup.isActiveAndEnabled)
        {
            return;
        }
        if (gameManager.isCardEffect)
        {
            return;
        }
        if (gameManager.isSelling)
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
                StallManager.StallData stallData = stallManager.GetStallByWaypointIndex(Tilepopup_waypointIndex);
                OnsenManager.OnsenData onsenData = onsenManager.GetOnsenByWaypointIndex(Tilepopup_waypointIndex);


                // Check if property data is not null
                if (stallData != null)
                {
                    popupInstance = Instantiate(TilepopupPrefab, canvas.transform);
                    Button Tilepopup_closeButton = popupInstance.transform.Find("Tilepopup_closeButton").GetComponent<Button>();
                    Tilepopup_closeButton.onClick.AddListener(CloseActivePopup);
                    UpdatePopupContentStall(popupInstance, stallData);
                    activePopupInstance = popupInstance;
                }
                else if (onsenData != null)
                {
                    popupInstance = Instantiate(TilepopupPrefab, canvas.transform);
                    Button Tilepopup_closeButton = popupInstance.transform.Find("Tilepopup_closeButton").GetComponent<Button>();
                    Tilepopup_closeButton.onClick.AddListener(CloseActivePopup);
                    UpdatePopupContentOnsen(popupInstance, onsenData);
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
    private void UpdatePopupContentStall(GameObject popupInstance, StallManager.StallData item)
    {
        gameManager = FindObjectOfType<GameManager>();

        TextMeshProUGUI Tilepopup_propertyNameText = popupInstance.transform.Find("Tilepopup_propertyNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ownerText = popupInstance.transform.Find("OwnerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_RentPrice = popupInstance.transform.Find("Tilepopup_RentPrice").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_BuyOutPrice = popupInstance.transform.Find("Tilepopup_BuyOutPrice").GetComponent<TextMeshProUGUI>();
        
        TextMeshProUGUI priceStage0Text = popupInstance.transform.Find("Tile_PriceStage0").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceStage4Text = popupInstance.transform.Find("Tile_PriceStage4").GetComponent<TextMeshProUGUI>();

        Tilepopup_propertyNameText.text = item.name;
        ownerText.text = "Owned By: " + (item.owned ? "Player " + item.ownerID : "None");
        if (item.owned)
        {

            string formattedStagePrice = gameManager.FormatPrice(item.rentPrices[item.currentStageIndex]);
            Tilepopup_RentPrice.text = "Current Rent Price: " + formattedStagePrice;

            string formattedBuyOutPrice = gameManager.FormatPrice(item.buyoutPrices[item.currentStageIndex]);
            Tilepopup_BuyOutPrice.text = "Buy Out Price: " + formattedBuyOutPrice;

        }
        else
        {
            Tilepopup_RentPrice.text = "";
            Tilepopup_BuyOutPrice.text = "" ;
        }
        string formattedStage0Price = gameManager.FormatPrice(item.stagePrices[0]);
        priceStage0Text.text = "Price to buy Land: " + formattedStage0Price;

        string formattedStage4Price = gameManager.FormatPrice(item.stagePrices[4]);
        priceStage4Text.text = "Price to buy Hotel: " + formattedStage4Price;
  
        for (int i = 1; i <= 3; i++)
        {
            if (i < item.stagePrices.Count)
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                string formattedStagePrice = gameManager.FormatPrice(item.stagePrices[i]);
                stagePriceText.text = "Price stage " + i + ": " + formattedStagePrice;
                stagePriceText.gameObject.SetActive(true);
            }
            else
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                stagePriceText.gameObject.SetActive(false);
            }
        }       
    }
    private void UpdatePopupContentOnsen(GameObject popupInstance, OnsenManager.OnsenData item)
    {
        gameManager = FindObjectOfType<GameManager>();

        TextMeshProUGUI Tilepopup_propertyNameText = popupInstance.transform.Find("Tilepopup_propertyNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ownerText = popupInstance.transform.Find("OwnerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_RentPrice = popupInstance.transform.Find("Tilepopup_RentPrice").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_BuyOutPrice = popupInstance.transform.Find("Tilepopup_BuyOutPrice").GetComponent<TextMeshProUGUI>();
        
        TextMeshProUGUI priceStage0Text = popupInstance.transform.Find("Tile_PriceStage0").GetComponent<TextMeshProUGUI>();


        Tilepopup_propertyNameText.text = item.name;
        ownerText.text = "Owned By: " + (item.owned ? "Player " + item.ownerID : "None");
        if (item.owned)
        {
            string formattedStagePrice = gameManager.FormatPrice(item.rentPriceOnsen);
            Tilepopup_RentPrice.text = "Current Rent Price: " + formattedStagePrice;
            Tilepopup_BuyOutPrice.text = "";
        }
        else
        {
            Tilepopup_RentPrice.text = "";
            Tilepopup_BuyOutPrice.text = "" ;
        }
        string formattedStage0Price = gameManager.FormatPrice(item.priceOnsen);
        priceStage0Text.text = "Price to buy Land: " + formattedStage0Price;

  
        for (int i = 1; i <= 3; i++)
        {
            if (i < item.stagePrices.Count)
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                string formattedStagePrice = gameManager.FormatPrice(item.stagePrices[i]);
                stagePriceText.text = "Price stage " + i + ": " + formattedStagePrice;
                stagePriceText.gameObject.SetActive(true);
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
