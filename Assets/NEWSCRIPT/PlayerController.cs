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
            int totalPropertyValue = 0;
            foreach (PropertyManager.PropertyData property in ownedProperties)
            {
                totalPropertyValue += property.stagePrices[property.currentStageIndex];
            }
            int taxAmount = (int)(totalPropertyValue * 0.1f);
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
        if (property == null && IsHotSpringPosition(currentPosition))
        {
            if (!hotspring.owned)
            {
                if (hotspring.priceHotSpring <= Money)
                {
                    yield return StartCoroutine(ShowMessage("You've discovered an onsen!"));
                    ShowHotSpringPopup(hotspring, this);
                }
                else
                {
                    yield return StartCoroutine(ShowMessage("You do not have enough money to build an onsen ryokan  at this onsen."));
                    gameManager.EndedAllInteraction = true;
                }
            }
            else
            {
                HandleOwnedHotSpring(hotspring);
            }
            
            yield return StartCoroutine(WaitForPropertyDecision());
            yield break;
        }

        // Handle standard properties
        if (property != null)
        {
            if (!property.owned && property.nextStageIndex <= 5 && property.stagePrices[property.nextStageIndex] <= Money)
            {
                ShowBuy012(property, this);
            }
            else if (property.owned)
            {
                HandleOwnedProperty(property);
            }
            
            yield return StartCoroutine(WaitForPropertyDecision());
        }
    }

    private bool IsHotSpringPosition(int position)
    {
        return position == 4 || position == 13 || position == 18 || position == 25;
    }

    private void HandleOwnedHotSpring(HotSpringManager.HotSpringData hotspring)
    {
        PlayerController ownerPlayer = FindPlayerByID(hotspring.ownerID);

        if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
        {
            int rentPriceToDeduct = hotspring.rentPriceSHotSpring;
            string formattedRent = FormatMoney(rentPriceToDeduct);
            Money -= rentPriceToDeduct;
            UpdateMoneyText();
            ownerPlayer.Money += rentPriceToDeduct;
            ownerPlayer.UpdateMoneyText();
            StartCoroutine(ShowMessage($"You pay the hot spring entry fee of ${formattedRent}"));
        }
    }

    private void HandleOwnedProperty(PropertyManager.PropertyData property)
    {
        PlayerController ownerPlayer = FindPlayerByID(property.ownerID);

        if (ownerPlayer != null && ownerPlayer.teamID == this.teamID)
        {
            HandleSameTeamProperty(property);
        }
        else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
        {
            HandleDifferentTeamProperty(property, ownerPlayer);
        }
    }

    private void HandleSameTeamProperty(PropertyManager.PropertyData property)
    {
        if (property.nextStageIndex <= property.stagePrices.Count && property.stagePrices[property.nextStageIndex] <= Money)
        {
            ShowBuy012(property, this);
        }
        else
        {
            StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
            gameManager.EndedAllInteraction = true;
        }
    }

    private void HandleDifferentTeamProperty(PropertyManager.PropertyData property, PlayerController ownerPlayer)
    {
        int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];
        string formattedRent = FormatMoney(rentPriceToDeduct);

        if (property.isComplimentaryMeal)
        {
            StartCoroutine(ShowMessage("This Food Stall offers a complimentary meal. No payment required"));
        }
        else
        {
            if (hasFreeRentTicket)
            {
                hasFreeRentTicket = false;
                StartCoroutine(ShowMessage("Your Free Meal Ticket has been redeemed! Enjoy your complimentary meal."));
            }
            else
            {
                Money -= rentPriceToDeduct;
                UpdateMoneyText();
                ownerPlayer.Money += rentPriceToDeduct;
                ownerPlayer.UpdateMoneyText();
                StartCoroutine(ShowMessage($"You pay the meal expense of ${formattedRent}"));
            }
        }

        if (property.currentStageIndex < 4)
        {
            if (property.buyoutPrices[property.currentStageIndex] <= Money)
            {
                ShowBuyOutPopUp(property, this);
            }
            else
            {
                StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                gameManager.EndedAllInteraction = true;
            }
        }
        else
        {
            StartCoroutine(ShowMessage("You can't buy out the hotel"));
            gameManager.EndedAllInteraction = true;
        }
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
