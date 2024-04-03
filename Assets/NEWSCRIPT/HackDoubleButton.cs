using UnityEngine;
using UnityEngine.UI;

public class HackDoubleButton : MonoBehaviour
{
    public Button hackDoubleButton;
    public PlayerController playerController;

    void Start()
    {
        hackDoubleButton.onClick.AddListener(HackDouble);
    }

    void HackDouble()
    {
        // Simulate rolling doubles and send the player to jail
        int[] diceValues = { 6, 6 }; // Doubles (you can change this to any doubles you want)
        playerController.HackRollDice(diceValues);

    }

            // coroutineAllowed = false;
        // int[] diceValues = new int[2];

        // // For testing purposes, set the dice values to double 6
        // diceValues[0] = 6;
        // diceValues[1] = 6;

        // // Simulate dice rolling animation (optional)
        // // You can remove this loop if you don't need the rolling animation
        // for (int i = 0; i <= 20; i++)
        // {
        //     for (int j = 0; j < diceImages.Length; j++)
        //     {
        //         diceImages[j].sprite = diceSides[5]; // Use the sprite for dice side 6
        //     }

        //     yield return new WaitForSeconds(0.05f);
        // }

        // // Calculate the sum of dice values
        // int sum = diceValues[0] + diceValues[1];
        // isDoubles = (diceValues[0] == diceValues[1]);
}
