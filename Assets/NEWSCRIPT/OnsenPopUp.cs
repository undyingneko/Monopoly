using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;

public class OnsenPopUp : MonoBehaviour
{
    [SerializeField]
    // private TextMeshProUGUI ownedByTeammateText;
    private TextMeshProUGUI propertyNameText;
    private Button closeButton;

    private TextMeshProUGUI HotSpringPriceText;
    private Button buyButton;

    private StallManager stallManager;
    private OnsenManager onsenManager;
    public PlayerController playerController;

    private PlayerController currentPlayer;
    private OnsenManager.OnsenData currentOnsen;


    private float buyConfirmationTime = 10f; // Time limit for confirming the purchase

    private Coroutine buyConfirmationCoroutine; 
    private GameManager gameManager;

    // public GameObject HotSpringImage;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        onsenManager = FindObjectOfType<OnsenManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return; 
        }
        stallManager = StallManager.Instance;

        if (stallManager == null)
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

        Transform HotSpringPriceTextTransform = transform.Find("HotSpringPriceText");
        if (HotSpringPriceTextTransform != null)
        {
            HotSpringPriceText = HotSpringPriceTextTransform.GetComponent<TextMeshProUGUI>();  
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

        if (playerController != null)
        {
            Debug.Log("Popup enabled");
            playerController.isHotSpringActive = true;
            buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
            closeButton.onClick.AddListener(Decline); // Add a listener to the close button
            buyButton.onClick.AddListener(() => BuyHotSpring(GameManager.currentPlayerIndex));
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
        int currentPlayerIndex = GameManager.currentPlayerIndex;
        if (currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
        {
            currentPlayer = gameManager.players[currentPlayerIndex];
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
    {   
        playerController.isHotSpringActive = false;
        Debug.Log("Popup disabled");
        if (buyConfirmationCoroutine != null)
        {
            StopCoroutine(buyConfirmationCoroutine);
        }
        buyButton.onClick.RemoveListener(() => BuyHotSpring(GameManager.currentPlayerIndex));
    }

    public void DisplayBuyHotSpring(OnsenManager.OnsenData hotspring)
    {
        if (hotspring == null)
        {
            Debug.LogError("PropertyData object is null!");
            return;
        }
        currentOnsen = hotspring;
        Debug.Log ("currentOnsen name:"+ currentOnsen.name);

        propertyNameText.text = hotspring.name;
        if (currentPlayer == null || currentOnsen == null)
        {
            Debug.LogError("Current player or property is null!");
            return;
        }
        HotSpringPriceText.text = "Price: " + FormatHotSpringPrice(currentOnsen); 
        buyConfirmationCoroutine = StartCoroutine(BuyConfirmationTimer());
    }


    private void BuyHotSpring(int currentPlayerIndex)
    {
            // int buyoutPrice = currentOnsen.buyoutPrices[stageIndex];
            int newhotspringprice = currentOnsen.CalculatePriceOnsen();
            
            if (gameManager != null && currentPlayerIndex >= 0 && currentPlayerIndex < gameManager.players.Length)
            {
                // PlayerController currentPlayer = gameManager.players[currentPlayerIndex];
                if (currentPlayer != null)
                {
                    if (currentPlayer.Money >= newhotspringprice)
                    {
                        currentPlayer.Money -= newhotspringprice; 
                        currentPlayer.UpdateMoneyText();
                        Debug.Log("Money deducted:" + newhotspringprice );

                        if (!currentOnsen.owned)
                        {
                            currentOnsen.owned = true;            
                            currentOnsen.ownerID = currentPlayer.playerID;                        
                            currentOnsen.teamownerID = currentPlayer.teamID;
                            currentPlayer.ownedOnsens.Add(currentOnsen);
                        }   
                        Debug.Log("Hot Spring bought successfully.");

                        gameObject.SetActive(false); 

                        // stallManager.DeactivateOldStageImages(currentOnsen);
                        // currentOnsen.stageImages[stageIndex].SetActive(true);
                        onsenManager.ActivateRentTagImage(currentOnsen);
                        onsenManager.UpdateonsenRentText(currentOnsen);
                        currentOnsen.OnsenImage.SetActive(true);
                        gameManager.OnsenDecisionMade = true;
                        Debug.Log("gameManager.OnsenDecisionMade set to : " + gameManager.OnsenDecisionMade);
                    }
                    else
                    {
                        Debug.LogWarning("Insufficient funds to buy the property.");
                        gameManager.OnsenDecisionMade = true;
                        Debug.Log("gameManager.OnsenDecisionMade set to : " + gameManager.OnsenDecisionMade);
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
        // playerController.EndBuyPropertyInteraction();
        gameManager.OnsenDecisionMade = true;
        Debug.Log("gameManager.OnsenDecisionMade set to : " + gameManager.OnsenDecisionMade);
    }
    public void Decline()
    {
        // Close the popup immediately when the close button is pressed
        gameObject.SetActive(false);
        // playerController.EndBuyPropertyInteraction();
        gameManager.OnsenDecisionMade = true;
        Debug.Log("gameManager.OnsenDecisionMade set to : " + gameManager.OnsenDecisionMade);
        
    }
    private string FormatHotSpringPrice (OnsenManager.OnsenData hotspring)
    {
        gameManager = FindObjectOfType<GameManager>();
        int priceHotSpring = hotspring.CalculateRentPriceOnsen();
        string formattedbuyoutPrice = gameManager.FormatPrice(priceHotSpring);
        return formattedbuyoutPrice;
    }

}

