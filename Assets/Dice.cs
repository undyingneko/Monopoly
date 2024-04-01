using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Dice : MonoBehaviour {

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int currentPlayer = 1;
    private bool coroutineAllowed = true;
    public Button rollButton; 
    // public Image diceImage;   
    public Image[] diceImages;// Array to hold references to the dice images
    public TextMeshProUGUI sumText;


	// Use this for initialization
	private void Start () {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        rend.sprite = diceSides[5];
                // Add listener to the button
        rollButton.onClick.AddListener(RollDiceOnClick);
        Debug.Log("Start method called");
	}

    private void RollDiceOnClick()
    {
        Debug.Log("RollDiceOnClick method called");
        if (!GameControl.gameOver && coroutineAllowed)
            StartCoroutine("RollTheDice");
    }
    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int[] diceValues = new int[2];// Array to hold the values of two dice
        // int randomDiceSide = 0;
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
        // Calculate the sum of the two dice values
        int sum = diceValues[0] + diceValues[1];
        GameControl.diceSideThrown = sum;

        // Handle player movement
        GameControl.MovePlayer(currentPlayer);

        // Move to the next player's turn
        currentPlayer = (currentPlayer % 4) + 1;
    

        coroutineAllowed = true;
        sumText.text = "" + sum;
    }
}
