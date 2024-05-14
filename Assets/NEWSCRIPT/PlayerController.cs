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
    private GameObject MessagePrefab;
    private GameObject ChancePrefab;
    private string ChancePrefabPath = "ChancePrefab";
    

    private BuyPropertyPopup012 buyPropertyPopup012Prefab;
    public string buyPropertyPopup012PrefabPath = "BuyPropertyPopup012Prefab"; // Path to the prefab in the Resources folder
    private BuyPropertyPopup012 BuypopupInstance;

    public string buyoutPopupPrefabPath = "buyoutPopupPrefab"; // The path to the MessagePrefab relative to the Resources folder
    private BuyOutPopUp buyoutPopupPrefab;
    private BuyOutPopUp buyoutPopupInstance;

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
    public TextMeshProUGUI plus300TextPrefab;
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
    
    
    private string MessagePrefabPath = "MessagePrefab";
 

 
    


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

        LoadAndCachePrefabs();
        LoadBuyPropertyPopup012Prefab();
        LoadBuyoutPopupPrefab();


        SetFontSize(dice1InputField, 50);
        SetFontSize(dice2InputField, 50);
        // PropertyManager.Instance.OnPropertiesLoaded += OnPropertiesLoaded;
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
    private void LoadAndCachePrefabs()
    {
        ChancePrefab = LoadPrefab(ChancePrefabPath);
        if (ChancePrefab == null)
        {
            Debug.LogError("Failed to load ChancePrefab from Resources folder at path: " + ChancePrefabPath);
        }  
        MessagePrefab = LoadPrefab(MessagePrefabPath);
        if (MessagePrefab == null)
        {
            Debug.LogError("Failed to load MessagePrefab from Resources folder at path: " + MessagePrefabPath);
        }      
    }

    private void LoadBuyPropertyPopup012Prefab()
    {
        buyPropertyPopup012Prefab = Resources.Load<BuyPropertyPopup012>("BuyPropertyPopup012Prefab");
        if (buyPropertyPopup012Prefab == null)
        {
            Debug.LogError("BuyPropertyPopup012Prefab not found in Resources folder.");
        }
    }
    private void LoadBuyoutPopupPrefab()
    {
        buyoutPopupPrefab = Resources.Load<BuyOutPopUp>("buyoutPopupPrefab");
        if (buyoutPopupPrefab == null)
        {
            Debug.LogError("BuyoutPopupPrefab not found in Resources folder.");
        }
    } 
    private GameObject LoadPrefab(string prefabPath)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab from Resources folder at path: " + prefabPath);
        }
        return prefab;
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
    private void InstantiateBuyPropertyPopup012(PropertyManager.PropertyData property)
    {
        // Find the Canvas GameObject
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas GameObject not found.");
            return;
        }
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
                int diceValue1 = int.Parse(dice1InputField.text);
                int diceValue2 = int.Parse(dice2InputField.text);           
                StartCoroutine(RollTheDice(diceValue1 , diceValue1));
                ShowMessage("Your Get out of Jail Ticket has been redeemed! You are now released from jail without penalty.");
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
                yield return StartCoroutine(WaitForPropertyDecision());
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
                ShowMessage("You have used your Get out of jail Ticket");
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
            GameObject ChancePrefabInstance = Instantiate(ChancePrefab, canvasTransform);
            yield return new WaitForSecondsRealtime(2f);
            Destroy(ChancePrefabInstance);
            yield return new WaitForSecondsRealtime(0.5f);
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
            ShowMessage($"You paid income tax of ${taxAmount}");
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

                yield return StartCoroutine(WaitForPropertyDecision());
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
            yield return StartCoroutine(WaitForPropertyDecision());
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
                DisplayPlus300();
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
                    yield return new WaitForSecondsRealtime(2f);
                    gameManager.EndedAllInteraction = true;
                    yield break;
                }
            }
            else if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
            {
                int rentPriceToDeduct = property.rentPrices[property.currentStageIndex];

                GameObject rentMessageObject = Instantiate(MessagePrefab, canvasTransform);
                TextMeshProUGUI RentMessageText = rentMessageObject.GetComponentInChildren<TextMeshProUGUI>();

                string formattedRent = FormatMoney(rentPriceToDeduct);
                RentMessageText.text = "You pay a rent of $" + formattedRent;

                yield return new WaitForSecondsRealtime(1f);
                Destroy(rentMessageObject);

                if (hasFreeRentTicket)
                {
                    hasFreeRentTicket = false;
                    ShowMessage("Your Free Meal Ticket has been redeemed! Enjoy your complimentary meal.");
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
                        InstantiateBuyoutPopup(property);
                    }
                    else if (property.buyoutPrices[property.currentStageIndex] > Money)
                    {
                        ShowMessage("Not enough money to acquire this property!");
                        yield return new WaitForSecondsRealtime(2f);
                        gameManager.EndedAllInteraction = true;
                        yield break;                    
                    }
                    
                    yield return StartCoroutine(WaitForPropertyDecision());
                    yield return new WaitForSecondsRealtime(1f);
                    PlayerController ownerPlayeragain = FindPlayerByID(property.ownerID);
                    if (property.stagePrices[property.nextStageIndex] <= Money && ownerPlayeragain.teamID == this.teamID)
                    {
                        InstantiateBuyPropertyPopup012(property);
                        
                    }
                    else if (property.stagePrices[property.nextStageIndex] > Money && ownerPlayeragain.teamID == this.teamID)
                    {
                        ShowMessage("Not enough money to acquire this property!");
                        yield return new WaitForSecondsRealtime(2f);
                    
                        gameManager.EndedAllInteraction = true;
                        yield break;
                    }
                }
                
                else if (property.currentStageIndex == 4)
                {
                    ShowMessage("You can't buy out the hotel");
                    yield return new WaitForSecondsRealtime(2f);
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

    public void ShowMessage(string message)
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
        yield return new WaitForSecondsRealtime(1f); // Wait for 1 second
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
        TextMeshProUGUI plus300Text = Instantiate(plus300TextPrefab, canvasTransform);
        plus300Text.gameObject.SetActive(true);
        StartCoroutine(HidePlus300Text(plus300Text));
    }
    private IEnumerator HidePlus300Text(TextMeshProUGUI plus300Text)
    {
        yield return new WaitForSecondsRealtime(2f);
        Destroy(plus300Text.gameObject); // Destroy the Plus300Text object after 2 seconds
    }

    public IEnumerator WaitForPropertyDecision()
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
        }
        gameManager.EndedAllInteraction = true;
    }

}
