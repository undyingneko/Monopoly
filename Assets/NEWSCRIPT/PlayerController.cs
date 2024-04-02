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
    public bool InJail = false;
    public int Money = 0;
    public TextMeshProUGUI plus200Text;
    public TextMeshProUGUI moneyText; // Reference to the TextMeshProUGUI object for displaying money



    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rollButton.onClick.AddListener(RollDiceOnClick);
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        transform.position = waypoints[waypointIndex].transform.position;
        if (waypointIndex == 0)
        {
            Money += 200; 
            DisplayPlus200();   
        }
        UpdateMoneyText();
    }

    private void RollDiceOnClick()
    {
        if (!GameManager.GameOver && isTurn && coroutineAllowed)
            StartCoroutine("RollTheDice");
    }

    private IEnumerator RollTheDice()
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
        MovePlayer(sum);
        sumText.text = "" + sum;
        coroutineAllowed = true;
        gameManager.NextTurn();
    }

    void MovePlayer(int steps)
    {
        StartCoroutine(MovePlayerCoroutine(steps));
    }

IEnumerator MovePlayerCoroutine(int steps)
{
    int remainingSteps = steps;

    while (remainingSteps + 1 > 0)
    {
        float stepDistance = moveSpeed * Time.deltaTime;
        float distanceToNextWaypoint = Vector2.Distance(transform.position, waypoints[waypointIndex].position);
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, stepDistance);
        if (distanceToNextWaypoint < stepDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            if (waypointIndex == 0)
            {
                Money += 200;
                DisplayPlus200();
                UpdateMoneyText();
            }
            remainingSteps--;
        }
        yield return null;
    }

    // Update the current position
    currentPosition += steps;

    // You may want to check if the player has landed on a property here
    // and display the buy property popup if necessary
}
    private void UpdateMoneyText()
    {
        // Update the text displayed on the moneyText object
        moneyText.text = Money.ToString();
    }
    public void EndTurn()
    {
        isTurn = false;
    }

    public void StartTurn()
    {
        isTurn = true;
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
