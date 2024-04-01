// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class BuyPopUp : MonoBehaviour
// {
//     public GameObject popUpPrefab; // Reference to the pop-up prefab
//     public Transform popUpParent; // Parent transform to instantiate pop-ups under

//     public void ShowPopUp(Property property)
//     {
//         // Instantiate a pop-up window
//         GameObject popUp = Instantiate(popUpPrefab, popUpParent);

//         // Get references to UI elements in the pop-up window
//         Text propertyNameText = popUp.transform.Find("PropertyName").GetComponent<Text>();
//         Button buyStage1Button = popUp.transform.Find("BuyStage1Button").GetComponent<Button>();
//         Button buyStage2Button = popUp.transform.Find("BuyStage2Button").GetComponent<Button>();
//         Button buyStage3Button = popUp.transform.Find("BuyStage3Button").GetComponent<Button>();

//         // Set the property name text
//         propertyNameText.text = property.propertyName;

//         // Set the buy price text for each stage
//         for (int i = 0; i < property.stagePrices.Count; i++)
//         {
//             Button buyButton = null;
//             switch (i)
//             {
//                 case 0:
//                     buyButton = buyStage1Button;
//                     break;
//                 case 1:
//                     buyButton = buyStage2Button;
//                     break;
//                 case 2:
//                     buyButton = buyStage3Button;
//                     break;
//             }
//             if (buyButton != null)
//             {
//                 buyButton.GetComponentInChildren<Text>().text = "Buy Stage " + (i + 1) + " - $" + property.stagePrices[i];
//                 int stageIndex = i; // To capture the current value of i in the lambda
//                 buyButton.onClick.AddListener(() => BuyStage(property, stageIndex));
//             }
//         }
//     }

//     void BuyStage(Property property, int stageIndex)
//     {
//         FollowThePath currentPlayer = GameControl.GetCurrentPlayer();
//         if (property.BuyNextStage(currentPlayer, stageIndex))
//         {
//         // Successfully bought the stage, update UI or perform other actions
//         }
//         else
//         {
//         // Player doesn't have enough money to buy this stage, show error message or handle accordingly
//         }
//     }
// }
