using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class FollowThePath : MonoBehaviour {

    public Transform[] waypoints;
    [SerializeField]
    private float moveSpeed = 1f;
    [HideInInspector]
    public int waypointIndex = 0;
    public bool moveAllowed = false;
    public bool InJail = false;
    public int Money = 0;
    public TextMeshProUGUI plus200Text;
    public int[] propertyStages = new int[5];
    public TextMeshProUGUI moneyText; // Reference to the TextMeshProUGUI object for displaying money
    public TextMeshProUGUI[] propertyTexts;   
 
    private void Start () 
    {
        transform.position = waypoints[waypointIndex].transform.position;
        if (waypointIndex == 0)
        {
            Money += 200; 
            DisplayPlus200();   
        }
    }
	
    private void Update () {
        if (moveAllowed)
            Move();
	}

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
        waypoints[waypointIndex].transform.position,
        moveSpeed * Time.deltaTime);

        if (transform.position == waypoints[waypointIndex].transform.position)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;

            // Check if the player has reached the first waypoint
            if (waypointIndex == 0)
            {
                Money += 200;
                DisplayPlus200(); // Add 200 to player's money
            }
        }
    }

    private void DisplayPlus200()
    {
        plus200Text.gameObject.SetActive(true);
        plus200Text.text = "+200";
        StartCoroutine(HidePlus200Text());
    }

    private IEnumerator HidePlus200Text()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        // Disable the UI Text element after 2 seconds
        plus200Text.gameObject.SetActive(false);
    }
  // Track stages of each property

    public void BuyNextStage(int propertyIndex)
    {
        int price = CalculatePriceForNextStage(propertyIndex);
        if (Money >= price && propertyStages[propertyIndex] < 4)
        {
            Money -= price; // Deduct money
            propertyStages[propertyIndex]++; // Increment property stage
            UpdateMoneyText(); // Update money text on UI
            UpdatePropertyText(propertyIndex); // Update property text on UI
        }
    }
    private int CalculatePriceForNextStage(int propertyIndex)
    {
        // Implement your logic to calculate the price for the next stage of the property
        // This can be based on the current stage, predefined prices, etc.
        // For simplicity, you can use a fixed price per stage in this example.
        return (propertyStages[propertyIndex] + 1) * 100; // Example: Each stage costs 100 more than the previous one
    }
    private void UpdateMoneyText()
    {
        moneyText.text = "$" + Money.ToString(); // Update the text with the money value
    }
    private void UpdatePropertyText(int propertyIndex)
    {
        propertyTexts[propertyIndex].text = "Stage: " + propertyStages[propertyIndex]; // Update the text with the property stage value
    }

}
