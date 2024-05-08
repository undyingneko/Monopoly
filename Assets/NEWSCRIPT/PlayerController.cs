using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // private PlayerController currentPlayerController;
    public int playerID;
    public int teamID;
    public TextMeshProUGUI teamNumberText;

    private int currentPosition;
    private bool isTurn;

    public Button rollButton;
    public Image[] diceImages;
    public TextMeshProUGUI sumText;
    
    private Sprite[] diceSides;
    private bool coroutineAllowed = true;

    private GameManager gameManager;

    private bool loopCompleted = false;
    
    public Transform[] waypoints;
    [SerializeField]
    private float moveSpeed = 1f;
    [HideInInspector]
    public int waypointIndex = 0;
    public bool moveAllowed = false;
    public int Money = 2000;
    public TextMeshProUGUI plus300TextPrefab;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI playerMoveText;
    private int consecutiveDoublesCount = 0;
    public bool isDoubles = false;
    public bool InJail = false;
    private int turnsInJail = 0;
    public TextMeshProUGUI goToJailText;

    public Transform canvasTransform;
    
    private BuyPropertyPopup012 buyPropertyPopup012Prefab;
    public string buyPropertyPopup012PrefabPath = "BuyPropertyPopup012Prefab"; // Path to the prefab in the Resources folder

    public string buyoutPopupPrefabPath = "buyoutPopupPrefab"; // The path to the MessagePrefab relative to the Resources folder
    private BuyOutPopUp buyoutPopupPrefab;

    // private BuyPropertyPopup012 buyPopupInstance;
    // private BuyOutPopUp buyoutPopupInstance;
    

    private PropertyManager propertyManager;

    public List<PropertyManager.PropertyData> properties;
    public List<PropertyManager.PropertyData> ownedProperties = new List<PropertyManager.PropertyData>();
    

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder = 0;
    
    [SerializeField]
    private string MessagePrefabPath = "MessagePrefab";
    private GameObject MessagePrefab; 

    public bool isBuyPopUpActive = false;
 

    public void AssignPlayerID(int id)
    {
        playerID = id;
    }    
    public void AssignTeamID(int teamid)
    {
        teamID = teamid;
        teamNumberText.text = "Team: " + teamID.ToString();
    }

    // public void UpdatePropertyOwnership(int propertystageIndex)
    // {
    //     for (int i = 0; i <= propertystageIndex; i++)
    //     {
    //         for (int j = 0; j <= i; j++)
    //         {
    //             property.stageIndexes[i].owned = true;
    //         }
    //     }
    // }

    [SerializeField]
    private string CardPrefabPath = "CardPrefab";
    private GameObject CardPrefab; 

    public List<string> cards;
    // private List<string> cards = new List<string> 
    private List<string> cardDescriptions = new List<string>
    {
        "Collect a Birthday Gift of $15 from each"
        // "Get out of jail Ticket",
        // "Go Collect Dinner Ticket",
        // "Festival Ticket",
        // "Jump Further to Start",

        // "Demolish one avenue and leave it ownerless.",
        // "Power cut 1 opponent avenue",
        // "Force one opponent to sell one property of your choice from their holdings",
        // "You win a lottery of $200,000.",
        // "No tax for the next time",
        // "You don't have to pay fee for the next time",
        // "Go 1 space forward",


        // "Go to jail",
        // "Pay Dog Shit Fee of $50,000.",
        // "Jump Back To Start",
        // "An earthquake has destroyed 1 of your avenues",
        // "You have to sell one property of your choice from your holdings",
        // "Pay Tax!!!",
        // "Go back 1 space"
        // // Add other cards here...
    };




  
    void Start()
    {   
        // currentPlayerController = FindObjectOfType<PlayerController>();
        propertyManager = PropertyManager.Instance;
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(RollDiceOnClick);
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        transform.position = waypoints[waypointIndex].transform.position;
        
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);

        buyPropertyPopup012Prefab = Resources.Load<BuyPropertyPopup012>("BuyPropertyPopup012Prefab");
        if (buyPropertyPopup012Prefab != null)
        {
            Debug.Log("Popup prefab loaded successfully.");
            
        }
        else
        {
            Debug.LogError("Failed to load popup prefab.");
        }
        buyoutPopupPrefab = Resources.Load<BuyOutPopUp>("buyoutPopupPrefab");
        if (buyoutPopupPrefab != null)
        {
            Debug.Log("Buyout popup prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load buyout popup prefab.");
        }


        properties = propertyManager.properties;
        if (propertyManager == null)
        {
            Debug.LogError("propertyManager is not assigned. Assign it in the Unity Editor or via script.");
        }        
 
        properties = propertyManager.properties;
        if (propertyManager == null)
        {
            Debug.LogError("propertyManager is not assigned. Assign it in the Unity Editor or via script.");
        }

        // Ensure waypoints array is assigned and not empty
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("waypoints array is not properly assigned or is empty. Assign it in the Unity Editor or via script.");
        }  
        Money = 2000;
        UpdateMoneyText();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;

        MessagePrefab = Resources.Load<GameObject>(MessagePrefabPath);
        if (MessagePrefab == null)
        {
            Debug.LogError("Failed to load MessagePrefab from Resources folder at path: " + MessagePrefabPath);
        }
        
        CardPrefab = Resources.Load<GameObject>(CardPrefabPath);
        if (CardPrefab == null)
        {
            Debug.LogError("Failed to load MessagePrefab from Resources folder at path: " + MessagePrefabPath);
        }
        
    }
    private void InstantiateBuyPropertyPopup012(PropertyManager.PropertyData property)
    {
        // Find the Canvas GameObject
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas GameObject not found.");
            return;
        }

        // Instantiate the buy property popup and assign it to a variable
        BuyPropertyPopup012 BuypopupInstance  = Instantiate(buyPropertyPopup012Prefab, canvas.transform);

        if (BuypopupInstance  != null)
        {
            Debug.Log("Buy property popup instantiated successfully.");

            BuypopupInstance .Display012(property);

            Debug.Log("Buy property decision made.");
        }
        else
        {
            Debug.LogError("Failed to instantiate the buy property popup.");
            return;
        }
    }

    public void InstantiateBuyoutPopup(PropertyManager.PropertyData property)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas GameObject not found.");
            return;
        }
        // Instantiate the buyout popup prefab
        BuyOutPopUp buyoutPopupInstance = Instantiate(buyoutPopupPrefab, canvas.transform);
 
        if (buyoutPopupInstance != null)
        {
            Debug.Log("Buy property popup instantiated successfully.");
            buyoutPopupInstance.DisplayBuyOut(property);

        }
        else
        {
            Debug.LogError("Failed to instantiate the buy property popup.");
            return;
        }
    }

    private void RollDiceOnClick()
    {
        if (!GameManager.GameOver && isTurn && coroutineAllowed)
        {
            if (InJail)
            {
                StartCoroutine(RollDiceInJail());
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(RollTheDice());
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator RollDiceInJail()
    {    
        Debug.Log("inside roll dice in jail");
        Debug.Log("current position step in jail=" + currentPosition);   
        coroutineAllowed = false;
        int[] diceValues = new int[2];

        // For testing purposes, set the dice values to double 6
        // diceValues[0] = 4;
        // diceValues[1] = 4;
        // for (int i = 0; i <= 20; i++)
        // {
        //     for (int j = 0; j < diceImages.Length; j++)
        //     {
        //         diceImages[j].sprite = diceSides[3]; // Use the sprite for dice side 6
        //     }

        //     yield return new WaitForSeconds(0.05f);
        // }     
        // //---------------------

        //------------------------------------------
        for (int i = 0; i <= 20; i++)
        {
            for (int j = 0; j < diceImages.Length; j++)
            {
                int randomDiceSide = Random.Range(0, 6);
                diceImages[j].sprite = diceSides[randomDiceSide];
                diceValues[j] = randomDiceSide + 1;
            }

            yield return new WaitForSeconds(0.05f);
        }
        //-------------------------------------
        int sum = diceValues[0] + diceValues[1];
        sumText.text = "" + sum; 
        isDoubles = (diceValues[0] == diceValues[1]); 

        if (isDoubles)
        {
            InJail = false;
            turnsInJail = 0;
            Debug.Log("current position before moving in jail" + currentPosition); 
            Debug.Log("sum= " + sum);
            MovePlayer(sum);
            yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            Debug.Log("current position after moving in jail" + currentPosition); 
            EndTurn();
            coroutineAllowed = false;
            
            
        }
        else
        {
            if (turnsInJail >= 3)
            {   
                InJail = false;
                MovePlayer(diceValues[0] + diceValues[1]);
                turnsInJail = 0;
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
                EndTurn();
            
                coroutineAllowed = false;
                
            }
            else
            {   
                turnsInJail++;
                EndTurn();
            }
        }
        coroutineAllowed = true; 
    }

    private IEnumerator RollTheDice()
    {
        Debug.Log("inside roll dice normal");
        coroutineAllowed = false;
        int[] diceValues = new int[2];

        //-----------------------
        // for (int i = 0; i <= 20; i++)
        // {
        //     for (int j = 0; j < diceImages.Length; j++)
        //     {
        //         int randomDiceSide = Random.Range(0, 6);
        //         diceImages[j].sprite = diceSides[randomDiceSide];
        //         diceValues[j] = randomDiceSide + 1;
        //     }
        //     yield return new WaitForSeconds(0.05f);
        // }
        //---------------------
        // For testing purposes, set the dice values to double 6
        diceValues[0] = 7;
        diceValues[1] = 5;
        for (int i = 0; i <= 20; i++)
        {
            for (int j = 0; j < diceImages.Length; j++)
            {
                diceImages[j].sprite = diceSides[3]; // Use the sprite for dice side 6
            }

            yield return new WaitForSeconds(0.05f);
        }      
        //---------------------

        int sum = diceValues[0] + diceValues[1];
        // int sum = 7;
        sumText.text = "" + sum; 
        yield return new WaitForSeconds(0.1f);

        yield return MovePlayerCoroutine(sum);
        // MovePlayer(diceValues[0] + diceValues[1]);

        if (currentPosition == 8)
        {
            DisplayGoToJailText();
            InJail = true;
            consecutiveDoublesCount = 0;
            EndTurn();
            coroutineAllowed = false;
            yield break;
             // Exit the coroutine early if the player is in jail
        }
        
        if (currentPosition == 12 || currentPosition == 20 || currentPosition == 23 || currentPosition == 28)
        {
            yield return StartCoroutine(DrawCard());   
        }        
        // CheckForDoubles(diceValues);
        
        yield return StartCoroutine(CheckForDoubles(diceValues));

        coroutineAllowed = true; 

    }

    public void HackRollDice(int[] diceValues)
    {
        CheckForDoubles(diceValues);
        
    }

    private IEnumerator CheckForDoubles(int[] diceValues)
    {   
        isDoubles = (diceValues[0] == diceValues[1]);

        if (isDoubles)
        {   
            Debug.Log ("inside isDouble");
            consecutiveDoublesCount++;
            
            if (consecutiveDoublesCount >= 3)
            {   
                
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
                consecutiveDoublesCount = 0;
                waypointIndex = 8;
                transform.position = waypoints[waypointIndex].position;
                DisplayGoToJailText();
                InJail = true;
                
                EndTurn();
                yield break;
                // coroutineAllowed = false;

            }
            else
            {   
                
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
                StartTurn();
                
            }
        }

        else
        {   
            consecutiveDoublesCount = 0;
            // MovePlayer(diceValues[0] + diceValues[1]);
            yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            EndTurn();
            yield break;
            // if (buyPropertyDecisionMade)
            // {
            //     yield return new WaitUntil(() => !isBuyPopUpActive); // Wait until the buy pop-up interaction is completed
            //     EndTurn(); // End the turn after the buy pop-up interaction is completed
            //     yield break;
            // }
        }
        
    }

    void MovePlayer(int steps)
    {
        StartCoroutine(MovePlayerCoroutine(steps));
        
    }

    IEnumerator MovePlayerCoroutine(int steps)
    {
        int remainingSteps = steps;

        while (remainingSteps > 0)
        {
            float stepDistance = moveSpeed * Time.deltaTime;
            Vector2 targetPosition = waypoints[(waypointIndex + 1) % waypoints.Length].position;
            float distanceToNextWaypoint = Vector2.Distance(transform.position, targetPosition);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, stepDistance);

  
            if (distanceToNextWaypoint < stepDistance)
            {

                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                remainingSteps--;

                if (waypointIndex == 0)
                {
                    // Ensure the loopCompleted flag is false before adding money
                    if (!loopCompleted)
                    {
                        Money += 300;
                        DisplayPlus300();
                        UpdateMoneyText();

                        // Set the loopCompleted flag to true to prevent multiple additions
                        loopCompleted = true;
                    }
                }
                else
                {
                    // Reset the loopCompleted flag if the player is not at waypoint 0
                    loopCompleted = false;
                }
                // if (waypointIndex == 0)
                // {
                //     // Ensure the loopCompleted flag is false before adding money
                //     Money += 300;
                //     DisplayPlus300();
                //     UpdateMoneyText();
                // }
            }

            yield return null;
        }

        // Update the current position
        currentPosition = (currentPosition + steps) % waypoints.Length;
        StartCoroutine(LandOnProperty());
    }

    public IEnumerator DrawCard()
    {
        string cardDescription = cardDescriptions[Random.Range(0, cardDescriptions.Count)];
        GameObject cardObject = Instantiate(CardPrefab, canvasTransform);

        // Set the card description text
        TextMeshProUGUI descriptionText = cardObject.GetComponentInChildren<TextMeshProUGUI>();
        if (descriptionText != null)
        {
            descriptionText.text = cardDescription;
        }
        else
        {
            Debug.LogError("Description text component not found on card prefab.");
        }
        yield return new WaitForSeconds(3f);
        Destroy(cardObject);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ApplyCardEffects(cardDescription));    
    }

    public IEnumerator ApplyCardEffects(string cardDescription)
    {
        // Implement logic to apply the effects based on the card description
        // For example:
        if (cardDescription.Contains("Birthday Gift"))
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            PlayerController currentPlayer = gameManager.GetCurrentPlayerController();
            currentPlayer.Money += 45; // Increase money by $15
            currentPlayer.UpdateMoneyText(); // Update UI to reflect the new money amount
            currentPlayer.ShowMessage("Collect a Birthday Gift of $15 from each player");
            foreach (PlayerController player in players)
            {
                if (player != currentPlayer) // Exclude the current player
                {
                    player.Money -= 15; // Deduct $15 from the player
                    player.UpdateMoneyText(); // Update UI to reflect the new money amount for the player
                }
            }

            yield return new WaitForSeconds(2f);
        }
        // Add similar logic for other card effects...
    }


    private IEnumerator LandOnProperty()
    {
        // Check if propertyManager or waypoints array is null
        PropertyManager.PropertyData property = propertyManager.GetPropertyByWaypointIndex(currentPosition);
        if (propertyManager == null)
        {
            Debug.LogError("propertyManager is null.");
            yield break;
        }

        if (waypoints == null)
        {
            Debug.LogError("waypoints array is null.");
            yield break;
        }

        if (property == null)
        {
            Debug.LogWarning("Property is null. No popup will be displayed.");
            gameManager.buyPropertyDecisionMade = true;
            yield break;
        }

        // Check if the property is unowned and the player has enough money to buy it
        if (!property.owned && property.nextStageIndex <= 5 && property.stagePrices[property.nextStageIndex] <= Money)
        {
            // Instantiate the buy property popup
            InstantiateBuyPropertyPopup012(property);
        }
        else if (property.owned && property.currentStageIndex < 5)
        {
            PlayerController ownerPlayer = FindPlayerByID(property.ownerID);

            if (ownerPlayer != null && ownerPlayer.teamID == this.teamID)
            {
                if (property.nextStageIndex <= property.stagePrices.Count && property.stagePrices[property.nextStageIndex] <= Money)
                {
                    InstantiateBuyPropertyPopup012(property);
                }
                else if (Money < property.stagePrices[property.nextStageIndex])
                {
                    ShowMessage("Not enough money to acquire this property!");
                    yield return new WaitForSeconds(2f);
                    gameManager.buyPropertyDecisionMade = true;
                    yield break;
                }
            }
            else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
            {
                int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];
                Money -= rentPriceToDeduct;
                ownerPlayer.Money += rentPriceToDeduct;
                UpdateMoneyText();
                ownerPlayer.UpdateMoneyText();

                GameObject rentMessageObject = Instantiate(MessagePrefab, canvasTransform);
                TextMeshProUGUI RentMessageText = rentMessageObject.GetComponentInChildren<TextMeshProUGUI>();
                RentMessageText.text = "You pay a rent of $" + rentPriceToDeduct;
                yield return new WaitForSeconds(1f);
                Destroy(rentMessageObject);
                yield return new WaitForSeconds(1f);

                if (property.currentStageIndex < 4)
                {
                    if (property.buyoutPrices[property.currentStageIndex] <= Money )
                    {
                        InstantiateBuyoutPopup(property);
                    }
                    else if (property.buyoutPrices[property.currentStageIndex] > Money)
                    {
                        ShowMessage("Not enough money to acquire this property!");
                        yield return new WaitForSeconds(2f);
                        gameManager.buyPropertyDecisionMade = true;
                        yield break;                    
                    }
                    
                    yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
                    yield return new WaitForSeconds(1f);

                    if (property.stagePrices[property.nextStageIndex] <= Money)
                    {
                        InstantiateBuyPropertyPopup012(property);
                        
                    }
                    else
                    {
                        ShowMessage("Not enough money to acquire this property!");
                        yield return new WaitForSeconds(2f);
                        gameManager.buyPropertyDecisionMade = true;
                        yield break;
                    }
                }
                
                else if (property.currentStageIndex == 4)
                {
                    ShowMessage("You can't buy out the hotel");
                    yield return new WaitForSeconds(2f);
                    gameManager.buyPropertyDecisionMade = true;
                }
            }
        }
        // else
        // {
        //     // Player doesn't have enough money to buy the property
        //     if (property.nextStageIndex >= 0 && property.nextStageIndex <= property.rentPrices.Count && Money < property.rentPrices[property.nextStageIndex])
        //     {
        //         ShowMessage("Not enough money to acquire this property!");
        //         yield return new WaitForSeconds(2f);
        //         buyPropertyPopup.buyPropertyDecisionMade = true;
        //         yield break;
        //     }
        //     else
        //     {
        //         Debug.LogError("Rent price data is missing for property.");
        //     }
        // }
    }

    // private void ShowMessage(string message)
    // {
    //     GameObject messageObject = Instantiate(MessagePrefab, canvasTransform);
    //     TextMeshProUGUI messageText = messageObject.GetComponent<TextMeshProUGUI>();
    //     messageText.text = message;
    //     Destroy(messageObject, 2f);
    // }
    private void ShowMessage(string message)
    {
        GameObject messageObject = Instantiate(MessagePrefab, canvasTransform);
        TextMeshProUGUI messageText = messageObject.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;
        Destroy(messageObject, 2f);

    }


    // Method to find player object by ID
    public PlayerController FindPlayerByID(int ID)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player.playerID == ID)
            {
                return player;
            }
        }
        return null;
    }
    
    public PlayerController FindTeamByID(int ID)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player.teamID == ID)
            {
                return player;
            }
        }
        return null; // Return -1 if no player with the given ID is found
    }

    // You may want to check if the player has landed on a property here
    // and display the buy property popup if necessary
    
    private void DisplayGoToJailText()
    {
        goToJailText.gameObject.SetActive(true);
        StartCoroutine(HideGoToJailText());
    }
    private IEnumerator HideGoToJailText()
    {
        yield return new WaitForSeconds(2f);
        goToJailText.gameObject.SetActive(false);
    }


    public void UpdateMoneyText()
    {
        // Update the text displayed on the moneyText object
        moneyText.text = Money.ToString();
        Debug.Log("Money updated. Current money: " + Money);
    }




    public void EndTurn()
    {
        isTurn = false;
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);
        spriteRenderer.sortingOrder = originalSortingOrder;
        StartCoroutine(DelayedNextTurn());
        

    }
    private IEnumerator DelayedNextTurn()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        gameManager.NextTurn(); // Call NextTurn after waiting
    }
    public void StartTurn()
    {
        isTurn = true;
        coroutineAllowed = true;
        if (!isBuyPopUpActive)
        {   
            spriteRenderer.sortingOrder = originalSortingOrder + 1;
            rollButton.gameObject.SetActive(true);
            playerMoveText.gameObject.SetActive(true);
            gameManager.buyPropertyDecisionMade = false;
            gameManager.buyOutDecisionMade = false;    
        }
    }


    private void DisplayPlus300()
    {
        // Instantiate the Plus300Text prefab and set its position
        TextMeshProUGUI plus300Text = Instantiate(plus300TextPrefab, canvasTransform);
        plus300Text.gameObject.SetActive(true);

        // Set the position of the Plus300Text
        // plus300Text.rectTransform.position = /* Set position here */;

        StartCoroutine(HidePlus300Text(plus300Text));
    }
    private IEnumerator HidePlus300Text(TextMeshProUGUI plus300Text)
    {
        yield return new WaitForSeconds(2f);
        Destroy(plus300Text.gameObject); // Destroy the Plus300Text object after 2 seconds
    }

}
