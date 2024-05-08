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

    // private bool loopCompleted = false;
    
    public Transform[] waypoints;
    [SerializeField]
    // private float moveSpeed = 1f;
    [HideInInspector]
    public int waypointIndex = 0;
    public bool moveAllowed = false;
    public int Money = 2000000;
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
    private BuyPropertyPopup012 BuypopupInstance;

    public string buyoutPopupPrefabPath = "buyoutPopupPrefab"; // The path to the MessagePrefab relative to the Resources folder
    private BuyOutPopUp buyoutPopupPrefab;
    private BuyOutPopUp buyoutPopupInstance;
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

    [SerializeField]
    private string ChancePrefabPath = "ChancePrefab";
    private GameObject ChancePrefab; 

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

    public List<Card> cardDeck = new List<Card>();
    [System.Serializable]
    public class Card
    {
        public string name;
        public string description;

        public Card(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }

    public bool hasGetOutOfJailCard = false;
    public bool hasFreeRentTicket = false;

    void PopulateCardDeck()
    {
        // cardDeck.Add(new Card("Birthday Gift", "Collect a Birthday Gift of $15000 from each player."));
        // cardDeck.Add(new Card("Lottery Win: $200,000", "Congratulations! You have won a lottery prize of $200,000.")); 
        // cardDeck.Add(new Card( "Dog Poop Cleanup Fee", "Oops! You have to pay a fee of $50,000 for dog poop cleanup."));

        // cardDeck.Add(new Card("Get out of Jail Ticket", "You can use this card to get out of jail once."));
        // cardDeck.Add(new Card("Go to Jail", "Go directly to Jail. Do not pass 'Go,' do not collect $300,000"));

        // cardDeck.Add(new Card("Advance to Go", "Move your character to the \"Go\" space on the board and collect $300,000 from the bank."));
        // cardDeck.Add(new Card("Go Back to Go", "Go back to \"Go\" without passing 'Go,' without collecting $300,000"));

        // cardDeck.Add(new Card("Advance 1 Space", "Advance 1 space on the board."));
        cardDeck.Add(new Card("Move Backward 1 Space", "Move your character back one space on the board."));

        // cardDeck.Add(new Card("Tax Exemption", "You are exempt from paying any taxes the next time."));
        // cardDeck.Add(new Card("Tax Levy",  "Pay a tax equal to 10% of the total value of your owned properties."));

        // cardDeck.Add(new Card("Avenue Demolition", "Demolish one avenue and leave it ownerless."));

        // cardDeck.Add(new Card("Generous Treat", "Select one food stall of the opponent. Any player landing on this stall is treated to a complimentary meal for one turn, no payment necessary."));
        // cardDeck.Add(new Card("Free Meal Ticket", "Receive a ticket for a complimentary meal at any food stall on your next visit."));

        // cardDeck.Add(new Card("Property Seizure", "Force one opponent to sell one property of your choice from their holdings."));
        // cardDeck.Add(new Card("Natural Disaster: Food Stall Ruined",  "An earthquake has destroyed 1 of your food stalls at the festival "));
        // cardDeck.Add(new Card("Forced Property Sale", "You must sell one property of your choice from your holdings."));

        // cardDeck.Add(new Card("Firework Spectacle", "Select one of your stalls to host a firework display, turning it into a hot spot and increasing its value."));     
    }
    // // private List<string> cards = new List<string> 
    // private List<string> cardDescriptions = new List<string>
    // {
    //     // "Firework",
    // };



    void Start()
    {   
        // currentPlayerController = FindObjectOfType<PlayerController>();
        propertyManager = PropertyManager.Instance;
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(StartRollDiceCoroutine);
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
        Money = 2000000;
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
            Debug.LogError("Failed to load CardPrefabPath from Resources folder at path: " + MessagePrefabPath);
        }
        ChancePrefab = Resources.Load<GameObject>(ChancePrefabPath);
        if (ChancePrefab == null)
        {
            Debug.LogError("Failed to load ChancePrefabPath from Resources folder at path: " + MessagePrefabPath);
        }  
        PopulateCardDeck();
        
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
        BuypopupInstance  = Instantiate(buyPropertyPopup012Prefab, canvas.transform);

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
        buyoutPopupInstance = Instantiate(buyoutPopupPrefab, canvas.transform);
 
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

    public IEnumerator RollDiceOnClick()
    {
        if (!GameManager.GameOver && isTurn && coroutineAllowed)
        {
            if (InJail && !hasGetOutOfJailCard)
            {

                StartCoroutine(RollDiceInJail());
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else if (InJail && hasGetOutOfJailCard)
            {
                InJail = false;
                hasGetOutOfJailCard = false;
                turnsInJail = 0;
                StartCoroutine(RollTheDice());
                ShowMessage("Your Get out of Jail Ticket has been redeemed! You are now released from jail without penalty.");
                yield return new WaitForSeconds(2f);
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(RollTheDice());
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
                            if (hasGetOutOfJailCard)
                {
                    

                }
        }
    }
    private void StartRollDiceCoroutine()
    {
        StartCoroutine(RollDiceOnClick());
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
            yield return new WaitUntil(() => gameManager.EndedAllInteraction);
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
                WaitForPropertyDecision();
                yield return new WaitUntil(() => gameManager.EndedAllInteraction);
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
            if (hasGetOutOfJailCard)
            {
                DisplayGoToJailText();
                InJail = false;
                hasGetOutOfJailCard = false; 
                ShowMessage("You have used your Get out of jail Ticket");
                yield return new WaitForSeconds(2f);

            }
            else
            {
                DisplayGoToJailText();
                InJail = true;
                consecutiveDoublesCount = 0;
                EndTurn();
                coroutineAllowed = false;
                yield break;
                // Exit the coroutine early if the player is in jail
            }
        }
        
        if (currentPosition == 12 || currentPosition == 20 || currentPosition == 23 || currentPosition == 28)
        {
            GameObject ChancePrefabInstance = Instantiate(ChancePrefab, canvasTransform);
            yield return new WaitForSeconds(2f);
            Destroy(ChancePrefabInstance);
            yield return new WaitForSeconds(0.5f);
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
                WaitForPropertyDecision();
                yield return new WaitUntil(() => gameManager.EndedAllInteraction);
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
                
                yield return new WaitUntil(() => gameManager.EndedAllInteraction);
                StartTurn();
                
            }
        }

        else
        {   
            consecutiveDoublesCount = 0;
            // MovePlayer(diceValues[0] + diceValues[1]);
            yield return new WaitUntil(() => gameManager.EndedAllInteraction);
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
        int stepsRemaining = Mathf.Abs(steps); // Ensure steps is positive
        int direction = 1; // Default direction is forward

        if (steps < 0)
        {
            direction = -1; // Change direction to backward if steps are negative
        }

        while (stepsRemaining > 0)
        {
            currentPosition = (currentPosition + direction) % waypoints.Length; // Calculate the new position

            // Move the player's game object to the new waypoint position
            transform.position = waypoints[currentPosition].position;

            stepsRemaining--;

            if (currentPosition == 0 && direction == 1)
            {
                // Add $300,000 to the player's money
                Money += 300000;
                UpdateMoneyText(); // Update UI to reflect the new money amount
                DisplayPlus300();
            }
            yield return new WaitForSeconds(0.3f);
        }

        // Update the current position
        yield return StartCoroutine(LandOnProperty());
    }



    public IEnumerator DrawCard()
    {
        Card drawnCard = cardDeck[Random.Range(0, cardDeck.Count)];
        GameObject cardObject = Instantiate(CardPrefab, canvasTransform);

        // Set the card description text
        // TextMeshProUGUI descriptionText = cardObject.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI nameText = cardObject.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = cardObject.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        if (nameText != null && descriptionText != null)
        {
            nameText.text = drawnCard.name;
            descriptionText.text = drawnCard.description;
        }
        else
        {
            Debug.LogError("Description text component not found on card prefab.");
        }
        yield return new WaitForSeconds(3f);
        Destroy(cardObject);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ApplyCardEffects(drawnCard.name));    
    }

    public IEnumerator ApplyCardEffects(string cardName)
    {
        PlayerController currentPlayer = gameManager.GetCurrentPlayerController();

        switch (cardName)
        {
            case "Birthday Gift":
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                currentPlayer.Money += 15000 * (players.Length - 1);
                currentPlayer.UpdateMoneyText(); // Update UI to reflect the new money amount
                currentPlayer.ShowMessage("Collect a Birthday Gift of $15,000 from each player");

                
                foreach (PlayerController player in players)
                {
                    if (player != currentPlayer) // Exclude the current player
                    {
                        player.Money -= 15000; // Deduct $15 from the player
                        player.UpdateMoneyText(); // Update UI to reflect the new money amount for the player
                        player.ShowMessage("You gave $15,000 as a birthday gift");
                    }
                }
                yield return new WaitForSeconds(2f);
                break;

            case "Lottery Win":
                currentPlayer.Money += 200000;
                currentPlayer.UpdateMoneyText();
                currentPlayer.ShowMessage("You have collected $200,000 from the bank.");
                yield return new WaitForSeconds(2f);
                break;

            case "Dog Poop Cleanup Fee":
                currentPlayer.Money -= 50000;
                currentPlayer.UpdateMoneyText();
                currentPlayer.ShowMessage("You paid a fee of $50,000 for dog poop cleanup.");
                yield return new WaitForSeconds(2f);
                break;

            case "Get out of jail Free Ticket":
                currentPlayer.hasGetOutOfJailCard = true;
                currentPlayer.ShowMessage("You got a Get out of jail Free Ticket to leave jail.");
                yield return new WaitForSeconds(2f);
                break;

            case "Go to Jail":
                currentPlayer.waypointIndex = 8;
                transform.position = waypoints[waypointIndex].position; 
                DisplayGoToJailText();
                InJail = true;
                currentPlayer.ShowMessage("You have been sent to jail.");
                yield return new WaitForSeconds(2f);
                break;

            case "Advance to Go":
                // Determine the number of steps needed to reach the "Go" space
                int stepsToGo = (40 - currentPlayer.currentPosition) % 40;

                // Calculate the target waypoint index
                int targetWaypointIndex = (currentPlayer.currentPosition + stepsToGo) % 40;

                // Slowly move the player to the "Go" space
                while (currentPlayer.currentPosition != targetWaypointIndex)
                {
                    // Move the player to the next waypoint
                    currentPlayer.MoveForward();
                    yield return new WaitForSeconds(0.3f); // Adjust the delay as needed
                }

                // Add $300 from the bank
                currentPlayer.Money += 300000;
                currentPlayer.UpdateMoneyText(); // Update UI to reflect the new money amount
                currentPlayer.ShowMessage("Your character has moved forward to 'Go' and collected $300,000 from the bank.");
                yield return new WaitForSeconds(2f);
                break;

            case "Go Back to Go":
                // Calculate the number of steps needed to move backward to the "Go" space
                int stepsToGoBackward = currentPosition;
                
                // Move the player backward one tile at a time until reaching the "Go" space
                for (int i = 0; i < stepsToGoBackward; i++)
                {
                    // Move the player backward
                    currentPlayer.MoveBackward();
                    
                    // Delay between tile movements (adjust as needed)
                    yield return new WaitForSeconds(0.3f);
                }
                currentPlayer.ShowMessage("Your character has moved back to 'Go'");
                yield return new WaitForSeconds(2f);
                break;

            case "Advance 1 Space":
                StartCoroutine(MovePlayerCoroutine(1));
                break;

            case "Move Backward 1 Space":
                yield return StartCoroutine(MovePlayerCoroutine(-1));
                yield return new WaitUntil(() => gameManager.EndedAllInteraction);
                break;




            case "Free Meal Ticket":
                currentPlayer.hasFreeRentTicket = true;
                currentPlayer.ShowMessage("You've received a Free Dinner Ticket. Your meal will be complimentary next time.");
                yield return new WaitForSeconds(2f);
                break;

            // Add similar cases for other card names...

            default:
                Debug.LogWarning("Unrecognized card name: " + cardName);
                break;
        }
    }

    public void MoveForward()
    {
        // Increment the current waypoint index to move the player forward
        currentPosition = (currentPosition + 1) % waypoints.Length;

        // Move the player's game object to the new waypoint position
        transform.position = waypoints[currentPosition].position;
    }

    public void MoveBackward()
    {
        currentPosition = (currentPosition - 1 + waypoints.Length) % waypoints.Length;
        transform.position = waypoints[currentPosition].position;
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
            yield return new WaitUntil(() => gameManager.EndedAllInteraction);
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
                    gameManager.EndedAllInteraction = true;
                    yield break;
                }
            }
            else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
            {
                int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];
                GameObject rentMessageObject = Instantiate(MessagePrefab, canvasTransform);
                TextMeshProUGUI RentMessageText = rentMessageObject.GetComponentInChildren<TextMeshProUGUI>();
                RentMessageText.text = "You pay a rent of $" + rentPriceToDeduct;
                yield return new WaitForSeconds(1f);
                Destroy(rentMessageObject);

                if (hasFreeRentTicket)
                {
                    hasFreeRentTicket = false;
                    ShowMessage("Your Free Meal Ticket has been redeemed! Enjoy your complimentary meal.");
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    Money -= rentPriceToDeduct;
                    UpdateMoneyText();
                }

                yield return new WaitForSeconds(1f);

                ownerPlayer.Money += rentPriceToDeduct;
                ownerPlayer.UpdateMoneyText();

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
                        gameManager.EndedAllInteraction = true;
                        yield break;                    
                    }
                    
                    yield return new WaitUntil(() => gameManager.EndedAllInteraction);
                    yield return new WaitForSeconds(1f);

                    if (property.stagePrices[property.nextStageIndex] <= Money)
                    {
                        InstantiateBuyPropertyPopup012(property);
                        
                    }
                    else
                    {
                        ShowMessage("Not enough money to acquire this property!");
                        yield return new WaitForSeconds(2f);
                        WaitForPropertyDecision();
                        gameManager.EndedAllInteraction = true;
                        yield break;
                    }
                }
                
                else if (property.currentStageIndex == 4)
                {
                    ShowMessage("You can't buy out the hotel");
                    yield return new WaitForSeconds(2f);
                    WaitForPropertyDecision();
                    gameManager.EndedAllInteraction = true;
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
            gameManager.EndedAllInteraction = false;    
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

    IEnumerator WaitForPropertyDecision()
    {
        while (BuypopupInstance != null || buyoutPopupInstance != null)
        {
            while (BuypopupInstance != null)
            {
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            }
            while (buyoutPopupInstance != null)
            {
                yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
            }

            gameManager.EndedAllInteraction = true;
        }
    }

}
