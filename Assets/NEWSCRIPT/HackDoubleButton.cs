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
        // playerController.HackRollDice(diceValues);

    }
}
