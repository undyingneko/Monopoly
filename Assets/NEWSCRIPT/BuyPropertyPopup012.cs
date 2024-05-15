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
    public TextMeshProUGUI[] BuyPropertyPopup_stageNumberTexts;
    public Button BuyPropertyPopup_closeButton; // Reference to the close button

    private PropertyManager propertyManager;
    public PlayerController playerController;


    private PlayerController currentPlayer;
    private PropertyManager.PropertyData currentProperty;

    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; // Coroutine reference for buy confirmation timer
    private GameManager gameManager;

    // public Transform canvasTransform;
    public GameObject stageImagePrefab;

    
    // public PlayerController buyPropertyPopup;

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is not set.");
        }      
        // GameManager gameManager = FindObjectOfType<GameManager>();
        // playerController = GetComponentInParent<PlayerController>();
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
            Debug.LogError("PropertyManager reference is not set in BuyPropertyPopup012!");
            return;
        }
    }

    private void OnEnable()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }
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
        if (playerController != null)
        {
            Debug.Log("Popup enabled");
            playerController.isBuyPopUpActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            BuyPropertyPopup_closeButton.onClick.AddListener(Decline); 
            for (int i = 0; i < BuyPropertyPopup_buyButtons.Length; i++)
            {
                int index = i; 
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
        playerController.isBuyPopUpActive = false;
        if (buyConfirmationCoroutine != null)
        {
            StopCoroutine(buyConfirmationCoroutine);
        }
        BuyPropertyPopup_closeButton.onClick.RemoveListener(Decline);
        for (int i = 0; i < BuyPropertyPopup_buyButtons.Length; i++)
        {
            BuyPropertyPopup_buyButtons[i].onClick.RemoveAllListeners();
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

        if (currentPlayer == null || currentProperty == null)
        {
            Debug.LogError("Current player or property is null!");
            return;
        }
        if (currentProperty != null && currentPlayer != null)
        {
            if (currentProperty.owned && currentProperty.teamownerID == currentPlayer.teamID && currentProperty.ownerID != currentPlayer.playerID)
            {
                ownedByTeammateText.gameObject.SetActive(true);
            }
            else
            {
                ownedByTeammateText.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("currentProperty or currentPlayer is null!");
        }

        if (currentProperty.currentStageIndex < 2)
        {

        // Ensure the length of the stagePriceTexts array matches the number of prices in the property
            for (int i = 0; i < 3; i++)
            {
                if (i < property.stagePrices.Count)
                {
                    if (i == 0)
                    {
                        // Display "LAND" for stage 0
                        BuyPropertyPopup_stageNumberTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stageNumberTexts[i].text = "LAND";
                    }

                    else
                    {
                        // Display stage number for other stages
                        BuyPropertyPopup_stageNumberTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stageNumberTexts[i].text = "STAGE " + i; // Add 1 to stage index to display stage number
                    } 
                                
                    if (currentProperty.owned && i <= currentProperty.currentStageIndex)
                    {
                        // If the player owns the property and the current stage is already bought or lower, display an "Owned" mark
                        BuyPropertyPopup_stagePriceTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stagePriceTexts[i].text = "Owned";
                        BuyPropertyPopup_buyButtons[i].interactable = false;
                        BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        // If the player doesn't own the property or the current stage is not bought, display the price and enable the buy button
                        BuyPropertyPopup_stagePriceTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stagePriceTexts[i].text = "Price: " + FormatStagePrice(i, property);
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
        }
        else if (currentProperty.currentStageIndex == 2)
        {
            BuyPropertyPopup_buyButtons[3].interactable = true;
            BuyPropertyPopup_buyButtons[3].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[3].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[3].text = "Price: " + FormatStagePrice(3, property);         
            BuyPropertyPopup_stageNumberTexts[3].text = "STAGE " + 3;
        }
        else if (currentProperty.currentStageIndex == 3)
        {
            BuyPropertyPopup_buyButtons[4].interactable = true;
            BuyPropertyPopup_buyButtons[4].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[4].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[4].text = "Price: " + FormatStagePrice(4, property); 
            BuyPropertyPopup_stageNumberTexts[4].text = "HOTEL";
        }

        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }
    public void BuyStage(int stageIndex, int currentPlayerIndex)
    {
            int stagePrice = currentProperty.stagePrices[stageIndex];
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= stagePrice)
                    {
                        currentPlayer.Money -= stagePrice; 
                        currentPlayer.UpdateMoneyText(); 
                        Debug.Log("Money deducted successfully. Remaining money: " + currentPlayer.Money);
                        
                        if (!currentProperty.owned)
                        {
                            currentProperty.owned = true;            
                            currentProperty.ownerID = currentPlayer.playerID;                        
                            currentProperty.teamownerID = currentPlayer.teamID;
                            currentPlayer.ownedProperties.Add(currentProperty);
                        }                    
                        if (stageIndex > currentProperty.currentStageIndex)
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
                        gameManager.buyPropertyDecisionMade = true;
                        Debug.Log("gameManager.buyPropertyDecisionMade set to : " + gameManager.buyPropertyDecisionMade);
                    }
                    else
                    {
                        Debug.LogWarning("Insufficient funds to buy the property.");
                        gameManager.buyPropertyDecisionMade = true;
                        Debug.Log("gameManager.buyPropertyDecisionMade set to : " + gameManager.buyPropertyDecisionMade);
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
            if (buyConfirmationCoroutine != null)
            {
                StopCoroutine(buyConfirmationCoroutine);
            }
        
        Debug.Log ("currentPlayerIndex:"+ GameManager.currentPlayerIndex);
    }

    IEnumerator BuyConfirmationTimer()
    {
        yield return new WaitForSeconds(buyConfirmationTime);
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        gameManager.buyPropertyDecisionMade = true;
        Debug.Log("gameManager.buyPropertyDecisionMade set to : " + gameManager.buyPropertyDecisionMade);
    }

    public void Decline()
    {
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        gameManager.buyPropertyDecisionMade = true;
        Debug.Log("gameManager.buyPropertyDecisionMade set to : " + gameManager.buyPropertyDecisionMade);  
    }

    private string FormatStagePrice(int stageIndex, PropertyManager.PropertyData property)
    {
        gameManager = FindObjectOfType<GameManager>();
        int stagePrice = property.CalculateStagePrice(stageIndex);
        string formattedstagePrice = gameManager.FormatPrice(stagePrice);
        return formattedstagePrice;
    }

}

