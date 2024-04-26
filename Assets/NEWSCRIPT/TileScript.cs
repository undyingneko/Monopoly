using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TileScript : MonoBehaviour
{
    public int Tilepopup_waypointIndex;
    public GameObject TilepopupPrefab; // Reference to the prefab variant of the popup window
    public Canvas canvas;
    private GameObject popupInstance;

    // public TextMeshProUGUI Tilepopup_propertyNameText;
    public TextMeshProUGUI[] Tilepopup_stagePriceTexts;
    // public TextMeshProUGUI Tilepopup_RentPrice;
    // public Button Tilepopup_closeButton;

    private PropertyManager propertyManager;

    private void Start()
    {
        propertyManager = PropertyManager.Instance;
    }

    private void OnMouseDown()
    {
        // Perform raycasting to detect mouse click
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has a collider and is a tile
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                // Get the JSON waypoint index of the clicked tile
                int Tilepopup_waypointIndex = hit.collider.GetComponent<TileScript>().Tilepopup_waypointIndex;

                // Get property data based on the JSON waypoint index
                PropertyManager.PropertyData propertyData = propertyManager.GetPropertyByWaypointIndex(Tilepopup_waypointIndex);

                // Check if property data is not null
                if (propertyData != null)
                {
                    // Instantiate the popup window prefab variant
                    popupInstance = Instantiate(TilepopupPrefab, canvas.transform);
                    Button Tilepopup_closeButton = popupInstance.transform.Find("Tilepopup_closeButton").GetComponent<Button>();
                    Tilepopup_closeButton.onClick.AddListener(Decline);

                    // Populate the popup window with property information
                    UpdatePopupContent(popupInstance, propertyData);
                }
                else
                {
                    Debug.Log("No property found for this tile.");
                }
            }
        }
    }

    private void UpdatePopupContent(GameObject popupInstance, PropertyManager.PropertyData propertyData)
    {
        // // Get references to the Text elements within the popup window

        TextMeshProUGUI Tilepopup_propertyNameText = popupInstance.transform.Find("Tilepopup_propertyNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ownerText = popupInstance.transform.Find("OwnerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_RentPrice = popupInstance.transform.Find("Tilepopup_RentPrice").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Tilepopup_BuyOutPrice = popupInstance.transform.Find("Tilepopup_BuyOutPrice").GetComponent<TextMeshProUGUI>();
        
        // TextMeshProUGUI priceEachStageText = popupInstance.transform.Find("PriceEachStageText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceStage0Text = popupInstance.transform.Find("Tile_PriceStage0").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceStage4Text = popupInstance.transform.Find("Tile_PriceStage4").GetComponent<TextMeshProUGUI>();
        // Tilepopup_stagePriceTexts = new TextMeshProUGUI[5]; // Assuming 5 stage prices
        // for (int i = 0; i < 5; i++)
        // {
        //     Tilepopup_stagePriceTexts[i] = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
        // }        


        //----- Update Text elements with property information-------


        Tilepopup_propertyNameText.text = propertyData.name;
        ownerText.text = "Owned By: " + (propertyData.owned ? "Player " + propertyData.ownerID : "None");
        Tilepopup_RentPrice.text = "Current Rent Price: " + propertyData.rentPrices;
        Tilepopup_BuyOutPrice.text = "Buy Out Price: " + propertyData.buyoutPrice;

        // string stagePricesText = "Price Each Stage:\n";
        // for (int i = 0; i < propertyData.prices.Count; i++)
        // {
        //     stagePricesText += "Stage " + i + ": " + propertyData.prices[i] + "\n";
        // }

        priceStage0Text.text = "Price to buy Land: " + propertyData.stagePrices[0];
        priceStage4Text.text = "Price to buy Hotel: " + propertyData.stagePrices[4];

        // priceEachStageText.text = stagePricesText;    
        for (int i = 1; i <= 3; i++)
        {
            if (i < propertyData.stagePrices.Count)
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                stagePriceText.text = "Price stage " + i + ": " + propertyData.stagePrices[i];
                stagePriceText.gameObject.SetActive(true);
            }
            else
            {
                TextMeshProUGUI stagePriceText = popupInstance.transform.Find("Tile_PriceStage" + i).GetComponent<TextMeshProUGUI>();
                stagePriceText.gameObject.SetActive(false);
            }
        }       



    }
    public void Decline()
    {
        // Close the popup window when the close button is pressed
        if (popupInstance != null)
        {
            Destroy(popupInstance);
        }
    }   
}
