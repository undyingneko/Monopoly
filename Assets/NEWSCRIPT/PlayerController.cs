using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

public class PlayerController : MonoBehaviour
{
    // public List<PropertyManager.PropertyData> properties;
    // public List<PropertyManager.PropertyData> ownedProperties = new List<PropertyManager.PropertyData>();
    public List<PropertyManager.PropertyData> ownedProperties;
    public List<PropertyManager.PropertyData> ListPropertiesForEffect;
    public PropertyManager.PropertyData propertyToBeEffected;
    public List<HotSpringManager.HotSpringData> ownedHotSprings;

    public List<PropertyManager.PropertyData> ListPropertiesForSelling;
    public List<PropertyManager.PropertyData> propertiesToSell;

    
    
    // public List<PropertyManager.PropertyData> propertyToSell;

    // private PlayerController currentPlayerController;
    
    public int dice1Value;
    public int dice2Value;
    
    public GameObject MessageObject;
    public GameObject ChancePopUp;

    public BuyPropertyPopup012 buy012PopUp;
    public BuyOutPopUp buyoutPopup;
    public HotSpringPopUp hotspringpopup;

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

    public Transform canvasTransform;
    
    public int fatigueLevel = 0;
    private const int maxFatigueLevel = 5;

    private PropertyManager propertyManager;
    private HotSpringManager hotSpringManager;

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
    public bool hasFreeRentTicket = false;
    
    public TMP_InputField dice1InputField;
    public TMP_InputField dice2InputField;

    void Start()
    {   
        propertyManager = PropertyManager.Instance;
        hotSpringManager = HotSpringManager.Instance;
       
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(() => StartRollDiceCoroutine(dice1Value, dice2Value));
        transform.position = waypoints[waypointIndex].transform.position;
        
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);    
        // properties = propertyManager.properties;
  
