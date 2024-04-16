using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    PlayerController playerController;
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

    
    private BuyPropertyPopup buyPropertyPopupPrefab;
    public string buyPropertyPopupPrefabPath = "BuyPropertyPopupPrefab"; // Path to the prefab in the Resources folder

    private PropertyManager propertyManager;

    public List<PropertyManager.PropertyData> properties;
    public List<PropertyManager.PropertyData> ownedProperties = new List<PropertyManager.PropertyData>();
    
    public bool isBuyPopUpActive = false;
    public bool buyPropertyDecisionMade = false;

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder = 0;

    public void AssignPlayerID(int id)
    {
        playerID = id;
    }    
    public void AssignTeamID(int teamid)
    {
        teamID = teamid;
        teamNumberText.text = "Team: " + teamID.ToString();
    }

    public void UpdatePropertyOwnership(int propertystageIndex)
    {
        for (int i = 0; i <= propertystageIndex; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                properties[j].owned = true;
            }
        }
    }

  
    void Start()
    {   
        playerController = FindObjectOfType<PlayerController>();
        propertyManager = PropertyManager.Instance;
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(RollDiceOnClick);
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        transform.position = waypoints[waypointIndex].transform.position;
        
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);

        buyPropertyPopupPrefab = Resources.Load<BuyPropertyPopup>("BuyPropertyPopupPrefab");
        if (buyPropertyPopupPrefab != null)
        {
            Debug.Log("Popup prefab loaded successfully.");
            
        }
        else
        {
            Debug.LogError("Failed to load popup prefab.");
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

    }
    private void InstantiateBuyPropertyPopup(PropertyManager.PropertyData property)
    {
        // Find the Canvas GameObject
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas GameObject not found.");
            return;
        }

        // Instantiate the buy property popup and assign it to a variable
        BuyPropertyPopup popupInstance = Instantiate(buyPropertyPopupPrefab, canvas.transform);

        if (popupInstance != null)
        {
            Debug.Log("Buy property popup instantiated successfully.");

            // Display the property data in the popup
            popupInstance.Display(property);
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
        diceValues[0] = 4;
        diceValues[1] = 4;
        for (int i = 0; i <= 20; i++)
        {
            for (int j = 0; j < diceImages.Length; j++)
            {
                diceImages[j].sprite = diceSides[3]; // Use the sprite for dice side 6
            }

            yield return new WaitForSeconds(0.05f);
        }      
        // //---------------------

        //------------------------------------------
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
        //-------------------------------------
        int sum = diceValues[0] + diceValues[1];
        sumText.text = "" + sum; 
        isDoubles = (diceValues[0] == diceValues[1]); 

        if (isDoubles)
        {
            InJail = false;
            turnsInJail = 0;
            Debug.Log("current position before moving in jail" + currentPosition); 
            MovePlayer(diceValues[0] + diceValues[1]);
            yield return new WaitUntil(() => playerController.buyPropertyDecisionMade);
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
                yield return new WaitUntil(() => playerController.buyPropertyDecisionMade);
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
        diceValues[0] = 4;
        diceValues[1] = 4;
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
            Debug.Log("exited roll dice normal. currentPlayerIndex: " + GameManager.currentPlayerIndex);
            yield break;
             // Exit the coroutine early if the player is in jail
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
                
                yield return new WaitUntil(() => playerController.buyPropertyDecisionMade);
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
                
                yield return new WaitUntil(() => playerController.buyPropertyDecisionMade);
                StartTurn();
                
            }
        }

        else
        {   
            consecutiveDoublesCount = 0;
            MovePlayer(diceValues[0] + diceValues[1]);
            yield return new WaitUntil(() => playerController.buyPropertyDecisionMade);
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

        while (remainingSteps >= 0)
        {
            float stepDistance = moveSpeed * Time.deltaTime;
            float distanceToNextWaypoint = Vector2.Distance(transform.position, waypoints[waypointIndex].position);
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, stepDistance);

            if (distanceToNextWaypoint < stepDistance)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                remainingSteps--;
            }

            yield return null;
        

        // Check if the player has completed a full loop around the board
            if (waypointIndex == 0)
            {
                Money += 300;
                DisplayPlus300();
                UpdateMoneyText();
            }
        }

    // Update the current position
    currentPosition = (currentPosition + steps) % waypoints.Length;
    Debug.Log("Current posision after moving only=" +currentPosition);
    
    LandOnProperty();
    }
    // You may want to check if the player has landed on a property here

    private void LandOnProperty()
    {


        // Get the property data for the current waypoint index from the PropertyManager
        PropertyManager.PropertyData property = propertyManager.GetPropertyByWaypointIndex(currentPosition);
        Debug.Log("Inside LandOnProperty method.");

        // Check if propertyManager is null
        if (propertyManager == null)
        {
            Debug.LogError("propertyManager is null.");
            return;
        }

        // Check if waypoints array is null
        if (waypoints == null)
        {
            Debug.LogError("waypoints array is null.");
            return;
        }

        if (property != null)
        {
            // Check if the property is unowned and the player has enough money to buy it
            if (!property.owned && property.prices[0] <= Money)
            {
                // Instantiate the buy property popup
                InstantiateBuyPropertyPopup(property);
            }
            if (property.owned)
            {   
                PlayerController ownerPlayer = FindTeamByID(property.ownerID);
                if (ownerPlayer != null && ownerPlayer.teamID != this.teamID)
                {
                    Money -= property.rent;
                    ownerPlayer.Money += property.rent;
                    UpdateMoneyText();
                    ownerPlayer.UpdateMoneyText();
                }
                // EndTurn();
                playerController.buyPropertyDecisionMade = true;
                
            }
        }
        else
        {
            Debug.LogWarning("Property is null. No popup will be displayed.");
            playerController.buyPropertyDecisionMade = true;
            // EndTurn();
        }
    }



    // private void PayRent(PropertyManager.PropertyData property)
    // {
    //     // Deduct rent from the player's money
    //     Money -= property.rent;
        
    //     // Find the owner player object and add rent amount to their money
    //     PlayerController ownerPlayer = FindPlayerByID(property.ownerID);
        
    //     if (ownerPlayer != null)
    //     {
    //         ownerPlayer.Money += property.rent;
    //     }

    //     // Update UI for both players
    //     UpdateMoneyText();
    //     ownerPlayer.UpdateMoneyText();
    // }

    // public void EndBuyPropertyInteraction()
    // {
    //     buyPropertyDecisionMade = true;
    //     Debug.Log("Decision is made");
    //     EndTurn();
        
    // }

    // Method to find player object by ID
    private PlayerController FindPlayerByID(int ID)
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
    
    private PlayerController FindTeamByID(int ID)
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
        gameManager.NextTurn();
        

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
            playerController.buyPropertyDecisionMade = false;
            
            
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
