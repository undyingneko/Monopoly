using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public int playerID;
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
    public int Money = 0;
    public TextMeshProUGUI plus200Text;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI playerMoveText;
    private int consecutiveDoublesCount = 0;
    public bool isDoubles = false;
    public bool InJail = false;
    private int turnsInJail = 0;
    public TextMeshProUGUI goToJailText;

    public BuyPropertyPopup buyPropertyPopup;
    private PropertyManager propertyManager;

    public List<PropertyManager.PropertyData> properties;
    
    void Start()
    {   
        propertyManager = PropertyManager.Instance;
        gameManager = FindObjectOfType<GameManager>();

        rollButton.onClick.AddListener(RollDiceOnClick);
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        transform.position = waypoints[waypointIndex].transform.position;
        UpdateMoneyText();
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);

        buyPropertyPopup = FindObjectOfType<BuyPropertyPopup>();
        properties = propertyManager.properties;   
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
        coroutineAllowed = false;
        int[] diceValues = new int[2];

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
        
        int sum = diceValues[0] + diceValues[1];
        sumText.text = "" + sum; 
        isDoubles = (diceValues[0] == diceValues[1]); 

        if (isDoubles)
        {
            InJail = false;
            MovePlayer(sum);
            turnsInJail = 0;
            EndTurn();
            coroutineAllowed = false;
            
        }
        else
        {
            if (turnsInJail >= 3)
            {   
                InJail = false;
                MovePlayer(sum);
                turnsInJail = 0;
                EndTurn();
                coroutineAllowed = false;
                
                // RollTheDiceAfterJail();
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

        // //-----------------------
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
        // //---------------------
        // // For testing purposes, set the dice values to double 6
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

        int sum = diceValues[0] + diceValues[1];
        sumText.text = "" + sum; 

        // Allow other actions and then check for doubles
        yield return new WaitForSeconds(0.1f);
        
        MovePlayer(diceValues[0] + diceValues[1]);
        
        CheckForDoubles(diceValues);
        coroutineAllowed = true; 
    }

    public void HackRollDice(int[] diceValues)
    {
        CheckForDoubles(diceValues);
        
    }

    private void CheckForDoubles(int[] diceValues)
    {   
        isDoubles = (diceValues[0] == diceValues[1]);

        if (isDoubles)
        {
            consecutiveDoublesCount++;
            
            if (consecutiveDoublesCount >= 3)
            {
                consecutiveDoublesCount = 0;
                waypointIndex = 8;
                transform.position = waypoints[waypointIndex].position;
                DisplayGoToJailText();
                InJail = true;
                EndTurn();
                return;
            }
            else
            {
                StartTurn();
                
            }
        }

        else
        {
            consecutiveDoublesCount = 0;
            EndTurn();
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
                if (waypointIndex == 0)
                {
                    Money += 200;
                    DisplayPlus200();
                    UpdateMoneyText();
                }
                waypointIndex = (waypointIndex + 1) % waypoints.Length;

                remainingSteps--;
            }
            yield return null;
            
        }

    // Update the current position
    currentPosition = (currentPosition + steps) % waypoints.Length;
    LandOnProperty();
    }
    // You may want to check if the player has landed on a property here

    private void LandOnProperty()
    {   

        if (currentPosition == 8)
        {
            // Display the Go to Jail text and handle the jail logic
            DisplayGoToJailText();                 
            InJail = true;
            consecutiveDoublesCount = 0;
            EndTurn();
            return;
        }
        // Get the property data for the current waypoint index from the PropertyManager
        PropertyManager.PropertyData property = propertyManager.GetPropertyByWaypointIndex(currentPosition);
        
        if (property != null)
        {
            // Check if the property is unowned and the player has enough money to buy it
            if (!property.owned && property.prices[0] <= Money)
            {
                // Display the buy property popup with property details
                buyPropertyPopup.Display(property);
            }
            else
            {
                PayRent(property);
            }
        }
    }

    private void PayRent(PropertyManager.PropertyData property)
    {
        // Deduct rent from the player's money
        Money -= property.rent;
        
        // Find the owner player object and add rent amount to their money
        PlayerController ownerPlayer = FindPlayerByID(property.ownerID);
        if (ownerPlayer != null)
        {
            ownerPlayer.Money += property.rent;
        }

        // Update UI for both players
        UpdateMoneyText();
        ownerPlayer.UpdateMoneyText();
    }



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
    }
    public void EndTurn()
    {
        isTurn = false;
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);
        gameManager.NextTurn();
        
    }

    public void StartTurn()
    {
        isTurn = true;
        rollButton.gameObject.SetActive(true);
        playerMoveText.gameObject.SetActive(true);
    }
    private void DisplayPlus200()
    {
        plus200Text.gameObject.SetActive(true);
        plus200Text.text = "+200";
        StartCoroutine(HidePlus200Text());
    }
    private IEnumerator HidePlus200Text()
    {
        yield return new WaitForSeconds(2f);
        plus200Text.gameObject.SetActive(false);
    }

}
