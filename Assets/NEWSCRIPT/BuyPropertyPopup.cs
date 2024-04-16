using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BuyPropertyPopup : MonoBehaviour
{
    public TextMeshProUGUI propertyNameText;
    public TextMeshProUGUI[] stagePriceTexts;
    public Button[] buyButtons;
    public Button closeButton; // Reference to the close button
   
    private PlayerController playerController;

    private PropertyManager.PropertyData currentProperty;
    private bool buyingStage; // Flag to track if the player is in the process of buying a stage
    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; // Coroutine reference for buy confirmation timer
    private GameManager gameManager;


    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Popup enabled");
            playerController.isBuyPopUpActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            closeButton.onClick.AddListener(Decline); // Add a listener to the close button
            for (int i = 0; i < buyButtons.Length; i++)
            {
                int index = i; // Store the current index in a local variable to avoid closure issues
                buyButtons[i].onClick.AddListener(() => BuyStage(index, GameManager.currentPlayerIndex));
            }
        }
        else
        {
            Debug.LogError("PlayerController not found!");
        }
    }

    private void OnDisable()
    {   Debug.Log("Popup disabled");
        playerController = FindObjectOfType<PlayerController>();
        playerController.isBuyPopUpActive = false;
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

    public void BuyStage(int stageIndex, int currentPlayerIndex)
    {
        if (!buyingStage)
        {
            buyingStage = true;
            int stagePrice = currentProperty.prices[stageIndex];
            
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= stagePrice)
                    {
                        currentPlayer.Money -= stagePrice; // Deduct money
                        Debug.Log("Money deducted successfully. Remaining money: " + currentPlayer.Money);
                        
                        currentProperty.owned = true; // Set property ownership                
                        currentProperty.ownerID = currentPlayer.playerID;                        
                        currentProperty.teamownerID = currentPlayer.teamID;

                        currentPlayer.UpdateMoneyText(); // Update money UI
                        currentPlayer.ownedProperties.Add(currentProperty); // Add property to player's properties list

                        currentPlayer.UpdatePropertyOwnership(stageIndex);

                        Debug.Log("Property bought successfully.");
                        currentProperty.CalculateRent(stageIndex);
                        gameObject.SetActive(false); // Close the Buy Property Popup
                        // playerController.EndBuyPropertyInteraction();
                        playerController.buyPropertyDecisionMade = true;
                        Debug.Log("buyPropertyDecisionMade set to : " + playerController.buyPropertyDecisionMade);
                        
                    }
                    else
                    {
                        Debug.LogWarning("Insufficient funds to buy the property.");
                        // playerController.EndBuyPropertyInteraction();
                        // playerController.EndTurn();
                        playerController.buyPropertyDecisionMade = true;
                        Debug.Log("buyPropertyDecisionMade set to : " + playerController.buyPropertyDecisionMade);
                    }
                }
                else
                {
                    Debug.LogWarning("Current player not found.");
                }
            }
            else
            {
                Debug.LogWarning("GameManager not found or invalid currentPlayerIndex.");
            }

            // Stop the buy confirmation timer coroutine if needed
            if (buyConfirmationCoroutine != null)
            {
                StopCoroutine(buyConfirmationCoroutine);
            }
            // playerController.EndBuyPropertyInteraction();
        }
        Debug.Log ("currentPlayerIndex:"+ GameManager.currentPlayerIndex);
    }

    IEnumerator BuyConfirmationTimer()
    {
        yield return new WaitForSeconds(buyConfirmationTime);

        // Close the popup after the confirmation time if no purchase is made
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        playerController.buyPropertyDecisionMade = true;
        Debug.Log("buyPropertyDecisionMade set to : " + playerController.buyPropertyDecisionMade);
    }

    public void Decline()
    {
        // Close the popup immediately when the close button is pressed
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        playerController.buyPropertyDecisionMade = true;
        Debug.Log("buyPropertyDecisionMade set to : " + playerController.buyPropertyDecisionMade);
        
    }
}
