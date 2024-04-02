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
    public int[][] propertyPrices;
    public PropertyPopup propertyPopup;
 
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
        if (!moveAllowed && Money > 0)
        {
            int lastWaypointIndex = waypointIndex % waypoints.Length;
            PropertyManager.PropertyData propertyData = PropertyManager.Instance.GetPropertyByWaypointIndex(lastWaypointIndex);
            if (propertyData != null)
            {
                foreach (int price in propertyData.prices)
                {
                    if (Money >= price)
                    {
                        propertyPopup.ShowPropertyDetails(propertyData.name, propertyData.prices);
                        break; // Exit the loop since the player can buy at least one stage
                    }
                }
            }
        }
	}

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
        waypoints[waypointIndex].transform.position,
        moveSpeed * Time.deltaTime);

        if (transform.position == waypoints[waypointIndex].transform.position)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            if (waypointIndex == 0)
            {
                Money += 200;
                DisplayPlus200();
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
        yield return new WaitForSeconds(2f);
        plus200Text.gameObject.SetActive(false);
    }
  // Track stages of each property

    public void BuyNextStage(int propertyIndex)
    {
        if (Money >= propertyPrices[propertyIndex][propertyStages[propertyIndex]] && propertyStages[propertyIndex] < 4)
        {
            Money -= propertyPrices[propertyIndex][propertyStages[propertyIndex]]; // Deduct money
            propertyStages[propertyIndex]++; // Increment property stage
            UpdateMoneyText(); // Update money text on UI
            UpdatePropertyText(propertyIndex); // Update property text on UI
        }
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
