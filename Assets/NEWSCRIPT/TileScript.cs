using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileScript : MonoBehaviour
{
    private TextMeshProUGUI Tilepopup_propertyNameText;
    private TextMeshProUGUI ownerText;
    private TextMeshProUGUI Tilepopup_RentPrice;
    private TextMeshProUGUI Tilepopup_BuyOutPrice;
    private TextMeshProUGUI priceStage0Text;
    private TextMeshProUGUI priceStage4Text;
    private TextMeshProUGUI[] stagePriceTexts = new TextMeshProUGUI[3];
    public string popupName = "TilepopupPrefab";

    public GameObject TilepopupPrefab; // Reference to the prefab variant of the popup window
    public Canvas canvas;
    private GameObject popupInstance;
    public GameObject assignedPopup; // Reference to the assigned popup for this tile

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

        assignedPopup = canvas.transform.Find(popupName).gameObject;
        if (assignedPopup != null)
        {
            assignedPopup.SetActive(false); // Ensure the popup is initially inactive
        }
        Button Tilepopup_closeButton = assignedPopup.transform.Find("Tilepopup_closeButton").GetComponent<Button>();
        Tilepopup_closeButton.onClick.AddListener(CloseActivePopup);      
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

        if (gameManager.isCardEffect || gameManager.isSelling)
        {
            return;
        }

        CloseActivePopup();

        if (assignedPopup != null)
        {
            assignedPopup.SetActive(true); // Set the assigned popup active
            activePopupInstance = assignedPopup;

            // Get the JSON waypoint index of the clicked tile
            int Tilepopup_waypointIndex = GetWaypointIndexFromName(gameObject.name);

            // Get property data based on the JSON waypoint index
            StallManager.StallData stallData = stallManager.GetStallByWaypointIndex(Tilepopup_waypointIndex);
            OnsenManager.OnsenData onsenData = onsenManager.GetOnsenByWaypointIndex(Tilepopup_waypointIndex);

            Tilepopup_propertyNameText = assignedPopup.transform.Find("Tilepopup_propertyNameText").GetComponent<TextMeshProUGUI>();
            ownerText = assignedPopup.transform.Find("OwnerText").GetComponent<TextMeshProUGUI>();
            Tilepopup_RentPrice = assignedPopup.transform.Find("Tilepopup_RentPrice").GetComponent<TextMeshProUGUI>();
            Tilepopup_BuyOutPrice = assignedPopup.transform.Find("Tilepopup_BuyOutPrice").GetComponent<TextMeshProUGUI>();
            priceStage0Text = assignedPopup.transform.Find("Tile_PriceStage0").GetComponent<TextMeshProUGUI>();
            priceStage4Text = assignedPopup.transform.Find("Tile_PriceStage4").GetComponent<TextMeshProUGUI>();
            for (int i = 1; i <= 3; i++)
            {
                stagePriceTexts[i-1] = assignedPopup.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                stagePriceTexts[i-1].gameObject.SetActive(false);
            }

            Tilepopup_propertyNameText.gameObject.SetActive(false);
            ownerText.gameObject.SetActive(false);
            Tilepopup_RentPrice.gameObject.SetActive(false);
            Tilepopup_BuyOutPrice.gameObject.SetActive(false);
            priceStage0Text.gameObject.SetActive(false);
            priceStage4Text.gameObject.SetActive(false);

            if (stallData != null)
            {
                UpdatePopupContentStall(assignedPopup, stallData);
            }
            else if (onsenData != null)
            {
                UpdatePopupContentOnsen(assignedPopup, onsenData);
            }
            else
            {
                Debug.Log("No property found for this tile.");
            }
        }
    }

    private void CloseActivePopup()
    {
        if (activePopupInstance != null)
        {
            activePopupInstance.SetActive(false);
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

        Tilepopup_propertyNameText.gameObject.SetActive(true);
        ownerText.gameObject.SetActive(true);
        Tilepopup_RentPrice.gameObject.SetActive(true);
        Tilepopup_BuyOutPrice.gameObject.SetActive(true);
        priceStage0Text.gameObject.SetActive(true);
        priceStage4Text.gameObject.SetActive(true);

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
            Tilepopup_BuyOutPrice.text = "";
        }

        string formattedStage0Price = gameManager.FormatPrice(item.stagePrices[0]);
        priceStage0Text.text = "Price to buy Land: " + formattedStage0Price;

        string formattedStage4Price = gameManager.FormatPrice(item.stagePrices[4]);
        priceStage4Text.text = "Price to buy Hotel: " + formattedStage4Price;

        for (int i = 1; i <= 3; i++)
        {
            if (i < item.stagePrices.Count)
            {
                stagePriceTexts[i - 1].gameObject.SetActive(true);
                string formattedStagePrice = gameManager.FormatPrice(item.stagePrices[i]);
                stagePriceTexts[i - 1].text = "Price stage " + i + ": " + formattedStagePrice;
            }
            else
            {
                stagePriceTexts[i - 1].gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePopupContentOnsen(GameObject popupInstance, OnsenManager.OnsenData item)
    {
        gameManager = FindObjectOfType<GameManager>();

        Tilepopup_propertyNameText.gameObject.SetActive(true);
        ownerText.gameObject.SetActive(true);
        Tilepopup_RentPrice.gameObject.SetActive(true);
        Tilepopup_BuyOutPrice.gameObject.SetActive(true);
        priceStage0Text.gameObject.SetActive(true);

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
            Tilepopup_BuyOutPrice.text = "";
        }

        string formattedStage0Price = gameManager.FormatPrice(item.priceOnsen);
        priceStage0Text.text = "Price to buy Onsen: " + formattedStage0Price;
    }
}
