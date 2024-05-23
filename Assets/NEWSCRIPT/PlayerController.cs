using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer playerSpriteRenderer;
    // public List<StallManager.StallData> properties;
    // public List<StallManager.StallData> ownedStalls = new List<StallManager.StallData>();
    public List<StallManager.StallData> ownedStalls;
    public List<OnsenManager.OnsenData> ownedOnsens;

    public List<SellableItem> ListPropertiesForEffect;
    public SellableItem propertyToBeEffected;
    
    public SellableItem itemToLandOn;

    public List<SellableItem> ListPropertiesForSelling;
    public List<SellableItem> propertiesToSell;

    // public List<StallManager.StallData> propertyToSell;
    // private PlayerController currentPlayerController;
    
    public int dice1Value;
    public int dice2Value;
    public GameObject BankRuptStamp;
    public GameObject MessageObject;
    public GameObject ChancePopUp;

    public BuyPropertyPopup012 buy012PopUp;
    public BuyOutPopUp buyoutPopup;
    public OnsenPopUp onsenpopup;

    public int playerID;
    public int teamID;
    public TextMeshProUGUI teamNumberText;

    public int currentPosition;
    private bool isTurn;

    public Button rollButton;
    public Button sellButton;
    public TextMeshProUGUI selectedmoney;
    public TextMeshProUGUI MoneyNeeded;
    public int moneyneeded;

    public TextMeshProUGUI sumText;
    
    private bool coroutineAllowed = true;

    private GameManager gameManager;

    public Transform[] waypoints;
    [HideInInspector]
    public int waypointIndex = 0;
    public bool moveAllowed = false;
    public int Money = 2000000;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI playerMoveText;
    private int consecutiveDoublesCount = 0;
    public bool isDoubles = false;
    public bool InJail = false;
    private int turnsInJail = 0;
    public TextMeshProUGUI goToJailText;
    public bool isBankRupt = false;

    public Transform canvasTransform;
    
    public int fatigueLevel = 0;
    private const int maxFatigueLevel = 5;

    private StallManager stallManager;
    private OnsenManager onsenManager;

    // private GameObject darkenScreenGameObject;

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder = 0;
    
    public bool isBuyPopUpActive = false;
    public bool isHotSpringActive = false;

    public void AssignPlayerID(int id)
    {
        playerID = id;
    }    
    public void AssignTeamID(int teamid)
    {
        teamID = teamid;
        teamNumberText.text = "Team: " + teamID.ToString();
    }

    public bool hasGetOutOfJailCard = false;
    public bool hasVipExperienceTicket = false;
    
    public TMP_InputField dice1InputField;
    public TMP_InputField dice2InputField;

    void Start()
    {   
        stallManager = StallManager.Instance;
        onsenManager = OnsenManager.Instance;
       
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(() => StartRollDiceCoroutine(dice1Value, dice2Value));
        transform.position = waypoints[waypointIndex].transform.position;
        
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);    
        // properties = stallManager.properties;
  
        if (stallManager == null)
        {
            Debug.LogError("stallManager is not assigned.");
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
        
        SetFontSize(dice1InputField, 50);
        SetFontSize(dice2InputField, 50);
        // StallManager.Instance.OnPropertiesLoaded += OnPropertiesLoaded;
        // currentPlayer = gameManager.players[currentPlayerIndex]
        selectedmoney.gameObject.SetActive(false);
        MoneyNeeded.gameObject.SetActive(false);
        playerSpriteRenderer  = GetComponent<SpriteRenderer>();
    }
    private void ShowHotSpringPopup(OnsenManager.OnsenData onsen, PlayerController player)
    {
        onsenpopup.playerController = player;
        player.onsenpopup.gameObject.SetActive(true);
        player.onsenpopup.DisplayBuyHotSpring(onsen);
    }
    private void ShowBuy012(StallManager.StallData stall, PlayerController player)
    {
        buy012PopUp.playerController = player;
        player.buy012PopUp.gameObject.SetActive(true);
        player.buy012PopUp.Display012(stall);
    }
    private void ShowBuyOutPopUp(StallManager.StallData stall, PlayerController player)
    {
        buyoutPopup.playerController = player;
        player.buyoutPopup.gameObject.SetActive(true);
        player.buyoutPopup.DisplayBuyOut(stall);
    }

    private IEnumerator DisplayChancePopUp()
    {
        ChancePopUp.gameObject.SetActive(true);
        yield return StartCoroutine(gameManager.HideObject(ChancePopUp));
    }
    
    public IEnumerator ShowMessage(string message)
    {
        MessageObject.gameObject.SetActive(true);
        TextMeshProUGUI messageText = MessageObject.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;
        yield return StartCoroutine(gameManager.HideObject(MessageObject));
    }

    private void SetFontSize(TMP_InputField inputField, float fontSize)
    {
        inputField.textComponent.fontSize = fontSize;
        if (inputField.placeholder != null && inputField.placeholder is TMP_Text)
        {
            ((TMP_Text)inputField.placeholder).fontSize = fontSize;
        }
    }
    // private void OnPropertiesLoaded()
    // {
    //     // Now that properties are loaded, access them
    //     properties = stallManager.properties;
    // }   

    public IEnumerator RollDiceOnClick(int dice1Value, int dice2Value)
    {
        if (!GameManager.GameOver && isTurn && coroutineAllowed)
        {
            if (InJail && !hasGetOutOfJailCard)
            {
                // int diceValue1 = int.Parse(dice1InputField.text);
                // int diceValue2 = int.Parse(dice2InputField.text);  
                // StartCoroutine(RollDiceInJail(diceValue1, diceValue2));
                StartCoroutine(RollDiceInJail(dice1Value, dice2Value));
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else if (InJail && hasGetOutOfJailCard)
            {
                InJail = false;
                hasGetOutOfJailCard = false;
                turnsInJail = 0;
                // int diceValue1 = int.Parse(dice1InputField.text);
                // int diceValue2 = int.Parse(dice2InputField.text);           
                // StartCoroutine(RollTheDice(diceValue1 , diceValue2));
                StartCoroutine(RollTheDice(dice1Value, dice2Value));
                yield return StartCoroutine(ShowMessage("Your Get out of Jail Ticket has been redeemed! You are now released from jail without penalty."));
                // yield return new WaitForSecondsRealtime(2f);
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else
            {
                int diceValue1 = int.Parse(dice1InputField.text); 
                int diceValue2 = int.Parse(dice2InputField.text);
                StartCoroutine(RollTheDice(diceValue1 , diceValue2));
                // StartCoroutine(RollTheDice(dice1Value, dice2Value));
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }

        }
    }
    private void StartRollDiceCoroutine(int dice1Value, int dice2Value)
    {
        StartCoroutine(RollDiceOnClick(dice1Value, dice2Value));
    }

    private IEnumerator RollDiceInJail(int dice1Value, int dice2Value)
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

        //     yield return new WaitForSecondsRealtime(0.05f);
        // } 
                // int sum = diceValues[0] + diceValues[1];    
        // //---------------------

        //------------------------------------------
        foreach (var dice1Image in gameManager.dice1Sides.Values)
        {
            dice1Image.SetActive(false);
        }
        foreach (var dice2Image in gameManager.dice2Sides.Values)
        {
            dice2Image.SetActive(false);
        }   
        //-----------------------
        for (int i = 0; i <= 20; i++)
        {
            int randomDice1Side = Random.Range(1, 7); 
            int randomDice2Side = Random.Range(1, 7); 

            gameManager.dice1Sides[randomDice1Side].SetActive(true);
            gameManager.dice2Sides[randomDice2Side].SetActive(true);

            yield return new WaitForSecondsRealtime(0.05f);

            gameManager.dice1Sides[randomDice1Side].SetActive(false);
            gameManager.dice2Sides[randomDice2Side].SetActive(false);

            // Store the random values
            diceValues[0] = randomDice1Side;
            diceValues[1] = randomDice2Side;
        }
        // Set the final values
        gameManager.dice1Sides[diceValues[0]].SetActive(true);
        gameManager.dice2Sides[diceValues[1]].SetActive(true);
        int sum = diceValues[0] + diceValues[1];

        //-------------------------------------

        sumText.text = "" + sum; 
        isDoubles = (diceValues[0] == diceValues[1]); 

        if (isDoubles)
        {
            InJail = false;
            turnsInJail = 0;
            Debug.Log("current position before moving in jail" + currentPosition); 
            Debug.Log("sum= " + sum);
            MovePlayer(sum);
            yield return StartCoroutine(WaitForPropertyDecision());
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
                // yield return StartCoroutine(WaitForPropertyDecision());
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

    private IEnumerator RollTheDice(int dice1Value, int dice2Value)
    {
        Debug.Log("inside roll dice normal");
        coroutineAllowed = false;
        int[] diceValues = new int[2];
        foreach (var dice1Image in gameManager.dice1Sides.Values)
        {
            dice1Image.SetActive(false);
        }
        foreach (var dice2Image in gameManager.dice2Sides.Values)
        {
            dice2Image.SetActive(false);
        }   
        //-----------------------
        // for (int i = 0; i <= 20; i++)
        // {
        //     int randomDice1Side = Random.Range(1, 7); 
        //     int randomDice2Side = Random.Range(1, 7); 

        //     gameManager.dice1Sides[randomDice1Side].SetActive(true);
        //     gameManager.dice2Sides[randomDice2Side].SetActive(true);

        //     yield return new WaitForSecondsRealtime(0.05f);

        //     gameManager.dice1Sides[randomDice1Side].SetActive(false);
        //     gameManager.dice2Sides[randomDice2Side].SetActive(false);

        //     // Store the random values
        //     diceValues[0] = randomDice1Side;
        //     diceValues[1] = randomDice2Side;
        // }
        // // Set the final values
        // gameManager.dice1Sides[diceValues[0]].SetActive(true);
        // gameManager.dice2Sides[diceValues[1]].SetActive(true);
        // int sum = diceValues[0] + diceValues[1];
        //---------------------
        // For testing purposes
        diceValues[0] = dice1Value;
        diceValues[1] = dice2Value;
        int sum = diceValues[0] + diceValues[1];      
        //---------------------

        sumText.text = "" + sum; 
        yield return new WaitForSecondsRealtime(0.1f);

        yield return MovePlayerCoroutine(sum);
        // MovePlayer(diceValues[0] + diceValues[1]);

        if (currentPosition == 8)
        {
            if (hasGetOutOfJailCard)
            {
                DisplayGoToJailText();
                InJail = false;
                hasGetOutOfJailCard = false; 
                yield return StartCoroutine(ShowMessage("You have used your Get out of jail Ticket"));
                // yield return new WaitForSecondsRealtime(2f);

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
        yield return StartCoroutine(CheckPosition());
        yield return StartCoroutine(CheckForDoubles(diceValues));
        coroutineAllowed = true; 
    }

    private IEnumerator CheckPosition()
    {
        // if (currentPosition == 4 || currentPosition == 13 || currentPosition == 18 || currentPosition == 25)
        // {
        //     yield return StartCoroutine(ShowMessage($"You've discovered a hot spring!"));
        //     ShowHotSpringPopup(onsen, this);
        // }
        if (currentPosition == 12 || currentPosition == 20 || currentPosition == 23 || currentPosition == 28)
        {
            yield return StartCoroutine(DisplayChancePopUp());
            yield return new WaitForSecondsRealtime(1f);
            yield return StartCoroutine(CardManager.Instance.DrawAndDisplayCard(this));
        }     

        if (currentPosition == 31)
        {
            int totalPropertyValueforTax = 0;
            foreach (StallManager.StallData stall in ownedStalls)
            {
                totalPropertyValueforTax += stall.stagePrices[stall.currentStageIndex];
            }
            int taxAmount = (int)(totalPropertyValueforTax * 0.1f);
            Money -= taxAmount;
            UpdateMoneyText();
            yield return StartCoroutine(ShowMessage($"You paid income tax of ${taxAmount}"));
            // yield return new WaitForSecondsRealtime(2f);
        }   
    }

    private IEnumerator CheckForDoubles(int[] diceValues)
    {   
        isDoubles = (diceValues[0] == diceValues[1]);

        if (isDoubles)
        {   
            Debug.Log ("inside isDouble");
            consecutiveDoublesCount++;
            fatigueLevel = Mathf.Min(fatigueLevel + 1, maxFatigueLevel);
            
            if (consecutiveDoublesCount >= 3)
            {   
                // yield return StartCoroutine(WaitForPropertyDecision());
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
                yield return StartCoroutine(WaitForPropertyDecision());
                StartTurn();  
            }
        }
        else
        {   
            consecutiveDoublesCount = 0;
            // MovePlayer(diceValues[0] + diceValues[1]);
            // yield return StartCoroutine(WaitForPropertyDecision());
            EndTurn();
            yield break;
        }
    }

    void MovePlayer(int steps)
    {
        StartCoroutine(MovePlayerCoroutine(steps));
    }

    public IEnumerator MovePlayerCoroutine(int steps)
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
            transform.position = waypoints[currentPosition].position;
            stepsRemaining--;
            if (currentPosition == 0 && direction == 1)
            {
                // Add $300,000 to the player's money
                Money += 300000;
                UpdateMoneyText(); // Update UI to reflect the new money amount
                gameManager.DisplayPlus300();
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }
        yield return StartCoroutine(LandOnProperty());
    }

    public void MoveForward()
    {
        currentPosition = (currentPosition + 1) % waypoints.Length;
        transform.position = waypoints[currentPosition].position;
    }

    public void MoveBackward()
    {
        currentPosition = (currentPosition - 1 + waypoints.Length) % waypoints.Length;
        transform.position = waypoints[currentPosition].position;
    }

    private IEnumerator LandOnProperty()
    {
        if (stallManager == null)
        {
            Debug.LogError("stallManager is null.");
            yield break;
        }
        if (waypoints == null)
        {
            Debug.LogError("waypoints array is null.");
            yield break;
        }
        itemToLandOn = null;
        var stall = stallManager.GetPropertyByWaypointIndex(currentPosition);
        var hotSpring = onsenManager.GetOnsenByWaypointIndex(currentPosition);

        if (stall != null)
        {
            itemToLandOn = new SellableItem { stallData = stall };
        }
        else if (hotSpring != null)
        {
            itemToLandOn = new SellableItem { onsenData = hotSpring };
        }

        if (itemToLandOn == null)
        {
            Debug.Log("No stall or hot spring found at the current position.");
            yield break;
        }
        if (!itemToLandOn.owned)
        {
            if (itemToLandOn.onsenData != null)
            {
                // Handle hot spring
                if (itemToLandOn.Price <= Money)
                {
                    yield return StartCoroutine(ShowMessage("You've discovered an onsen!"));
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying the natural onsen."));
                    ShowHotSpringPopup(itemToLandOn.onsenData, this);
                }
                else
                {
                    yield return StartCoroutine(ShowMessage("You do not have enough money to build an onsen ryokan at this onsen."));
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying the natural onsen."));
                    gameManager.EndedAllInteraction = true;
                }
            }
            else if (itemToLandOn.stallData != null)
            {
                if (itemToLandOn.stallData.nextStageIndex <= 5 && itemToLandOn.Price <= Money)
                {
                    ShowBuy012(itemToLandOn.stallData, this);
                }
                else
                {
                    yield return StartCoroutine(ShowMessage("You do not have enough money to buy this stall."));
                    gameManager.EndedAllInteraction = true;
                }
            }
        }
        else //if (itemToLandOn.owned)
        {
            PlayerController ownerPlayer = FindPlayerByID(itemToLandOn.ownerID);
            if (itemToLandOn.onsenData != null)
            {
                int rentPriceToDeduct = 3000000;
                // int rentPriceToDeduct = itemToLandOn.onsenData.rentPriceOnsen + (20000 * fatigueLevel);
                string formattedRent = FormatMoney(rentPriceToDeduct); 
                yield return StartCoroutine(ShowMessage($"You pay the hot spring entry fee of ${formattedRent}, plus a surcharge for deluxe relaxation amenities tailored to your fatigue level of {fatigueLevel}.")); 

                if (ownerPlayer.teamID != this.teamID)
                {
                    yield return StartCoroutine(HandleRent(rentPriceToDeduct,itemToLandOn));
                    // Money -= rentPriceToDeduct;
                    // UpdateMoneyText();
                    // ownerPlayer.Money += rentPriceToDeduct;
                    // ownerPlayer.UpdateMoneyText();
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has decreased to 0."));
                    gameManager.OnsenDecisionMade = true;
                }
                else
                {
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying your onsen."));
                }
            }
            else if (itemToLandOn.stallData != null)
            {
                if (itemToLandOn.currentStageIndex < 5)
                {
                    if (ownerPlayer.teamID == this.teamID)
                    {
                        if (stall.nextStageIndex <= stall.stagePrices.Count && stall.stagePrices[stall.nextStageIndex] <= Money)
                        {
                            ShowBuy012(stall, this);
                        }
                        else if (Money < stall.stagePrices[stall.nextStageIndex])
                        {
                            yield return StartCoroutine(ShowMessage("Not enough money to acquire this stall!"));
                            // yield return new WaitForSecondsRealtime(2f);
                            gameManager.EndedAllInteraction = true;
                            yield break;
                        }
                    }
                    else if (ownerPlayer.teamID != this.teamID)
                    {
                        // int rentPriceToDeduct = stall.rentPrices[stall.currentStageIndex];
                        int rentPriceToDeduct = 3000000;
                        string formattedRent = FormatMoney(rentPriceToDeduct);  
                        yield return StartCoroutine(ShowMessage("You have to pay the meal expense of $" + formattedRent));
                        yield return StartCoroutine(HandleRent(rentPriceToDeduct,itemToLandOn));

                        if (stall.currentStageIndex < 4)
                        {
                            if (stall.buyoutPrices[stall.currentStageIndex] <= Money )
                            {
                                ShowBuyOutPopUp(stall, this);
                            }
                            else if (stall.buyoutPrices[stall.currentStageIndex] > Money)
                            {
                                yield return StartCoroutine(ShowMessage("Not enough money to acquire this stall!"));
                                // yield return new WaitForSecondsRealtime(2f);
                                gameManager.EndedAllInteraction = true;
                                yield break;                    
                            }
                            
                            // yield return StartCoroutine(WaitForPropertyDecision());
                            yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
                            yield return new WaitForSecondsRealtime(1f);
                            PlayerController ownerPlayeragain = FindPlayerByID(stall.ownerID);
                            if (stall.stagePrices[stall.nextStageIndex] <= Money && ownerPlayeragain.teamID == this.teamID)
                            {
                                ShowBuy012(stall, this);
                                
                            }
                            else if (stall.stagePrices[stall.nextStageIndex] > Money && ownerPlayeragain.teamID == this.teamID)
                            {
                                yield return StartCoroutine(ShowMessage("Not enough money to acquire this stall!"));
                                // yield return new WaitForSecondsRealtime(2f);
                                gameManager.EndedAllInteraction = true;
                                yield break;
                            }
                        }
                        else if (stall.currentStageIndex == 4)
                        {
                            yield return StartCoroutine(ShowMessage("You can't buy out the hotel"));
                            // yield return new WaitForSecondsRealtime(2f);
                            // yield return StartCoroutine(WaitForPropertyDecision());
                            gameManager.EndedAllInteraction = true;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator HandleRent( int rentPriceToDeduct, SellableItem itemToLandOn)
    {
        // string formattedRent = FormatMoney(rentPriceToDeduct);
        PlayerController ownerPlayer = FindPlayerByID(itemToLandOn.ownerID);
        if (itemToLandOn.isWelcomeEvent)
        {
            yield return StartCoroutine(ShowMessage("This property offers a complimentary meal. No payment required"));
            yield return new WaitForSecondsRealtime(1f);
        }
        else
        {
            if (hasVipExperienceTicket)
            {
                hasVipExperienceTicket = false;
                yield return StartCoroutine(ShowMessage("Your Free Meal Ticket has been redeemed! Enjoy your complimentary meal."));
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                if (Money >= rentPriceToDeduct) // Check if player has enough money to pay the rent
                {
                    Money -= rentPriceToDeduct;
                    UpdateMoneyText();
                    yield return new WaitForSecondsRealtime(1f);
                    ownerPlayer.Money += rentPriceToDeduct;
                    ownerPlayer.UpdateMoneyText();
                }
                else
                {
                    yield return StartCoroutine(ShowMessage("You don't have enough money to pay the rent! You need to sell one of your properties."));
                    GameManager.Instance.rentToPay = rentPriceToDeduct;
                    gameManager.isSelling = true;
                    foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                    {
                        var tileScript = tile.GetComponent<TileScript>();
                        if (tileScript != null)
                        {
                            tileScript.enabled = false;
                        }
                    }
                    ListPropertiesForSelling.Clear();
                    propertiesToSell.Clear();
                    gameManager.selectedPropertiestoSell.Clear();
                    // ListPropertiesForSelling.AddRange(ownedStalls);
                    foreach (var stalltosell in ownedStalls)
                    {
                        ListPropertiesForSelling.Add(new SellableItem { stallData = stalltosell });
                    }
                    // Add hot springs
                    foreach (var onsentosell in ownedOnsens)
                    {
                        ListPropertiesForSelling.Add(new SellableItem { onsenData = onsentosell });
                    }

                    if (ListPropertiesForSelling.Count > 0)
                    {
                        int totalownedStallValue = 0;
                        int totalownedOnsenValue = 0;
                        foreach (var stallcalculated in ownedStalls)
                        {
                            totalownedStallValue += stallcalculated.stagePrices[stallcalculated.currentStageIndex];                        
                        }
                        foreach (var onsencalculated in ownedOnsens)
                        {
                            totalownedOnsenValue += onsencalculated.priceOnsen;
                        }
                        int moneyall = Money + totalownedStallValue + totalownedOnsenValue;
                        if (moneyall >= GameManager.Instance.rentToPay)
                        {
                            if (ListPropertiesForSelling.Count == 1)
                            {
                                yield return StartCoroutine(ApplySell(rentPriceToDeduct));
 
                                // foreach (var stallToSell in propertiesToSell)
                                // {
                                    yield return StartCoroutine(ShowMessage("You have sold all your properties!"));
                                // }
                                GameManager.Instance.SellSelectionMade = true;                                               
                            }
                            else if (ListPropertiesForSelling.Count > 1)
                            {
                                yield return StartCoroutine(ShowMessage("Select a stall to sell:"));
                                foreach (var playerProperty in ListPropertiesForSelling)
                                {
                                    GameObject tileImage = gameManager.waypointIndexToTileMap[playerProperty.JSONwaypointIndex];
                                    tileImage.transform.position += new Vector3(0, 1, 0);
                                    SellingHandler clickHandler = tileImage.GetComponent<SellingHandler>();
                                    if (clickHandler == null)
                                    {
                                        clickHandler = tileImage.AddComponent<SellingHandler>();
                                    }
                                    clickHandler.SetAssociatedProperty(playerProperty);
                                    clickHandler.playerController = this;
                                    selectedmoney.gameObject.SetActive(true);
                                    MoneyNeeded.gameObject.SetActive(true);
                                    moneyneeded =0;
                                    moneyneeded = GameManager.Instance.rentToPay - Money;
                                    MoneyNeeded.text = FormatMoney(moneyneeded);
                                }

                                yield return new WaitUntil(() => gameManager.SellSelectionMade);                                   
                                propertiesToSell = gameManager.selectedPropertiestoSell;
                                if (propertiesToSell != null)
                                {
                                    yield return StartCoroutine(ApplySell(rentPriceToDeduct));                                            
                                }
                                foreach (var foodstall in ListPropertiesForSelling)
                                {
                                    GameObject tileImage = gameManager.waypointIndexToTileMap[foodstall.JSONwaypointIndex];
                                    tileImage.transform.position += new Vector3(0, -1, 0);
                                }  
                            }                                     
                        }
                        else
                        {
                            yield return StartCoroutine(ApplySell(rentPriceToDeduct));  
                            // Money = 0;
                            // UpdateMoneyText();
                            // ownerPlayer.Money += moneyall;
                            // ownerPlayer.UpdateMoneyText();                                            
                            isBankRupt = true;                                          
                            yield return StartCoroutine(ShowMessage("You have sold all of your properties"));
                            yield return StartCoroutine(ShowMessage("BANKRUPT!!!"));
                            BankRuptStamp.gameObject.SetActive(true);
                            HidePlayerImage();
                            GameManager.Instance.SellSelectionMade = true;
                            yield break;
                        }            
                    }
                    else
                    {
                        // Inform the player if they don't own any properties to sell
                        yield return StartCoroutine(ShowMessage("You don't own any properties to sell."));
                    }
                    yield return new WaitUntil(() => gameManager.SellSelectionMade);
                    
                    gameManager.SellSelectionMade = false;
                    ListPropertiesForSelling.Clear();
                    gameManager.selectedPropertiestoSell.Clear();
                    propertiesToSell.Clear();
                    moneyneeded =0;
                    // totalPropertyValue =0;
                    GameManager.Instance.rentToPay = 0;
                    foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                    {
                        var tileScript = tile.GetComponent<TileScript>();
                        if (tileScript != null)
                        {
                            tileScript.enabled = true;
                        }
                    }
                    gameManager.isSelling = false;
                    yield return new WaitForSecondsRealtime(2f); 
                    yield return null;
                }
            }
        }
    }

    private IEnumerator ApplySell(int rentPriceToDeduct)
    {
        PlayerController ownerPlayer = FindPlayerByID(itemToLandOn.ownerID);
        foreach (var propertyToSell in ListPropertiesForSelling)
        {
            int compensationAmount = propertyToSell.Price;
            Money += compensationAmount;
            UpdateMoneyText();
            if (propertyToSell.stallData != null)
            {
                StallManager.StallData stalltosell = propertyToSell.stallData;
                ownedStalls.Remove(stalltosell);
            }
            else if (propertyToSell.onsenData != null)
            {
                OnsenManager.OnsenData onsentosell = propertyToSell.onsenData;
                ownedOnsens.Remove(onsentosell);
            }
            propertyToSell.owned = false;
            propertyToSell.ownerID = 0;
            propertyToSell.teamownerID = 0;
            propertyToSell.currentStageIndex = -1;
            propertyToSell.isFireWork = false;
            propertyToSell.isWelcomeEvent = false;
            if (gameManager.currentHostingFireWork == propertyToSell)
            {
                gameManager.currentHostingFireWork = null;
                gameManager.FireworkPlaceIsSet = false;
            }           
            foreach (GameObject stageImage in propertyToSell.StageImages)
            {
                stageImage.SetActive(false);
            }
            foreach (GameObject rentTagImage in propertyToSell.RentTagImages)
            {
                rentTagImage.SetActive(false);
            }
            propertyToSell.rentText.gameObject.SetActive(false);
            yield return StartCoroutine(ShowMessage("You have sold  your" + propertyToSell.name));
        }
        Money -= rentPriceToDeduct;
        UpdateMoneyText();
        ownerPlayer.Money += rentPriceToDeduct;
        ownerPlayer.UpdateMoneyText();                                                
        selectedmoney.gameObject.SetActive(false);
        MoneyNeeded.gameObject.SetActive(false);
        yield return null;
    }

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

    public void DisplayGoToJailText()
    {
        goToJailText.gameObject.SetActive(true);
        StartCoroutine(HideGoToJailText());
    }
    private IEnumerator HideGoToJailText()
    {
        yield return new WaitForSecondsRealtime(2f);
        goToJailText.gameObject.SetActive(false);
    }

    public void UpdateMoneyText()
    {
        // Update the text displayed on the moneyText object
        moneyText.text = FormatMoney(Money);
        // moneyText.text = Money.ToString();
        Debug.Log("Money updated. Current money: " + Money);
    }
    
    public string FormatMoney(long amount)
    {
        if (amount >= 2000000)
        {
            float millionValue = amount / 1000000f;
            return millionValue.ToString("0.###") + "M";
        }
        else if (amount >= 1000)
        {
            return (amount / 1000f).ToString("0.#") + "K";
        }
        else
        {
            return amount.ToString();
        }
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
        yield return StartCoroutine(WaitForPropertyDecision());
        yield return new WaitForSecondsRealtime(0.5f); // Wait for 1 second
        gameManager.buyPropertyDecisionMade = false;
        gameManager.buyOutDecisionMade = false;
        gameManager.EndedAllInteraction = false;    
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
        }
    }

    public IEnumerator WaitForPropertyDecision()
    {
        while (buy012PopUp != null && buy012PopUp.gameObject.activeSelf|| buyoutPopup != null&& buyoutPopup.gameObject.activeSelf || onsenpopup!= null&& onsenpopup.gameObject.activeSelf )
        {
            while (buy012PopUp.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            }
            while (buyoutPopup.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
            }
            while (onsenpopup.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.OnsenDecisionMade);
            }
        }
        gameManager.EndedAllInteraction = true;
    }
    public void ShowPlayerImage()
    {
        playerSpriteRenderer.enabled = true; // Show the player image
    }

    // Method to hide the player's image
    public void HidePlayerImage()
    {
        playerSpriteRenderer.enabled = false; // Hide the player image
    }
}
