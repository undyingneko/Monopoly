using System.Data.SqlTypes;
using Mirror.Examples.Basic;
using UnityEngine;

public class SellingHandler : MonoBehaviour
{
    private PropertyManager.PropertyData associatedProperty;
    // private GameManager gameManager;
    public int totalPropertyValue;
    // public int moneyneeded;
    public PlayerController playerController;
    
    private void OnMouseDown()
    {
        // gameManager = FindObjectOfType<GameManager>();
        if (GameManager.Instance.isSelling)
        {
            if (associatedProperty == null)
            {
                Debug.LogError("Associated property is null!");
                return;
            }
            Debug.Log("Tile clicked for demolition: " + associatedProperty.name);
            totalPropertyValue = 0;
            playerController.selectedmoney.text = playerController.FormatMoney(totalPropertyValue);


            
            if (GameManager.Instance.selectedPropertiestoSell.Contains(associatedProperty))
            {
                // Property is already selected, remove it from the list
                GameManager.Instance.selectedPropertiestoSell.Remove(associatedProperty);
                // totalPropertyValue -= associatedProperty.stagePrices[associatedProperty.currentStageIndex];
                // playerController.selectedmoney.text = playerController.FormatMoney(totalPropertyValue);
                Debug.Log("Property removed from selection: " + associatedProperty.name);
            }
            else
            {
                // Property is not selected, add it to the list
                GameManager.Instance.selectedPropertiestoSell.Add(associatedProperty);
                // totalPropertyValue += associatedProperty.stagePrices[associatedProperty.currentStageIndex];
                // playerController.selectedmoney.text = playerController.FormatMoney(totalPropertyValue);
                Debug.Log("Property added to selection: " + associatedProperty.name);

            }

            foreach (var propertyToSell in GameManager.Instance.selectedPropertiestoSell)
            {
                totalPropertyValue += propertyToSell.stagePrices[propertyToSell.currentStageIndex];
                playerController.selectedmoney.text = playerController.FormatMoney(totalPropertyValue);
             
            }
            
            Debug.Log("Selected properties count: " + GameManager.Instance.selectedPropertiestoSell.Count);
            Debug.Log("GameManager selected property: " + (GameManager.Instance.selectedProperty != null ? GameManager.Instance.selectedProperty.name : "null"));
            
            if (totalPropertyValue >= playerController.moneyneeded)
            {
                // Player has enough money, enable the sell button
                EnableSellButton(playerController);
            }
            else
            {
                // Player does not have enough money, disable the sell button
                DisableSellButton(playerController);
            }

        }
        else
        {
            Debug.Log("Tile clicked outside of demolition mode.");
        }
    }
    public void EnableSellButton(PlayerController player)
    {
        player.sellButton.gameObject.SetActive(true); // Set the button active
        player.sellButton.onClick.RemoveAllListeners(); // Remove any existing listeners
        player.sellButton.onClick.AddListener(() => SellProperties(player));  // Add a listener for the button click event
    }
    public void DisableSellButton(PlayerController player)
    {
        player.sellButton.gameObject.SetActive(false); // Set the button inactive
        player.sellButton.onClick.RemoveAllListeners(); // Remove any existing listeners
    }
    private void SellProperties(PlayerController player)
    {
        // Set GameManager.Instance.SellSelectionMade to true
        player.sellButton.gameObject.SetActive(false);
        player.sellButton.onClick.RemoveAllListeners();
        GameManager.Instance.SellSelectionMade = true;
    }
    public void SetAssociatedProperty(PropertyManager.PropertyData property)
    {
        associatedProperty = property;
        Debug.Log("Associated property set: " + property.name);
    }


}
