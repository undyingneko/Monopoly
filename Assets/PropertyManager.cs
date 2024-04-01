// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PropertyManager : MonoBehaviour
// {
//     public List<Property> properties;

//     void Start()
//     {
//         // Instantiate properties and assign values
//         properties = new List<Property>
//         {
//             new Property { propertyName = "Park Place", stagePrices = new List<int> { 350 }, stageRentAmounts = new List<int> { 35 } },
//             new Property { propertyName = "Boardwalk", stagePrices = new List<int> { 400 }, stageRentAmounts = new List<int> { 40 } },
//             // Add more properties as needed
//         };

//         // Print property information
//         foreach (Property property in properties)
//         {
//             Debug.Log("Property Name: " + property.propertyName);
//             Debug.Log("Price: " + property.stagePrices[0]); // Accessing the price of the first stage
//             Debug.Log("Rent Amount: " + property.stageRentAmounts[0]); // Accessing the rent amount of the first stage
//             // Add more property attributes to print
//         }
//     }
// }