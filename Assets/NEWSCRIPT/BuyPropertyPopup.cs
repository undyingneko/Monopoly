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

        for (int i = 0; i < property.prices.Count; i++)
        {
            stagePriceTexts[i].text = "Price: " + property.prices[i].ToString();
        }

        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].gameObject.SetActive(true);
        }

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
