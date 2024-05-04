using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class BuyPropertyPopup012 : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ownedByTeammateText;
    public TextMeshProUGUI BuyPropertyPopup_propertyNameText;
    public TextMeshProUGUI[] BuyPropertyPopup_stagePriceTexts;
    public Button[] BuyPropertyPopup_buyButtons;
    public Button BuyPropertyPopup_closeButton; // Reference to the close button
   
    private PlayerController playerController;
    private PlayerController currentPlayer;

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
        // GameManager gameManager = FindObjectOfType<GameManager>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return; // Exit the method if GameManager is not found to avoid null reference errors
        }
        int currentPlayerIndex = GameManager.currentPlayerIndex;

        if (currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
        {
            currentPlayer = gameManager.players[currentPlayerIndex];
            if (currentPlayer == null)
            {
                Debug.LogError("Current player not found!");
            }
        }
        else
        {
            Debug.LogError("Invalid currentPlayerIndex in GameManager!");
        }
        propertyManager = PropertyManager.Instance;
        ownedByTeammateText.gameObject.SetActive(false);
        
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
                BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
                BuyPropertyPopup_buyButtons[i].interactable = false;
            }

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
        if (property == null)
        {
            Debug.LogError("PropertyData object is null!");
            return;
        }
        currentProperty = property;
        Debug.Log ("currentProperty name:"+ currentProperty.name);

        BuyPropertyPopup_propertyNameText.text = property.name;
        // property.stageImageInstances.Clear();
    
        

        // Ensure the length of the stagePriceTexts array matches the number of prices in the property
        for (int i = 0; i < BuyPropertyPopup_stagePriceTexts.Length; i++)
        {
            if (i < property.stagePrices.Count)
            {
                if (currentProperty.owned && i <= currentProperty.currentStageIndex)
                {
                    // If the player owns the property and the current stage is already bought or lower, display an "Owned" mark
                    BuyPropertyPopup_stagePriceTexts[i].text = "Owned";
                    BuyPropertyPopup_buyButtons[i].interactable = false;
                    BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    // If the player doesn't own the property or the current stage is not bought, display the price and enable the buy button
                    BuyPropertyPopup_stagePriceTexts[i].text = "Price: " + property.stagePrices[i].ToString();
                    BuyPropertyPopup_buyButtons[i].interactable = true;
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
        if (currentProperty.owned && currentProperty.teamownerID == currentPlayer.teamID)
        {
            ownedByTeammateText.gameObject.SetActive(true);
            ownedByTeammateText.text = "Owned by your teammate";
            Debug.Log("ownedByTeammateText set to active");
        }
        else
        {
            ownedByTeammateText.gameObject.SetActive(false);
        }
        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }


    public void BuyStage(int stageIndex, int currentPlayerIndex)
    {
        if (!buyingStage)
        {
            buyingStage = true;
            int stagePrice = currentProperty.stagePrices[stageIndex];
            
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= stagePrice)
                    {
                        currentPlayer.Money -= stagePrice; // Deduct money
                        currentPlayer.UpdateMoneyText(); // Update money UI
                        Debug.Log("Money deducted successfully. Remaining money: " + currentPlayer.Money);
                        
                        if (!currentProperty.owned) // Check if property is not owned
                        {
                            currentProperty.owned = true; // Set property ownership                
                            currentProperty.ownerID = currentPlayer.playerID;                        
                            currentProperty.teamownerID = currentPlayer.teamID;
                            currentPlayer.ownedProperties.Add(currentProperty);
                            // currentPlayer.UpdatePropertyOwnership(stageIndex);
                        }                    

                        if (stageIndex >= currentProperty.currentStageIndex)
                        {
                            currentProperty.currentStageIndex = stageIndex;
                            currentProperty.nextStageIndex = stageIndex + 1;
                        }     

                        Debug.Log("Property bought successfully.");

                        gameObject.SetActive(false); 

                        propertyManager.DeactivateOldStageImages(currentProperty);
                        currentProperty.stageImages[stageIndex].SetActive(true);
                        propertyManager.ActivateRentTagImage(currentProperty);
                        propertyManager.UpdateRentText(currentProperty, stageIndex);

                        Debug.Log("JSONwaypointIndex = "+ currentProperty.JSONwaypointIndex + "+" + "currentStageIndex = " + currentProperty.currentStageIndex);
                        Debug.Log("Image Count " + currentProperty.stageImages.Count);
                        playerController.buyPropertyDecisionMade = true;
                        Debug.Log("buyPropertyDecisionMade set to : " + playerController.buyPropertyDecisionMade);
                        
                    }
                    else
                    {
                        Debug.LogWarning("Insufficient funds to buy the property.");
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
                Debug.LogWarning("GameManager reference is null in BuyStage method!");
                return;
            }

            // Stop the buy confirmation timer coroutine if needed
            if (buyConfirmationCoroutine != null)
            {
                StopCoroutine(buyConfirmationCoroutine);
            }
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

