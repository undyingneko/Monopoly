using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileScript : MonoBehaviour
{
    public int Tilepopup_waypointIndex;
    public GameObject TilepopupPrefab; // Reference to the prefab variant of the popup window

    public TextMeshProUGUI Tilepopup_propertyNameText;
    public TextMeshProUGUI[] Tilepopup_stagePriceTexts;
    public TextMeshProUGUI Tilepopup_RentPrice;
    public Button Tilepopup_closeButton;

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
                    GameObject popupInstance = Instantiate(TilepopupPrefab);

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
        // Text propertyNameText = popupInstance.transform.Find("PropertyNameText").GetComponent<Text>();
        // Text priceEachStageText = popupInstance.transform.Find("PriceEachStageText").GetComponent<Text>();
        Text ownerText = popupInstance.transform.Find("OwnerText").GetComponent<Text>();
        // Text rentPriceText = popupInstance.transform.Find("RentPriceText").GetComponent<Text>();



        //----- Update Text elements with property information-------
        // propertyNameText.text = "Property Name: " + propertyData.name;
        // priceEachStageText.text = "Price Each Stage: " + string.Join(", ", propertyData.prices);

        Tilepopup_propertyNameText.text = propertyData.name;


        for (int i = 0; i < Tilepopup_stagePriceTexts.Length; i++)
        {
            if (i < propertyData.prices.Count)
            {
                Tilepopup_stagePriceTexts[i].text = "Stage "+ i +"'s price: "+ propertyData.prices[i].ToString();

            }
            else
            {
                // If there are more stagePriceTexts than prices, deactivate the extra buttons
                Tilepopup_stagePriceTexts[i].gameObject.SetActive(false);
            }
        }        

        Tilepopup_RentPrice.text = "Current Rent Price: " + propertyData.rent;

        ownerText.text = "Owned By: " + (propertyData.owned ? "Player " + propertyData.ownerID : "None");
        // rentPriceText.text = "Rent Price: " + propertyData.rent;
    }
}
