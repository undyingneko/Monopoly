// using UnityEngine;
// using System.Collections.Generic;

// public class Property
// {
//     public string name;
//     public int price;
//     public int currentStage;
// }

// public class Player
// {
//     public int money;
//     public Dictionary<string, int> propertyStages = new Dictionary<string, int>();
// }

// public class GameManager : MonoBehaviour
// {
//     public List<Property> properties;
//     public Player player;

//     private void Start()
//     {
//         // Initialize the player's money
//         player.money = 1000;

//         // Initialize the player's property stages
//         foreach (Property property in properties)
//         {
//             player.propertyStages[property.name] = 0;
//         }
//     }

//     public void BuyNextStage(Property property)
//     {
//         if (player.money >= property.price && player.propertyStages[property.name] < 4)
//         {
//             player.money -= property.price;
//             player.propertyStages[property.name]++;
//         }
//     }

//     public bool CanBuyFifthStage(Property property)
//     {
//         return player.propertyStages[property.name] >= 4;
//     }
// }