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

    public void ShowPropertyDetails(string propertyName, List<int> stagePrices)
    {
    
        propertyNameText.text = propertyName;

        // Display stage prices
        for (int i = 0; i < stagePrices.Count; i++)
        {
            stagePriceTexts[i].text = "Stage " + (i + 1) + ": $" + stagePrices[i];
        }

        // Instantiate buy buttons for each stage
        for (int i = 0; i < stagePrices.Count; i++)
        {
            // Create a buy button for each stage
            GameObject buyButton = Instantiate(buyButtonPrefab, buttonContainers[i]);
            // Set button text
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            // Add functionality to the buy button
            int stageIndex = i;
            buyButton.GetComponent<Button>().onClick.AddListener(() => BuyStage(stageIndex));
        }

        // Show the popup window
        gameObject.SetActive(true);
    }

    private void BuyStage(int stageIndex)
    {
        // Implement buy functionality here for the selected stageIndex
        Debug.Log("Buying Stage " + (stageIndex + 1));
    }
}
