using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PropertyPopup : MonoBehaviour
{
    public TextMeshProUGUI propertyNameText;
    public TextMeshProUGUI[] stagePriceTexts;
    public GameObject buyButtonPrefab;
    public Transform[] buttonContainers; // Array of containers for buy buttons

    private int selectedStageIndex;

    public void ShowPropertyDetails(PropertyManager.PropertyData propertyData, int currentPlayerIndex)
    {
        propertyNameText.text = propertyData.name;

        for (int i = 0; i < propertyData.prices.Count; i++)
        {
            stagePriceTexts[i].text = "Stage " + (i + 1) + ": $" + propertyData.prices[i];
        }

        for (int i = 0; i < propertyData.prices.Count; i++)
        {
            GameObject buyButton = Instantiate(buyButtonPrefab, buttonContainers[i]);
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            int stageIndex = i;
            buyButton.GetComponent<Button>().onClick.AddListener(() => BuyStage(stageIndex, currentPlayerIndex));
        }

        // Show the popup window
        gameObject.SetActive(true);
    }

    private void BuyStage(int stageIndex, int currentPlayerIndex)
    {
        // Implement buy functionality here for the selected stageIndex
        Debug.Log("Buying Stage " + (stageIndex + 1));
        gameObject.SetActive(false);
    }
}
