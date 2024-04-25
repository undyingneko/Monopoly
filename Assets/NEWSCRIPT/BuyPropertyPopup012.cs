using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class BuyPropertyPopup012 : MonoBehaviour
{
    public TextMeshProUGUI BuyPropertyPopup_propertyNameText;
    public TextMeshProUGUI[] BuyPropertyPopup_stagePriceTexts;
    public Button[] BuyPropertyPopup_buyButtons;
    public Button BuyPropertyPopup_closeButton; // Reference to the close button
   
    private PlayerController playerController;

    private PropertyManager.PropertyData currentProperty;
    private bool buyingStage; // Flag to track if the player is in the process of buying a stage
    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; // Coroutine reference for buy confirmation timer
    private GameManager gameManager;
    private PropertyManager propertyManager;
    public Transform canvasTransform;
    public GameObject stageImagePrefab;






    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        propertyManager = PropertyManager.Instance;
        // propertyManager.LoadAllStageImages();

        // Ensure that the PropertyManager reference is not null
        if (propertyManager == null)
        {
            Debug.LogError("PropertyManager reference is not set in BuyPropertyPopup012!");
            return;
        }
    }
    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Popup enabled");
            playerController.isBuyPopUpActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            BuyPropertyPopup_closeButton.onClick.AddListener(Decline); // Add a listener to the close button
            for (int i = 0; i < BuyPropertyPopup_buyButtons.Length; i++)
            {
                int index = i; // Store the current index in a local variable to avoid closure issues
                BuyPropertyPopup_buyButtons[i].onClick.AddListener(() => BuyStage(index, GameManager.currentPlayerIndex));

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



    public void Display012(PropertyManager.PropertyData property)
    {
        currentProperty = property;

        BuyPropertyPopup_propertyNameText.text = property.name;
        // property.stageImageInstances.Clear();

        // Ensure the length of the stagePriceTexts array matches the number of prices in the property
        for (int i = 0; i < BuyPropertyPopup_stagePriceTexts.Length; i++)
        {
            if (i < property.prices.Count)
            {
                if (currentProperty.owned && i <= currentProperty.currentStageIndex)
                {
                    // If the player owns the property and the current stage is already bought or lower, display an "Owned" mark
                    BuyPropertyPopup_stagePriceTexts[i].text = "Owned";
                    BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    // If the player doesn't own the property or the current stage is not bought, display the price and enable the buy button
                    BuyPropertyPopup_stagePriceTexts[i].text = "Price: " + property.prices[i].ToString();
                    BuyPropertyPopup_buyButtons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // If there are more stagePriceTexts than prices, deactivate the extra buttons
                BuyPropertyPopup_stagePriceTexts[i].gameObject.SetActive(false);
                BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
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

                        if (stageIndex > currentProperty.currentStageIndex)
                        {
                            currentProperty.currentStageIndex = stageIndex;
                        }     
                        Debug.Log("currentStageIndex: " + currentProperty.currentStageIndex);                  

                        currentPlayer.UpdateMoneyText(); // Update money UI
                        currentPlayer.ownedProperties.Add(currentProperty); // Add property to player's properties list

                        currentPlayer.UpdatePropertyOwnership(stageIndex);

                        Debug.Log("Property bought successfully.");
                        currentProperty.CalculateRent(stageIndex);
                        gameObject.SetActive(false); // Close the Buy Property Popup
                        // playerController.EndBuyPropertyInteraction();
                        GameObject stageImageInstance = Instantiate(currentProperty.stageImages[stageIndex], canvasTransform);
                        Debug.Log("JSONwaypointIndex = "+ currentProperty.JSONwaypointIndex + "+" + "currentStageIndex = " + currentProperty.currentStageIndex);
                        Debug.Log("Image Count " + currentProperty.stageImages.Count);
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
    
// public void ActivateBoughtStageImage(int stageIndex)
// {
//     if (currentProperty != null)
//     {
//         // Deactivate all stage images first
//         DeactivateAllStageImages(currentProperty);

        
//         // Check if the stage index is within the bounds of the stageImages list
//         if (stageIndex >= 0 && stageIndex < currentProperty.stageImages.Count)
//         {
//             // Activate the image GameObject at the specified index
//             GameObject stageImage = currentProperty.stageImages[stageIndex];
//             if (stageImage != null)
//             {
//                 stageImage.SetActive(true);
//                 Debug.Log("Activated stage image for JSONwaypointIndex: " + currentProperty.JSONwaypointIndex + ", Stage Index: " + stageIndex);
//             }
//             else
//             {
//                 Debug.LogWarning("Image GameObject is null.");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("Invalid stage index: " + stageIndex);
//         }
//     }
//     else
//     {
//         Debug.LogWarning("Property is null. Cannot activate stage image.");
//     }
// }




// private void DeactivateAllStageImages(PropertyManager.PropertyData property)
// {
//     // Iterate through each stage image in the list and deactivate it
//     foreach (var stageImage in property.stageImages)
//     {
//         stageImage.SetActive(false);
//     }
// }

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
