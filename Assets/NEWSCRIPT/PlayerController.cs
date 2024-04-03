using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rollButton.onClick.AddListener(RollDiceOnClick);
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        transform.position = waypoints[waypointIndex].transform.position;
        UpdateMoneyText();
        playerMoveText.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);       
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

    // private void RollTheDiceAfterJail()
    // {
    //     StartCoroutine(RollTheDice());
    // }


    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int[] diceValues = new int[2];

        // Roll the dice
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

        if (isDoubles||currentPosition == 8 )
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
    currentPosition += steps;

    // You may want to check if the player has landed on a property here
    // and display the buy property popup if necessary
    }
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


    private void UpdateMoneyText()
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
