using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class BuyOutPopUp : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ownedByTeammateText;
    public TextMeshProUGUI propertyNameText;
    public Button closeButton;

    public TextMeshProUGUI[] stageBuyOutPriceTexts;
    public Button[] buyButtons;
    public TextMeshProUGUI[] stageNumberTexts;
     // Reference to the close button

    private PropertyManager propertyManager;
    private PlayerController playerController;


    private PlayerController currentPlayer;
    private PropertyManager.PropertyData currentProperty;


    private bool buyingStage; // Flag to track if the player is in the process of buying a stage
    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; // Coroutine reference for buy confirmation timer
    private GameManager gameManager;

    // public Transform canvasTransform;
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

        propertyManager = PropertyManager.Instance;
        // ownedByTeammateText.gameObject.SetActive(false);

        // Ensure that the PropertyManager reference is not null
        if (propertyManager == null)
        {
            Debug.LogError("PropertyManager reference is not set");
            return;
        }








    }

    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();

        Transform propertyNameTextTransform = transform.Find("PropertyNameText");
        if (propertyNameTextTransform != null)
        {
            propertyNameText = propertyNameTextTransform.GetComponent<TextMeshProUGUI>();
            
        }
        else
        {
            Debug.LogError("PropertyNameText not found in the instantiated prefab.");
        }

        Transform closeButtonTransform = transform.Find("CloseButton");
        if (closeButtonTransform != null)
        {
            closeButton = closeButtonTransform.GetComponent<Button>();
        }
        else
        {
            Debug.LogError("CloseButton not found in the instantiated prefab.");
        }    
        for (int i = 0; i < stageBuyOutPriceTexts.Length; i++)
        {
            // Find the stage buyout price text within the instantiated prefab
            string stageBuyOutPriceTextName = "stageBuyOutPriceText" + i;
            Transform stageBuyOutPriceTextTransform = transform.Find(stageBuyOutPriceTextName);
            // Assign the TextMeshProUGUI component if found
            if (stageBuyOutPriceTextTransform != null)
            {
                stageBuyOutPriceTexts[i] = stageBuyOutPriceTextTransform.GetComponent<TextMeshProUGUI>();
                stageBuyOutPriceTextTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Stage buyout price text not found for index " + i + " in the instantiated prefab.");
            }

            // Find the buy button within the instantiated prefab
            string buyButtonName = "BuyButton" + i;
            Transform buyButtonTransform = transform.Find(buyButtonName);
            // Assign the Button component if found
            if (buyButtonTransform != null)
            {
                buyButtons[i] = buyButtonTransform.GetComponent<Button>();
                buyButtonTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Buy button not found for index " + i + " in the instantiated prefab.");
            }

            // Find the stage number text within the instantiated prefab
            string stageNumberTextName = "StageNumberText" + i;
            Transform stageNumberTextTransform = transform.Find(stageNumberTextName);
            // Assign the TextMeshProUGUI component if found
            if (stageNumberTextTransform != null)
            {
                stageNumberTexts[i] = stageNumberTextTransform.GetComponent<TextMeshProUGUI>();
                stageNumberTextTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Stage number text not found for index " + i + " in the instantiated prefab.");
            }
        }  
        
        if (playerController != null)
        {
            Debug.Log("Popup enabled");
            playerController.isBuyPopUpActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            closeButton.onClick.AddListener(Decline); // Add a listener to the close button

            for (int i = 0; i < buyButtons.Length; i++)
            {
                buyButtons[i].gameObject.SetActive(false);
                buyButtons[i].interactable = false;
            }

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

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        // Get the current player index from the GameManager
        int currentPlayerIndex = GameManager.currentPlayerIndex;

        // Check if the current player index is valid
        if (currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
        {
            // Assign the current player using the player index
            currentPlayer = gameManager.players[currentPlayerIndex];

            // Check if the currentPlayer is null
            if (currentPlayer == null)
            {
                Debug.LogError("Current player not found!");
            }
            else
            {
                Debug.Log("Current player assigned: " + currentPlayer.name);
            }
        }
        else
        {
            Debug.LogError("Invalid currentPlayerIndex in GameManager!");
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
        Destroy(gameObject);
    }

    public void DisplayBuyOut(PropertyManager.PropertyData property)
    {
        if (property == null)
        {
            Debug.LogError("PropertyData object is null!");
            return;
        }
        currentProperty = property;
        Debug.Log ("currentProperty name:"+ currentProperty.name);

        propertyNameText.text = property.name;
        // property.stageImageInstances.Clear();  

        // Check if the current player and property are valid
        if (currentPlayer == null || currentProperty == null)
        {
            Debug.LogError("Current player or property is null!");
            return;
        }

        if (currentProperty.currentStageIndex < 2)
        {

        // Ensure the length of the stagePriceTexts array matches the number of prices in the property
            for (int i = 0; i < 3; i++)
            {
                if (i < property.buyoutPrices.Count)
                {
                    if (i == 0)
                    {
                        // Display "LAND" for stage 0
                        stageNumberTexts[i].gameObject.SetActive(true);
                        stageNumberTexts[i].text = "LAND";
                    }
                    else
                    {
                        // Display stage number for other stages
                        stageNumberTexts[i].gameObject.SetActive(true);
                        stageNumberTexts[i].text = "STAGE " + i; // Add 1 to stage index to display stage number
                    } 
                                
                    if (currentProperty.owned && i <= currentProperty.currentStageIndex)
                    {
                        // If the player owns the property and the current stage is already bought or lower, display an "Owned" mark
                        stageBuyOutPriceTexts[i].gameObject.SetActive(true);
                        stageBuyOutPriceTexts[i].text = "Owned";
                        buyButtons[i].interactable = false;
                        buyButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        // If the player doesn't own the property or the current stage is not bought, display the price and enable the buy button
                        stageBuyOutPriceTexts[i].gameObject.SetActive(true);
                        stageBuyOutPriceTexts[i].text = "Price: " + property.buyoutPrices[i].ToString();
                        buyButtons[i].interactable = true;
                        buyButtons[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    // If there are more stageBuyOutPriceTexts than prices, deactivate the extra buttons
                    stageBuyOutPriceTexts[i].gameObject.SetActive(false);
                    buyButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else if (currentProperty.currentStageIndex == 2)
        {
            buyButtons[3].interactable = true;
            buyButtons[3].gameObject.SetActive(true);
            stageBuyOutPriceTexts[3].gameObject.SetActive(true);
            stageBuyOutPriceTexts[3].text = "Price: " + property.buyoutPrices[3].ToString();           
            stageNumberTexts[3].text = "STAGE " + 3;
        }
        else if (currentProperty.currentStageIndex == 3)
        {
            buyButtons[4].interactable = true;
            buyButtons[4].gameObject.SetActive(true);
            stageBuyOutPriceTexts[4].gameObject.SetActive(true);
            stageBuyOutPriceTexts[4].text = "Price: " + property.buyoutPrices[4].ToString();  
            stageNumberTexts[4].text = "HOTEL";
        }

        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }


    public void BuyStage(int stageIndex, int currentPlayerIndex)
    {
        if (!buyingStage)
        {
            buyingStage = true;
            int buyoutPrice = currentProperty.buyoutPrices[stageIndex];
            
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= buyoutPrice)
                    {
                        PlayerController ownerPlayer = playerController.FindPlayerByID(currentProperty.ownerID);
                        currentPlayer.Money -= buyoutPrice; 
                        ownerPlayer.Money += buyoutPrice;
                        
                       
                        currentPlayer.UpdateMoneyText();
                        ownerPlayer.UpdateMoneyText();

                        Debug.Log("Money deducted successfully. Remaining money: " + currentPlayer.Money);
                        Debug.Log("Money added successfully. Remaining money: " + ownerPlayer.Money);

                        ownerPlayer.ownedProperties.Remove(currentProperty);
                        currentPlayer.ownedProperties.Add(currentProperty);

                        currentProperty.ownerID = currentPlayer.playerID;
                        currentProperty.teamownerID = currentPlayer.teamID; 

                        currentProperty.buyoutCount += 1;

                        Debug.Log("Property bought out successfully.");

                        gameObject.SetActive(false); 

                        // propertyManager.DeactivateOldStageImages(currentProperty);
                        // currentProperty.stageImages[stageIndex].SetActive(true);
                        propertyManager.ActivateRentTagImage(currentProperty);
                        // propertyManager.UpdateRentText(currentProperty, stageIndex);

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

