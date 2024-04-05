using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BuyPropertyPopup : MonoBehaviour
{
    public TextMeshProUGUI propertyNameText;
    public TextMeshProUGUI[] stagePriceTexts;
    public Button[] buyButtons;

    private PropertyManager.PropertyData currentProperty;
    private bool buyingStage; // Flag to track if the player is in the process of buying a stage
    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; // Coroutine reference for buy confirmation timer

    private void OnEnable()
    {
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }

    private void OnDisable()
    {
        // Stop the buy confirmation timer when the panel is disabled
        if (buyConfirmationCoroutine != null)
        {
            StopCoroutine(buyConfirmationCoroutine);
        }
    }

    public void Display(PropertyManager.PropertyData property)
    {
        currentProperty = property;

        propertyNameText.text = property.name;

        // Ensure the length of the stagePriceTexts array matches the number of prices in the property
        for (int i = 0; i < stagePriceTexts.Length; i++)
        {
            if (i < property.prices.Count)
            {
                stagePriceTexts[i].text = "Price: " + property.prices[i].ToString();
                buyButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // If there are more stagePriceTexts than prices, deactivate the extra buttons
                stagePriceTexts[i].gameObject.SetActive(false);
                buyButtons[i].gameObject.SetActive(false);
            }
        }

        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }


    public void BuyStage(int stageIndex)
    {
        if (!buyingStage)
        {
            buyingStage = true;

            // Deduct the stage price from the player's money
            int stagePrice = currentProperty.prices[stageIndex];
            FindObjectOfType<PlayerController>().Money -= stagePrice;

            // Set the property as owned by the player
            currentProperty.owned = true;
            currentProperty.ownerID = FindObjectOfType<PlayerController>().playerID;

            // Update UI
            FindObjectOfType<PlayerController>().UpdateMoneyText();

            // Close the popup
            gameObject.SetActive(false);

            // Stop the buy confirmation timer coroutine
            if (buyConfirmationCoroutine != null)
            {
                StopCoroutine(buyConfirmationCoroutine);
            }
        }
    }

    IEnumerator BuyConfirmationTimer()
    {
        yield return new WaitForSeconds(buyConfirmationTime);

        // Close the popup after the confirmation time if no purchase is made
        gameObject.SetActive(false);
    }

    public void Decline()
    {
        // Close the popup
        gameObject.SetActive(false);
    }
}