        if (propertyManager == null)
        {
            Debug.LogError("propertyManager is not assigned.");
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
        // PropertyManager.Instance.OnPropertiesLoaded += OnPropertiesLoaded;
        // currentPlayer = gameManager.players[currentPlayerIndex]
        selectedmoney.gameObject.SetActive(false);
        MoneyNeeded.gameObject.SetActive(false);
    }
    private void ShowHotSpringPopup(HotSpringManager.HotSpringData hotspring, PlayerController player)
    {
        hotspringpopup.playerController = player;
        player.hotspringpopup.gameObject.SetActive(true);
        player.hotspringpopup.DisplayBuyHotSpring(hotspring);
    }
    private void ShowBuy012(PropertyManager.PropertyData property, PlayerController player)
    {
        buy012PopUp.playerController = player;
        player.buy012PopUp.gameObject.SetActive(true);
        player.buy012PopUp.Display012(property);
    }
    private void ShowBuyOutPopUp(PropertyManager.PropertyData property, PlayerController player)
    {
        buyoutPopup.playerController = player;
        player.buyoutPopup.gameObject.SetActive(true);
        player.buyoutPopup.DisplayBuyOut(property);
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
    //     properties = propertyManager.properties;
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
        //     ShowHotSpringPopup(hotspring, this);
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
            foreach (PropertyManager.PropertyData property in ownedProperties)
            {
                totalPropertyValueforTax += property.stagePrices[property.currentStageIndex];
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

            // Move the player's game object to the new waypoint position
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

        // Update the current position
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
        PropertyManager.PropertyData property = propertyManager.GetPropertyByWaypointIndex(currentPosition);
        HotSpringManager.HotSpringData hotspring = hotSpringManager.GetHotSpringByWaypointIndex(currentPosition);
        
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

        // Handle hot spring properties
        if (property == null && currentPosition == 4 || currentPosition == 13 || currentPosition == 18 || currentPosition == 25)
        {
            if (!hotspring.owned)
            {
                if (hotspring.priceHotSpring <= Money)
                {
                    yield return StartCoroutine(ShowMessage("You've discovered an onsen!"));
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying the natural onsen."));
                    ShowHotSpringPopup(hotspring, this);
                    
                }
                else
                {
                    yield return StartCoroutine(ShowMessage("You do not have enough money to build an onsen ryokan  at this onsen."));
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying the natural onsen."));
                    gameManager.EndedAllInteraction = true;
                }
            }
            else
            {
                PlayerController ownerPlayer = FindPlayerByID(hotspring.ownerID);

                if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
                {
                    int rentPriceToDeduct = hotspring.rentPriceSHotSpring + (20000 * fatigueLevel);
                    string formattedRent = FormatMoney(rentPriceToDeduct);
                    Money -= rentPriceToDeduct;
                    UpdateMoneyText();
                    ownerPlayer.Money += rentPriceToDeduct;
                    ownerPlayer.UpdateMoneyText();
                    yield return StartCoroutine(ShowMessage($"You pay the hot spring entry fee of ${formattedRent}, plus a surcharge for deluxe relaxation amenities tailored to your fatigue level of" + fatigueLevel));
                    yield return StartCoroutine(ShowMessage("Your fatigue level has decreased to 0."));
                    fatigueLevel = 0;
                    gameManager.HotSpringDecisionMade = true;
                }
                else 
                {
                    fatigueLevel = 0;
                    yield return StartCoroutine(ShowMessage("Your fatigue level has been reset to 0 after enjoying your onsen."));
                }
            }
            yield return new WaitUntil(() => gameManager.HotSpringDecisionMade);
            // yield return StartCoroutine(WaitForPropertyDecision());
            yield break;
        }

        // Handle standard properties
        if (property != null)
        {
            if (!property.owned && property.nextStageIndex <= 5 && property.stagePrices[property.nextStageIndex] <= Money)
            {
                // Instantiate the buy property popup
                ShowBuy012(property, this);
            }
            else if (property.owned && property.currentStageIndex < 5)
            {
                PlayerController ownerPlayer = FindPlayerByID(property.ownerID);
                if (ownerPlayer != null && ownerPlayer.teamID == this.teamID)
                {
                    if (property.nextStageIndex <= property.stagePrices.Count && property.stagePrices[property.nextStageIndex] <= Money)
                    {
                        ShowBuy012(property, this);
                    }
                    else if (Money < property.stagePrices[property.nextStageIndex])
                    {
                        yield return StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                        // yield return new WaitForSecondsRealtime(2f);
                        gameManager.EndedAllInteraction = true;
                        yield break;
                    }
                }
                else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
                {
                    // int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];
                    int rentPriceToDeduct = 1985000;
                    string formattedRent = FormatMoney(rentPriceToDeduct);
                    
                    if (property.isComplimentaryMeal)
                    {
                        yield return StartCoroutine(ShowMessage("This Food Stall offers a complimentary meal. No payment required"));
                        yield return new WaitForSecondsRealtime(1f);
                    }
                    else
                    {
                        yield return StartCoroutine(ShowMessage("You pay the meal expense of $" + formattedRent));
                        if (hasFreeRentTicket)
                        {
                            hasFreeRentTicket = false;
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
                                // Implement logic for selling property here
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
                                ListPropertiesForSelling.AddRange(ownedProperties); 

                                if (ListPropertiesForSelling.Count > 0)
                                {
                                    gameManager.selectedProperty = null;
                                    
                                    int totalownedPropertyValue = 0;
                                    foreach (var propertycalculated in ownedProperties)
                                    {
                                        totalownedPropertyValue += propertycalculated.stagePrices[propertycalculated.currentStageIndex];
                                        
                                    }
                                    int moneyall = Money + totalownedPropertyValue;

                                    if (moneyall >= GameManager.Instance.rentToPay)
                                    {
                                        Debug.Log("moneyall > GameManager.Instance.rentToPay");
                                        yield return StartCoroutine(ShowMessage("Select a property to sell:"));
                                        foreach (var playerProperty in ListPropertiesForSelling)
                                        {
                                            GameObject tileImage = gameManager.waypointIndexToTileMap[playerProperty.JSONwaypointIndex];

                                            tileImage.transform.position += new Vector3(0, 1, 0);

                                            // Assign click handler for selecting the property
                                            SellingHandler clickHandler = tileImage.GetComponent<SellingHandler>();
                                           

                                            if (clickHandler == null)
                                            {
                                                clickHandler = tileImage.AddComponent<SellingHandler>();
                                            }
                                            clickHandler.SetAssociatedProperty(playerProperty);
                                            clickHandler.playerController = this;
                                            selectedmoney.gameObject.SetActive(true);
                                            MoneyNeeded.gameObject.SetActive(true);
                                            moneyneeded = GameManager.Instance.rentToPay - Money;
                                            MoneyNeeded.text = FormatMoney(moneyneeded);
                                        }

                                        yield return new WaitUntil(() => gameManager.SellSelectionMade);
                                    
                                        // propertyToSell = gameManager.selectedPropertiestoSell;
                                        propertiesToSell = gameManager.selectedPropertiestoSell;
                                        if (propertiesToSell != null)
                                        {
                                            foreach (var propertyToSell in propertiesToSell)
                                            {

                                                int compensationAmount = propertyToSell.stagePrices[propertyToSell.currentStageIndex];
                                                Money += compensationAmount;
                                                ownedProperties.Remove(propertyToSell);
                                                
                                                propertyToSell.owned = false;
                                                propertyToSell.ownerID = 0;
                                                propertyToSell.teamownerID = 0;
                                                propertyToSell.currentStageIndex = -1;

                                                // Deactivate property images and UI elements
                                                propertyManager.DeactivateOldStageImages(propertyToSell);
                                                propertyManager.DeactivateRentTagImage(propertyToSell);
                                                propertyToSell.rentText.gameObject.SetActive(false);
                                                // StartCoroutine(ShowMessage($"You sold {propertyToSell.name} for $ {compensationAmount}."));
                                            }
                                            Money -= rentPriceToDeduct;
                                            UpdateMoneyText();
                                            selectedmoney.gameObject.SetActive(false);
                                            MoneyNeeded.gameObject.SetActive(false);                                            
                                        }
                                        foreach (var foodstall in ListPropertiesForSelling)
                                        {
                                            GameObject tileImage = gameManager.waypointIndexToTileMap[foodstall.JSONwaypointIndex];
                                            tileImage.transform.position += new Vector3(0, -1, 0);
                                        }                                       

                                    }
                                    else
                                    {
                                        yield return StartCoroutine(ShowMessage("You have to sell all of your properties"));
                                        Debug.Log("totalownedPropertyValue <= GameManager.Instance.rentToPay");
                                        GameManager.Instance.selectedPropertiestoSell.Clear();
                                        GameManager.Instance.selectedPropertiestoSell.AddRange(ownedProperties);
                                        
                                        propertiesToSell = gameManager.selectedPropertiestoSell;
                                        if (propertiesToSell != null)
                                        {
                                            foreach (var propertyToSell in propertiesToSell)
                                            {
                                                int compensationAmount = propertyToSell.stagePrices[propertyToSell.currentStageIndex];
                                                Money += compensationAmount;
                                                
                                                ownedProperties.Remove(propertyToSell);
                                                
                                                propertyToSell.owned = false;
                                                propertyToSell.ownerID = 0;
                                                propertyToSell.teamownerID = 0;
                                                propertyToSell.currentStageIndex = -1;

                                                // Deactivate property images and UI elements
                                                propertyManager.DeactivateOldStageImages(propertyToSell);
                                                propertyManager.DeactivateRentTagImage(propertyToSell);
                                                propertyToSell.rentText.gameObject.SetActive(false);
                                            }

                                            Money -= rentPriceToDeduct;
                                            UpdateMoneyText();       
                                        }
                                        GameManager.Instance.SellSelectionMade = true;
                                        yield return StartCoroutine(ShowMessage("You have sold all of your properties"));                                       
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
                                // totalPropertyValue =0;
                                GameManager.Instance.rentToPay = 0;
                                // propertyToSell = null;
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
                    if (property.currentStageIndex < 4)
                    {
                        if (property.buyoutPrices[property.currentStageIndex] <= Money )
                        {
                            ShowBuyOutPopUp(property, this);
                        }
                        else if (property.buyoutPrices[property.currentStageIndex] > Money)
                        {
                            yield return StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                            // yield return new WaitForSecondsRealtime(2f);
                            gameManager.EndedAllInteraction = true;
                            yield break;                    
                        }
                        
                        // yield return StartCoroutine(WaitForPropertyDecision());
                        yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
                        yield return new WaitForSecondsRealtime(1f);
                        PlayerController ownerPlayeragain = FindPlayerByID(property.ownerID);
                        if (property.stagePrices[property.nextStageIndex] <= Money && ownerPlayeragain.teamID == this.teamID)
                        {
                            ShowBuy012(property, this);
                            
                        }
                        else if (property.stagePrices[property.nextStageIndex] > Money && ownerPlayeragain.teamID == this.teamID)
                        {
                            yield return StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                            // yield return new WaitForSecondsRealtime(2f);
                            gameManager.EndedAllInteraction = true;
                            yield break;
                        }
                    }
                    
                    else if (property.currentStageIndex == 4)
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
        while (buy012PopUp != null && buy012PopUp.gameObject.activeSelf|| buyoutPopup != null&& buyoutPopup.gameObject.activeSelf || hotspringpopup!= null&& hotspringpopup.gameObject.activeSelf )
        {
            while (buy012PopUp.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            }
            while (buyoutPopup.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
            }
            while (hotspringpopup.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
            }
        }
        gameManager.EndedAllInteraction = true;
    }

}
