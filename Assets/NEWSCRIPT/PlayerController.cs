using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

public class PlayerController : MonoBehaviour
{

    public List<PropertyManager.PropertyData> properties;
    public List<PropertyManager.PropertyData> ownedProperties = new List<PropertyManager.PropertyData>();
    public List<PropertyManager.PropertyData> opponentProperties;
    public PropertyManager.PropertyData propertyToDestroy;
    public PropertyManager.PropertyData propertyToDemolish;
    public PropertyManager.PropertyData propertyToSeize; 
    // private PlayerController currentPlayerController;
    
    private Sprite[] diceSides;

    public GameObject MessageObject;
    public GameObject ChancePopUp;

    public BuyPropertyPopup012 buy012PopUp;
    public BuyOutPopUp buyoutPopup;

    public int playerID;
    public int teamID;
    public TextMeshProUGUI teamNumberText;

    public int currentPosition;
    private bool isTurn;

    public Button rollButton;
    public Image[] diceImages;
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

    // private GameObject darkenScreenGameObject;

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder = 0;
    
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

    public bool hasGetOutOfJailCard = false;
    public bool hasFreeRentTicket = false;

    public TMP_InputField dice1InputField;
    public TMP_InputField dice2InputField;

    void Start()
    {   
        propertyManager = PropertyManager.Instance;
       
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(StartRollDiceCoroutine);

        if (diceSides == null || diceSides.Length == 0)
        {
            LoadDiceSides();
        }
        transform.position = waypoints[waypointIndex].transform.position;
        
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);    
        properties = propertyManager.properties;
  
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
    private void ShowBuy012(PropertyManager.PropertyData property, PlayerController player)
    {
        buy012PopUp.playerController = player;
        buy012PopUp.gameObject.SetActive(true);
        buy012PopUp.Display012(property);
    }
    private void ShowBuyOutPopUp(PropertyManager.PropertyData property, PlayerController player)
    {
        buyoutPopup.playerController = player;
        buyoutPopup.gameObject.SetActive(true);
        buyoutPopup.DisplayBuyOut(property);
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

    private void LoadDiceSides()
    {
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("Failed to load sprites from DiceSides folder.");
        }
        else
        {
            Debug.Log("Loaded " + diceSides.Length + " sprites from DiceSides folder.");
        }
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
                int diceValue1 = int.Parse(dice1InputField.text);
                int diceValue2 = int.Parse(dice2InputField.text);           
                StartCoroutine(RollTheDice(diceValue1 , diceValue1));
                StartCoroutine(ShowMessage("Your Get out of Jail Ticket has been redeemed! You are now released from jail without penalty."));
                yield return new WaitForSecondsRealtime(2f);
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
            }
            else
            {
                int diceValue1 = int.Parse(dice1InputField.text); 
                int diceValue2 = int.Parse(dice2InputField.text);
                StartCoroutine(RollTheDice(diceValue1 , diceValue2));
                rollButton.gameObject.SetActive(false);
                playerMoveText.gameObject.SetActive(false);
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

        //     yield return new WaitForSecondsRealtime(0.05f);
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

            yield return new WaitForSecondsRealtime(0.05f);
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

        //-----------------------
        // for (int i = 0; i <= 20; i++)
        // {
        //     for (int j = 0; j < diceImages.Length; j++)
        //     {
        //         int randomDiceSide = Random.Range(0, 6);
        //         diceImages[j].sprite = diceSides[randomDiceSide];
        //         diceValues[j] = randomDiceSide + 1;
        //     }
        //     yield return new WaitForSecondsRealtime(0.05f);
        // }
        //---------------------
        // For testing purposes, set the dice values to double 6
        diceValues[0] = dice1Value;
        diceValues[1] = dice2Value;
        for (int i = 0; i <= 20; i++)
        {
            for (int j = 0; j < diceImages.Length; j++)
            {
                diceImages[j].sprite = diceSides[3]; // Use the sprite for dice side 6
            }

            yield return new WaitForSecondsRealtime(0.05f);
        }      
        //---------------------

        int sum = diceValues[0] + diceValues[1];
        // int sum = 7;
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
                StartCoroutine(ShowMessage("You have used your Get out of jail Ticket"));
                yield return new WaitForSecondsRealtime(2f);

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
            StartCoroutine(ShowMessage($"You paid income tax of ${taxAmount}"));
            yield return new WaitForSecondsRealtime(2f);
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

    public IEnumerator WaitForPlayerSelection()
    {
        bool selectionMade = false;

        while (!selectionMade)
        {
            // Wait for the next frame to allow the player to click on a tile
            yield return null;

            // Check if the player has made a selection
            if (gameManager.selectedProperty != null)
            {
                selectionMade = true;
            }
        }
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
            yield return StartCoroutine(WaitForPropertyDecision());
            yield break;
        }

        // Check if the property is unowned and the player has enough money to buy it
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
                    StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                    yield return new WaitForSecondsRealtime(2f);
                    gameManager.EndedAllInteraction = true;
                    yield break;
                }
            }
            else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
            {
                int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];
                string formattedRent = FormatMoney(rentPriceToDeduct);
                StartCoroutine(ShowMessage("You pay a rent of $" + formattedRent));
                if (hasFreeRentTicket)
                {
                    hasFreeRentTicket = false;
                    StartCoroutine(ShowMessage("Your Free Meal Ticket has been redeemed! Enjoy your complimentary meal."));
                    yield return new WaitForSecondsRealtime(2f);
                }
                else
                {
                    Money -= rentPriceToDeduct;
                    UpdateMoneyText();
                }

                yield return new WaitForSecondsRealtime(1f);

                ownerPlayer.Money += rentPriceToDeduct;
                ownerPlayer.UpdateMoneyText();

                if (property.currentStageIndex < 4)
                {
                    if (property.buyoutPrices[property.currentStageIndex] <= Money )
                    {
                        ShowBuyOutPopUp(property, this);
                    }
                    else if (property.buyoutPrices[property.currentStageIndex] > Money)
                    {
                        StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                        yield return new WaitForSecondsRealtime(2f);
                        gameManager.EndedAllInteraction = true;
                        yield break;                    
                    }
                    
                    yield return StartCoroutine(WaitForPropertyDecision());
                    yield return new WaitForSecondsRealtime(1f);
                    PlayerController ownerPlayeragain = FindPlayerByID(property.ownerID);
                    if (property.stagePrices[property.nextStageIndex] <= Money && ownerPlayeragain.teamID == this.teamID)
                    {
                        ShowBuy012(property, this);
                        
                    }
                    else if (property.stagePrices[property.nextStageIndex] > Money && ownerPlayeragain.teamID == this.teamID)
                    {
                        StartCoroutine(ShowMessage("Not enough money to acquire this property!"));
                        yield return new WaitForSecondsRealtime(2f);
                        gameManager.EndedAllInteraction = true;
                        yield break;
                    }
                }
                
                else if (property.currentStageIndex == 4)
                {
                    StartCoroutine(ShowMessage("You can't buy out the hotel"));
                    yield return new WaitForSecondsRealtime(2f);
                    yield return StartCoroutine(WaitForPropertyDecision());
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
        //         yield return new WaitForSecondsRealtime(2f);
        //         buyPropertyPopup.buyPropertyDecisionMade = true;
        //         yield break;
        //     }
        //     else
        //     {
        //         Debug.LogError("Rent price data is missing for property.");
        //     }
        // }
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
        yield return new WaitForSecondsRealtime(1f); // Wait for 1 second
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
        while (buy012PopUp != null && buy012PopUp.gameObject.activeSelf|| buyoutPopup != null&& buyoutPopup.gameObject.activeSelf)
        {
            while (buy012PopUp.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyPropertyDecisionMade);
            }
            while (buyoutPopup.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => gameManager.buyOutDecisionMade);
            }
        }
        gameManager.EndedAllInteraction = true;
    }

}
