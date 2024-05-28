using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class BuyPropertyPopup012 : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ownedByTeammateText;
    public TextMeshProUGUI BuyPropertyPopup_stallNameText;
    public TextMeshProUGUI[] BuyPropertyPopup_stagePriceTexts;
    public Button[] BuyPropertyPopup_buyButtons;
    public TextMeshProUGUI[] BuyPropertyPopup_stageNumberTexts;
    public Button BuyPropertyPopup_closeButton; // Reference to the close button

    private StallManager stallManager;
    public PlayerController playerController;


    private PlayerController currentPlayer;
    private StallManager.StallData currentStall;

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

        stallManager = StallManager.Instance;
        // ownedByTeammateText.gameObject.SetActive(false);

        // Ensure that the StallManager reference is not null
        if (stallManager == null)
        {
            Debug.LogError("StallManager reference is not set in BuyPropertyPopup012!");
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


        if (currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
        {
            currentPlayer = gameManager.players[currentPlayerIndex];
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
    {   
        // Debug.Log("Popup disabled");
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
        gameManager.buyPropertyDecisionMade = false;
           
    }

    public void Display012(StallManager.StallData stall)
    {
        if (stall == null)
        {
            Debug.LogError("StallData object is null!");
            return;
        }
        currentStall = stall;
        // Debug.Log ("currentStall name:"+ currentStall.name);

        BuyPropertyPopup_stallNameText.text = stall.name;
        // stall.stageImageInstances.Clear();  

        if (currentPlayer == null || currentStall == null)
        {
            Debug.LogError("Current player or stall is null!");
            return;
        }
        if (currentStall != null && currentPlayer != null)
        {
            if (currentStall.owned && currentStall.teamownerID == currentPlayer.teamID && currentStall.ownerID != currentPlayer.playerID)
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
            Debug.LogError("currentStall or currentPlayer is null!");
        }

        if (currentStall.currentStageIndex < 2)
        {

        // Ensure the length of the stagePriceTexts array matches the number of prices in the stall
            for (int i = 0; i < 3; i++)
            {
                if (i < stall.stagePrices.Count)
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
                                
                    if (currentStall.owned && i <= currentStall.currentStageIndex)
                    {
                        // If the player owns the stall and the current stage is already bought or lower, display an "Owned" mark
                        BuyPropertyPopup_stagePriceTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stagePriceTexts[i].text = "Owned";
                        BuyPropertyPopup_buyButtons[i].interactable = false;
                        BuyPropertyPopup_buyButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        // If the player doesn't own the stall or the current stage is not bought, display the price and enable the buy button
                        BuyPropertyPopup_stagePriceTexts[i].gameObject.SetActive(true);
                        BuyPropertyPopup_stagePriceTexts[i].text = "Price: " + FormatStagePrice(i, stall);
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
        else if (currentStall.currentStageIndex == 2)
        {
            BuyPropertyPopup_buyButtons[3].interactable = true;
            BuyPropertyPopup_buyButtons[3].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[3].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[3].text = "Price: " + FormatStagePrice(3, stall);         
            BuyPropertyPopup_stageNumberTexts[3].text = "STAGE " + 3;
        }
        else if (currentStall.currentStageIndex == 3)
        {
            BuyPropertyPopup_buyButtons[4].interactable = true;
            BuyPropertyPopup_buyButtons[4].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[4].gameObject.SetActive(true);
            BuyPropertyPopup_stagePriceTexts[4].text = "Price: " + FormatStagePrice(4, stall); 
            BuyPropertyPopup_stageNumberTexts[4].text = "HOTEL";
        }

        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }
    public void BuyStage(int stageIndex, int currentPlayerIndex)
    {
            int stagePrice = currentStall.stagePrices[stageIndex];
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= stagePrice)
                    {
                        currentPlayer.Money -= stagePrice; 
                        currentPlayer.UpdateMoneyText(); 

                        
                        if (!currentStall.owned)
                        {
                            currentStall.owned = true;            
                            currentStall.ownerID = currentPlayer.playerID;                        
                            currentStall.teamownerID = currentPlayer.teamID;
                            currentPlayer.ownedStalls.Add(currentStall);
                        }                    
                        if (stageIndex > currentStall.currentStageIndex)
                        {
                            currentStall.currentStageIndex = stageIndex;
                            currentStall.nextStageIndex = stageIndex + 1;
                        }     
                        gameObject.SetActive(false); 

                        stallManager.DeactivateOldStageImages(currentStall);
                        currentStall.stageImages[stageIndex].SetActive(true);
                        stallManager.ActivateRentTagImage(currentStall);
                        stallManager.UpdateRentText(currentStall, stageIndex);
                        gameManager.buyPropertyDecisionMade = true;

                    }
                    else
                    {
                        gameManager.buyPropertyDecisionMade = true;
                    }
                }
                else
                {
                    Debug.LogWarning("Current player not found.");
                }
            }
            else
            {

                return;
            }
            if (buyConfirmationCoroutine != null)
            {
                StopCoroutine(buyConfirmationCoroutine);
            }
    }

    IEnumerator BuyConfirmationTimer()
    {
        yield return new WaitForSeconds(buyConfirmationTime);
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        gameManager.buyPropertyDecisionMade = true;
    }

    public void Decline()
    {
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        gameManager.buyPropertyDecisionMade = true;

    }

    private string FormatStagePrice(int stageIndex, StallManager.StallData stall)
    {
        gameManager = FindObjectOfType<GameManager>();
        // int stagePrice = stall.CalculateStagePrice(stageIndex);
        int stagePrice = stall.stagePrices[stageIndex];
        string formattedstagePrice = gameManager.FormatPrice(stagePrice);
        return formattedstagePrice;
    }

}

