using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class BuyOutPopUp : MonoBehaviour
{
    [SerializeField]
    // private TextMeshProUGUI ownedByTeammateText;
    private TextMeshProUGUI propertyNameText;
    private Button closeButton;

    private TextMeshProUGUI stageBuyOutPriceText;
    private Button buyButton;
    private TextMeshProUGUI stageNumberText;


    private StallManager propertyManager;
    public PlayerController currentPlayer;
    private StallManager.StallData currentProperty;

    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; 
    private GameManager gameManager;

    public GameObject stageImagePrefab;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return; 
        }
        propertyManager = StallManager.Instance;

        if (propertyManager == null)
        {
            Debug.LogError("StallManager reference is not set");
            return;
        }
    }

    private void OnEnable()
    {
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

        Transform stageBuyOutPriceTextTransform = transform.Find("stageBuyOutPriceText");
        if (stageBuyOutPriceTextTransform != null)
        {
            stageBuyOutPriceText = stageBuyOutPriceTextTransform.GetComponent<TextMeshProUGUI>();  
        }
        else
        {
            Debug.LogError("Stage buyout price text not found ");
        }
         Transform buyButtonTransform = transform.Find("BuyButton");
        if (buyButtonTransform != null)
        {
            buyButton = buyButtonTransform.GetComponent<Button>();
            
        }
        else
        {
            Debug.LogError("Buy button not found");
        }    

        Transform stageNumberTextTransform = transform.Find("StageNumberText");
        if (stageNumberTextTransform != null)
        {
            stageNumberText = stageNumberTextTransform.GetComponent<TextMeshProUGUI>();   
        }
        else
        {
            Debug.LogError("Stage number text not found");
        }

        if (currentPlayer != null)
        {
            Debug.Log("Popup enabled");
            currentPlayer.isBuyPopUpActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            closeButton.onClick.AddListener(Decline); // Add a listener to the close button
            buyButton.onClick.AddListener(() => BuyOut(GameManager.currentPlayerIndex));
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
    }

    private void OnDisable()
    {   
        currentPlayer.isBuyPopUpActive = false;
        Debug.Log("Popup disabled");
        if (buyConfirmationCoroutine != null)
        {
            StopCoroutine(buyConfirmationCoroutine);
        }
        buyButton.onClick.RemoveListener(() => BuyOut(GameManager.currentPlayerIndex));
    }

    public void DisplayBuyOut(StallManager.StallData stall)
    {
        if (stall == null)
        {
            Debug.LogError("StallData object is null!");
            return;
        }
        currentProperty = stall;
        Debug.Log ("currentProperty name:"+ currentProperty.name);

        propertyNameText.text = stall.name;
        if (currentPlayer == null || currentProperty == null)
        {
            Debug.LogError("Current player or stall is null!");
            return;
        }

        // int buyoutPrice = currentProperty.CalculateBuyoutPrice(currentProperty.currentStageIndex);

        if (currentProperty.currentStageIndex < 2)
        {
            if (currentProperty.currentStageIndex == 0)
            {
                // Display "LAND" for stage 0
                stageBuyOutPriceText.text = "Price: " + FormatBuyoutPrice(currentProperty.currentStageIndex, currentProperty); 
                stageNumberText.text = "LAND";
            }
            else if (currentProperty.currentStageIndex == 1)
            {
                // Display "LAND" for stage 0
                stageBuyOutPriceText.text = "Price: " + FormatBuyoutPrice(currentProperty.currentStageIndex, currentProperty); 
                stageNumberText.text = "STAGE 1";
            }          
        }
        else if (currentProperty.currentStageIndex == 2)
        {

            stageBuyOutPriceText.text = "Price: " + FormatBuyoutPrice(currentProperty.currentStageIndex, currentProperty);           
            stageNumberText.text = "STAGE 3";
        }
        else if (currentProperty.currentStageIndex == 3)
        {
            stageBuyOutPriceText.text = "Price: " + FormatBuyoutPrice(currentProperty.currentStageIndex, currentProperty);  
            stageNumberText.text = "HOTEL";
        }
        else
        {
            // If there are more stageBuyOutPriceTexts than prices, deactivate the extra buttons
            stageBuyOutPriceText.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
        }
        // Start the buy confirmation timer coroutine
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }


    public void BuyOut(int currentPlayerIndex)
    {
            int stageIndex = currentProperty.currentStageIndex;
            // int buyoutPrice = currentProperty.buyoutPrices[stageIndex];
            int buyoutPrice = currentProperty.CalculateBuyoutPrice(stageIndex);
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= buyoutPrice)
                    {
                        PlayerController ownerPlayer = currentPlayer.FindPlayerByID(currentProperty.ownerID);
                        currentPlayer.Money -= buyoutPrice; 
                        ownerPlayer.Money += buyoutPrice;
                        
                        currentPlayer.UpdateMoneyText();
                        ownerPlayer.UpdateMoneyText();

                        Debug.Log("Money deducted:" + buyoutPrice );

                        ownerPlayer.ownedStalls.Remove(currentProperty);
                        currentPlayer.ownedStalls.Add(currentProperty);

                        currentProperty.ownerID = currentPlayer.playerID;
                        currentProperty.teamownerID = currentPlayer.teamID; 

                        currentProperty.buyoutCount += 1;
                        
                        Debug.Log("Stall bought out successfully.");

                        gameObject.SetActive(false); 

                        // propertyManager.DeactivateOldStageImages(currentProperty);
                        // currentProperty.stageImages[stageIndex].SetActive(true);
                        propertyManager.ActivateRentTagImage(currentProperty);
                        propertyManager.UpdateRentText(currentProperty, stageIndex);

                        gameManager.buyOutDecisionMade = true;
                        Debug.Log("gameManager.buyOutDecisionMade set to : " + gameManager.buyOutDecisionMade);
                    }
                    else
                    {
                        Debug.LogWarning("Insufficient funds to buy the stall.");
                        gameManager.buyOutDecisionMade = true;
                        Debug.Log("gameManager.buyOutDecisionMade set to : " + gameManager.buyOutDecisionMade);
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

        // Close the popup after the confirmation time if no purchase is made
        gameObject.SetActive(false);
        // currentPlayer.EndBuyPropertyInteraction();
        gameManager.buyOutDecisionMade = true;
        Debug.Log("gameManager.buyOutDecisionMade set to : " + gameManager.buyOutDecisionMade);
    }
    public void Decline()
    {
        // Close the popup immediately when the close button is pressed
        gameObject.SetActive(false);
        // currentPlayer.EndBuyPropertyInteraction();
        gameManager.buyOutDecisionMade = true;
        Debug.Log("gameManager.buyOutDecisionMade set to : " + gameManager.buyOutDecisionMade);
        
    }
    private string FormatBuyoutPrice (int stageIndex, StallManager.StallData stall)
    {
        gameManager = FindObjectOfType<GameManager>();
        int buyoutPrice = stall.CalculateBuyoutPrice(stageIndex);
        string formattedbuyoutPrice = gameManager.FormatPrice(buyoutPrice);
        return formattedbuyoutPrice;
    }

}

