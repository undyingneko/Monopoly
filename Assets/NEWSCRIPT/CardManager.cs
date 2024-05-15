using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    public GameObject CardObject;
    public static CardManager Instance;
    // Card class definition
    private GameManager gameManager;
    private PropertyManager propertyManager;

    public class Card
    {
        public string name;
        public string description;

        public Card(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;

    private Dictionary<string, Card> cardDeck;
    private Dictionary<string, System.Func<PlayerController, IEnumerator>> cardEffects;

    public Transform canvasTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure CardManager persists between scenes if needed
            InitializeCardDeck();
            InitializeCardEffects();
            gameManager = FindObjectOfType<GameManager>();
            propertyManager = PropertyManager.Instance;

            
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate CardManager instances
        }

    }

    private void InitializeCardDeck()
    {
        cardDeck = new Dictionary<string, Card>();

        // Populate the card deck
        // cardDeck.Add("Birthday Gift", new Card("Birthday Gift", "Collect a Birthday Gift of $15000 from each player."));
        // cardDeck.Add("Lottery Win: $200,000", new Card("Lottery Win", "Congratulations! You have won a lottery prize of $200,000."));
        // cardDeck.Add("Dog Poop Cleanup Fee", new Card("Dog Poop Cleanup Fee", "Oops! You have to pay a fee of $50,000 for dog poop cleanup."));
        // cardDeck.Add("Get out of Jail Ticket", new Card("Get out of Jail Ticket", "You can use this card to get out of jail once."));
        // cardDeck.Add("Go to Jail", new Card("Go to Jail", "Go directly to Jail. Do not pass 'Go,' do not collect $300,000"));
        // cardDeck.Add("Advance to Go", new Card("Advance to Go", "Move your character to the \"Go\" space on the board and collect $300,000 from the bank."));
        // cardDeck.Add("Go Back to Go", new Card("Go Back to Go", "Go back to \"Go\" without passing 'Go,' without collecting $300,000"));
        // cardDeck.Add("Advance 1 Space", new Card("Advance 1 Space", "Advance 1 space on the board."));
        // cardDeck.Add("Move Backward 1 Space", new Card("Move Backward 1 Space", "Move your character back one space on the board."));
        // cardDeck.Add("Tax Levy", new Card("Tax Levy", "Pay a tax equal to 10% of the total value of your owned properties."));
        // cardDeck.Add("Avenue Demolition", new Card("Avenue Demolition", "Demolish one of the opponent's avenues, leaving it ownerless."));
        // cardDeck.Add("Property Seizure", new Card("Property Seizure", "Force one opponent to sell one property of your choice from their holdings."));
        // cardDeck.Add("Natural Disaster", new Card("Natural Disaster", "An earthquake has destroyed 1 of your food stalls at the festival."));
        cardDeck.Add("Sales Slump", new Card("Sales Slump", "You're Forced to Sell One Food Stall to Keep Your Business Afloat"));
        // cardDeck.Add("Generous Treat", new Card("Generous Treat", "Select one food stall of the opponent. Any player landing on this stall is treated to a complimentary meal for one turn, no payment necessary."));
        // cardDeck.Add("Free Meal Ticket", new Card("Free Meal Ticket", "Receive a ticket for a complimentary meal at any food stall on your next visit."));
        // cardDeck.Add("Firework Spectacle", new Card("Firework Spectacle", "Select one of your stalls to host a firework display, turning it into a hot spot and increasing its value."));
    }

    private void InitializeCardEffects()
    {
        cardEffects = new Dictionary<string, System.Func<PlayerController, IEnumerator>>();

        // Define card effects
        cardEffects.Add("Birthday Gift", BirthdayGiftEffect);
        cardEffects.Add("Lottery Win: $200,000", LotteryWinEffect);
        cardEffects.Add("Dog Poop Cleanup Fee", DogPoopCleanupFeeEffect);
        cardEffects.Add("Get out of Jail Ticket", GetOutOfJailEffect);
        cardEffects.Add("Go to Jail", GoToJailEffect);
        cardEffects.Add("Advance to Go", AdvanceToGoEffect);
        cardEffects.Add("Go Back to Go", GoBackToGoEffect);
        cardEffects.Add("Advance 1 Space", AdvanceOneSpaceEffect);
        cardEffects.Add("Move Backward 1 Space", MoveBackwardOneSpaceEffect);
        cardEffects.Add("Tax Levy", TaxLevyEffect);
        cardEffects.Add("Avenue Demolition", AvenueDemolitionEffect);
        cardEffects.Add("Property Seizure", PropertySeizureEffect);
        cardEffects.Add("Natural Disaster", NaturalDisasterEffect);
        cardEffects.Add("Sales Slump", SalesSlump);
        cardEffects.Add("Generous Treat", GenerousTreatEffect);
        cardEffects.Add("Free Meal Ticket", FreeMealTicketEffect);
        cardEffects.Add("Firework Spectacle", FireworkSpectacleEffect);
    }
    private IEnumerator ApplyCardEffect(string cardName, PlayerController player)
    {
        switch (cardName)
        {
            // case "Birthday Gift":
            //     yield return BirthdayGiftEffect(player);
            //     break;
            // case "Lottery Win":
            //     yield return LotteryWinEffect(player);
            //     break;
            // case "Dog Poop Cleanup Fee":
            //     yield return DogPoopCleanupFeeEffect(player);
            //     break;
            // case "Get out of Jail Ticket":
            //     yield return GetOutOfJailEffect(player);
            //     break;
            // case "Go to Jail":
            //     yield return GoToJailEffect(player);
            //     break;
            // case "Advance to Go":
            //     yield return AdvanceToGoEffect(player);
            //     break;
            // case "Go Back to Go":
            //     yield return GoBackToGoEffect(player);
            //     break;
            // case "Advance 1 Space":
            //     yield return AdvanceOneSpaceEffect(player);
            //     break;
            // case "Move Backward 1 Space":
            //     yield return MoveBackwardOneSpaceEffect(player);
            //     break;
            // case "Tax Levy":
            //     yield return TaxLevyEffect(player);
            //     break;
            // case "Avenue Demolition":
            //     yield return AvenueDemolitionEffect(player);
            //     break;
            // case "Property Seizure":
            //     yield return PropertySeizureEffect(player);
            //     break; 
            // case "Natural Disaster":
            //     yield return NaturalDisasterEffect(player);
            //     break;
            case "Sales Slump":
                yield return SalesSlump(player);
                break;  
            // case "Generous Treat":
            //     yield return GenerousTreatEffect(player);
            //     break;
            // case "Free Meal Ticket":
            //     yield return FreeMealTicketEffect(player);
            //     break;  
            // case "Firework Spectacle":
            //     yield return FireworkSpectacleEffect(player);
            //     break;
            default:
                Debug.LogWarning("Card effect not found: " + cardName);
                break;
        }
        yield return new WaitForSecondsRealtime(1f);
    }
    // Define card effects
    private IEnumerator BirthdayGiftEffect(PlayerController currentPlayer)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        
        currentPlayer.Money += 15000 * (players.Length - 1);
        currentPlayer.UpdateMoneyText(); // Update UI to reflect the new money amount
        StartCoroutine(currentPlayer.ShowMessage("Collect a Birthday Gift of $15,000 from each player"));

        foreach (PlayerController player in players)
        {
            if (player != currentPlayer) // Exclude the current player
            {
                player.Money -= 15000; // Deduct $15 from the player
                player.UpdateMoneyText(); // Update UI to reflect the new money amount for the player
                StartCoroutine(player.ShowMessage("You gave $15,000 as a birthday gift"));
            }
        }
        yield return null;
    }

    private IEnumerator LotteryWinEffect(PlayerController player)
    {
        player.Money += 200000;
        player.UpdateMoneyText();
        StartCoroutine(player.ShowMessage("Congratulations! You have won a lottery prize of $200,000."));
        yield return null;
    }

    private IEnumerator DogPoopCleanupFeeEffect(PlayerController player)
    {
        player.Money -= 50000;
        player.UpdateMoneyText();
        StartCoroutine(player.ShowMessage("Oops! You have to pay a fee of $50,000 for dog poop cleanup."));
        yield return null;
    }

    private IEnumerator GetOutOfJailEffect(PlayerController player)
    {
        player.hasGetOutOfJailCard = true;
        StartCoroutine(player.ShowMessage("You got a Get out of jail Free Ticket to leave jail."));
        yield return null;
    }

    private IEnumerator GoToJailEffect(PlayerController player)
    {
        player.waypointIndex = 8;
        player.transform.position = player.waypoints[player.waypointIndex].position; 
        player.DisplayGoToJailText();
        player.InJail = true;
        StartCoroutine(player.ShowMessage("You have been sent to jail."));
        yield return null;
    }

    private IEnumerator AdvanceToGoEffect(PlayerController player)
    {
        int stepsToGo = (40 - player.currentPosition) % 40;
        int targetWaypointIndex = (player.currentPosition + stepsToGo) % 40;
        while (player.currentPosition != targetWaypointIndex)
        {
            player.MoveForward();
            yield return new WaitForSecondsRealtime(0.3f); 
        }
        player.Money += 300000;
        player.UpdateMoneyText();
        StartCoroutine(player.ShowMessage("Your character has moved forward to 'Go' and collected $300,000 from the bank."));
        yield return null;     
    }

    private IEnumerator GoBackToGoEffect(PlayerController player)
    {
        int stepsToGoBackward = player.currentPosition;

        for (int i = 0; i < stepsToGoBackward; i++)
        {

            player.MoveBackward();
            yield return new WaitForSecondsRealtime(0.3f);
        }
        StartCoroutine(player.ShowMessage("Your character has moved back to 'Go'"));
        yield return null;  
    }

    private IEnumerator AdvanceOneSpaceEffect(PlayerController player)
    {
        StartCoroutine(player.MovePlayerCoroutine(1));
        yield return StartCoroutine(player.WaitForPropertyDecision());      
        StartCoroutine(player.ShowMessage("Advance 1 space on the board."));
        yield return null;
    }

    private IEnumerator MoveBackwardOneSpaceEffect(PlayerController player)
    {
        yield return StartCoroutine(player.MovePlayerCoroutine(-1));
        yield return StartCoroutine(player.WaitForPropertyDecision());
        yield return null;
    }

    private IEnumerator TaxLevyEffect(PlayerController player)
    {
        int totalPropertyValue = 0;

        foreach (PropertyManager.PropertyData property in player.ownedProperties)
        {
            totalPropertyValue += property.stagePrices[property.currentStageIndex];
        }
        int taxAmount = (int)(totalPropertyValue * 0.1f);
        player.Money -= taxAmount;
        player.UpdateMoneyText();

        StartCoroutine(player.ShowMessage($"You paid a tax of ${taxAmount}"));
        yield return null;
    }
    private IEnumerator FreeMealTicketEffect(PlayerController currentPlayer)
    {
        currentPlayer.hasFreeRentTicket = true;
        StartCoroutine(currentPlayer.ShowMessage("You've received a Free Dinner Ticket. Your meal will be complimentary next time."));
        yield return null;
    }

    private IEnumerator AvenueDemolitionEffect(PlayerController currentPlayer)
    {
            gameManager.isCardEffect = true;

                foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                {
                    var tileScript = tile.GetComponent<TileScript>();
                    if (tileScript != null)
                    {
                        tileScript.enabled = false;
                    }
                }                
                currentPlayer.ListPropertiesForEffect = new List<PropertyManager.PropertyData>();
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                foreach (PlayerController player in players)
                {
                    if (player.teamID != currentPlayer.teamID)  // Exclude the current player
                    {
                        currentPlayer.ListPropertiesForEffect.AddRange(player.ownedProperties);
                    }
                }
                if (currentPlayer.ListPropertiesForEffect.Count > 0)
                {
                    gameManager.selectedProperty = null;
                    StartCoroutine(currentPlayer.ShowMessage("Select a property to demolish:"));
                    yield return new WaitForSecondsRealtime(2f);
                    foreach (var opponentProperty in currentPlayer.ListPropertiesForEffect)
                    {
                        GameObject tileImage = gameManager.waypointIndexToTileMap[opponentProperty.JSONwaypointIndex];
    
                        tileImage.transform.position += new Vector3(0, 1, 0);
                        gameManager.AssignPropertyToTile(tileImage, opponentProperty);
                        TileClickHandler clickHandler = tileImage.GetComponent<TileClickHandler>();
                        if (clickHandler == null)
                        {
                            clickHandler = tileImage.AddComponent<TileClickHandler>();
                        }
                        clickHandler.SetAssociatedProperty(opponentProperty);                       
                    }                 
                    yield return currentPlayer.WaitForPlayerSelection();
                    currentPlayer.propertyToSeize = gameManager.selectedProperty;
                    Debug.Log("Selected Property: " + gameManager.selectedProperty.name);
                    Debug.Log("Selected property to demolish: " + currentPlayer.propertyToSeize.name);
                    if (currentPlayer.propertyToSeize != null)
                    {
                        PlayerController ownerPlayer = currentPlayer.FindPlayerByID(currentPlayer.propertyToSeize.ownerID);
                        if (ownerPlayer == null)
                        {
                            Debug.LogError("Owner player not found for property: " + currentPlayer.propertyToSeize.name);
                            yield break; // Exit if owner player is not found
                        }                       
                        ownerPlayer.ownedProperties.Remove(currentPlayer.propertyToSeize);
                        // Reset the property ownership
                        currentPlayer.propertyToSeize.owned = false;
                        currentPlayer.propertyToSeize.ownerID = 0; // Set ownerID to -1 (ownerless)
                        currentPlayer.propertyToSeize.teamownerID = 0; // Set teamownerID to -1 (ownerless)
                        currentPlayer.propertyToSeize.currentStageIndex = -1;
                        propertyManager.DeactivateOldStageImages(currentPlayer.propertyToSeize);
                        propertyManager.DeactivateRentTagImage(currentPlayer.propertyToSeize);
                        currentPlayer.propertyToSeize.rentText.gameObject.SetActive(false);

                        StartCoroutine(currentPlayer.ShowMessage($"You demolished {currentPlayer.propertyToSeize.name}, leaving it ownerless."));

                       
                        // Additional actions can be added here...
                    }
                    foreach (var opponentProperty in currentPlayer.ListPropertiesForEffect)
                    {
                        GameObject tileImage = gameManager.waypointIndexToTileMap[opponentProperty.JSONwaypointIndex];
                        tileImage.transform.position += new Vector3(0, -1, 0);
                    }                   
                }
                else
                {
                    // If no opponent-owned properties are available for demolition, show a message
                    StartCoroutine(currentPlayer.ShowMessage("There are no opponent-owned properties available for demolition."));
                }
                currentPlayer.ListPropertiesForEffect.Clear();
                currentPlayer.propertyToSeize = null;
                foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                {
                    var tileScript = tile.GetComponent<TileScript>();
                    if (tileScript != null)
                    {
                        tileScript.enabled = true;
                    }
                }
                gameManager.isCardEffect = false;
                yield return null;
    }

    private IEnumerator PropertySeizureEffect(PlayerController currentPlayer)
    {
                gameManager.isCardEffect = true;
                foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                {
                    var tileScript = tile.GetComponent<TileScript>();
                    if (tileScript != null)
                    {
                        tileScript.enabled = false;
                    }
                } 
                currentPlayer.ListPropertiesForEffect = new List<PropertyManager.PropertyData>();
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                foreach (PlayerController player in players)
                {
                    if (player.teamID != currentPlayer.teamID)  // Exclude the current player
                    {
                        currentPlayer.ListPropertiesForEffect.AddRange(player.ownedProperties);
                    }
                }
                // Check if there are opponent-owned properties available for seizure
                if (currentPlayer.ListPropertiesForEffect.Count > 0)
                {
                    gameManager.selectedProperty = null;
                    StartCoroutine(currentPlayer.ShowMessage("Select a property to seize:"));
                    yield return new WaitForSecondsRealtime(2f);
                    foreach (var opponentProperty in currentPlayer.ListPropertiesForEffect)
                    {
                        GameObject tileImage = gameManager.waypointIndexToTileMap[opponentProperty.JSONwaypointIndex];

                        tileImage.transform.position += new Vector3(0, 1, 0);
                        gameManager.AssignPropertyToTile(tileImage, opponentProperty);
                        TileClickHandler clickHandler = tileImage.GetComponent<TileClickHandler>();
                        if (clickHandler == null)
                        {
                            clickHandler = tileImage.AddComponent<TileClickHandler>();
                        }
                        clickHandler.SetAssociatedProperty(opponentProperty);
                    }
                    yield return currentPlayer.WaitForPlayerSelection();
                    currentPlayer.propertyToSeize = gameManager.selectedProperty;
                    Debug.Log("Selected Property: " + gameManager.selectedProperty.name);
                    Debug.Log("Selected property to seize: " + currentPlayer.propertyToSeize.name);
                    if (currentPlayer.propertyToSeize != null)
                    {
                        PlayerController ownerPlayer = currentPlayer.FindPlayerByID(currentPlayer.propertyToSeize.ownerID);
                        if (ownerPlayer == null)
                        {
                            Debug.LogError("Owner player not found for property: " + currentPlayer.propertyToSeize.name);
                            yield break; // Exit if owner player is not found
                        }
                        int compensationAmount = currentPlayer.propertyToSeize.stagePrices[currentPlayer.propertyToSeize.currentStageIndex];
                        ownerPlayer.Money += compensationAmount;
                        StartCoroutine(ownerPlayer.ShowMessage($"Your property {currentPlayer.propertyToSeize.name} has been seized. You received $ {compensationAmount} as compensation."));

                        ownerPlayer.ownedProperties.Remove(currentPlayer.propertyToSeize);
                        // Transfer the property ownership to the current player 
                        currentPlayer.propertyToSeize.owned = false;
                        currentPlayer.propertyToSeize.ownerID = 0;
                        currentPlayer.propertyToSeize.teamownerID = 0;
                        currentPlayer.propertyToSeize.currentStageIndex = -1;

                        propertyManager.DeactivateOldStageImages(currentPlayer.propertyToSeize);
                        propertyManager.DeactivateRentTagImage(currentPlayer.propertyToSeize);
                        currentPlayer.propertyToSeize.rentText.gameObject.SetActive(false);

                        StartCoroutine(currentPlayer.ShowMessage($"You seize {currentPlayer.propertyToSeize.name} of your opponent, leaving it ownerless."));
                    }
                    foreach (var opponentProperty in currentPlayer.ListPropertiesForEffect)
                    {
                        GameObject tileImage = gameManager.waypointIndexToTileMap[opponentProperty.JSONwaypointIndex];
                        tileImage.transform.position += new Vector3(0, -1, 0);
                    }
                }
                else
                {
                    StartCoroutine(currentPlayer.ShowMessage("There are no opponent-owned properties available for seizure."));
                }
                currentPlayer.ListPropertiesForEffect.Clear();
                currentPlayer.propertyToSeize = null;
                foreach (var tile in gameManager.waypointIndexToTileMap.Values)
                {
                    var tileScript = tile.GetComponent<TileScript>();
                    if (tileScript != null)
                    {
                        tileScript.enabled = true;
                    }
                }
                gameManager.isCardEffect = false;
                yield return null;
    }

    private IEnumerator NaturalDisasterEffect(PlayerController currentPlayer)
    {
                currentPlayer.propertyToSeize = null;
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                foreach (PlayerController player in players)
                {
                    if (player == currentPlayer) // Only affect the current player
                    {
                        if (currentPlayer.ownedProperties.Count > 0)
                        {                        
                            currentPlayer.propertyToSeize = currentPlayer.ownedProperties[Random.Range(0, currentPlayer.ListPropertiesForEffect.Count)];
                        // currentPlayer.propertyToSeize = player.RemoveRandomProperty();
                        // if (currentPlayer.propertyToSeize != null)
                        // {
                            // Reset the property ownership
                            currentPlayer.propertyToSeize.owned = false;
                            currentPlayer.propertyToSeize.ownerID = 0; // Set ownerID to 0 (ownerless)
                            currentPlayer.propertyToSeize.teamownerID = 0; // Set teamownerID to 0 (ownerless)
                            currentPlayer.propertyToSeize.currentStageIndex = -1;
                            propertyManager.DeactivateOldStageImages(currentPlayer.propertyToSeize);
                            propertyManager.DeactivateRentTagImage(currentPlayer.propertyToSeize);
                            currentPlayer.propertyToSeize.rentText.gameObject.SetActive(false);

                            StartCoroutine(currentPlayer.ShowMessage($"An earthquake has destroyed your property: {currentPlayer.propertyToSeize.name}"));
                            currentPlayer.ownedProperties.Remove(currentPlayer.propertyToSeize);
                            yield return null; // Wait to show the message
                        }
                        else
                        {
                            StartCoroutine(currentPlayer.ShowMessage("You have no properties to destroy."));
                        }
                    }
                }
                currentPlayer.propertyToSeize = null;
                yield return null;
    }

    private IEnumerator SalesSlump(PlayerController currentPlayer)
    {
        gameManager.isCardEffect = true;
        foreach (var tile in gameManager.waypointIndexToTileMap.Values)
        {
            var tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null)
            {
                tileScript.enabled = false;
            }
        }
        currentPlayer.ListPropertiesForEffect = new List<PropertyManager.PropertyData>();
        currentPlayer.ListPropertiesForEffect.AddRange(currentPlayer.ownedProperties);

        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            gameManager.selectedProperty = null;
            StartCoroutine(currentPlayer.ShowMessage("Select a property to sell:"));
            yield return new WaitForSecondsRealtime(2f);

            // Show player's properties for selection
            foreach (var playerProperty in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[playerProperty.JSONwaypointIndex];

                tileImage.transform.position += new Vector3(0, 1, 0);

                // Assign click handler for selecting the property
                TileClickHandler clickHandler = tileImage.GetComponent<TileClickHandler>();
                if (clickHandler == null)
                {
                    clickHandler = tileImage.AddComponent<TileClickHandler>();
                }
                clickHandler.SetAssociatedProperty(playerProperty);
            }

            // Wait for player selection
            yield return currentPlayer.WaitForPlayerSelection();
            currentPlayer.propertyToSeize = gameManager.selectedProperty;

            if (currentPlayer.propertyToSeize != null)
            {
                int compensationAmount = currentPlayer.propertyToSeize.stagePrices[currentPlayer.propertyToSeize.currentStageIndex];
                currentPlayer.Money += compensationAmount;
                currentPlayer.ownedProperties.Remove(currentPlayer.propertyToSeize);
                
                currentPlayer.propertyToSeize.owned = false;
                currentPlayer.propertyToSeize.ownerID = 0;
                currentPlayer.propertyToSeize.teamownerID = 0;
                currentPlayer.propertyToSeize.currentStageIndex = -1;

                // Deactivate property images and UI elements
                propertyManager.DeactivateOldStageImages(currentPlayer.propertyToSeize);
                propertyManager.DeactivateRentTagImage(currentPlayer.propertyToSeize);
                currentPlayer.propertyToSeize.rentText.gameObject.SetActive(false);
                StartCoroutine(currentPlayer.ShowMessage($"You sold {currentPlayer.propertyToSeize.name} for $ {compensationAmount}."));
            
            }
            foreach (var opponentProperty in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[opponentProperty.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }           
        }
        else
        {
            // Inform the player if they don't own any properties to sell
            StartCoroutine(currentPlayer.ShowMessage("You don't own any properties to sell."));
        }

        // Reset variables and enable tile scripts
        currentPlayer.ListPropertiesForEffect.Clear();
        currentPlayer.propertyToSeize = null;
        foreach (var tile in gameManager.waypointIndexToTileMap.Values)
        {
            var tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null)
            {
                tileScript.enabled = true;
            }
        }
        gameManager.isCardEffect = false;
        yield return null;
    }


    private IEnumerator GenerousTreatEffect(PlayerController currentPlayer)
    {
        // player.ProvideFreeMeal();
 
        yield return null;
    }

    private IEnumerator FireworkSpectacleEffect(PlayerController player)
    {
        // player.IncreaseStallValue();
        StartCoroutine(player.ShowMessage("Select one of your stalls to host a firework display, turning it into a hot spot and increasing its value."));
        yield return null;
    }

    public IEnumerator DrawAndDisplayCard(PlayerController player)
    {
        Card drawnCard = DrawRandomCard();
        StartCoroutine(ShowCardObject());
        if (cardName != null && cardDescription != null)
        {
            cardName.text = drawnCard.name;
            cardDescription.text = drawnCard.description;
        }
        else
        {
            Debug.LogError("Description text component not found on card prefab.");
        }
        yield return new WaitForSecondsRealtime(3f);
        yield return ApplyCardEffect(drawnCard.name, player);
    }

    private IEnumerator ShowCardObject()
    {
        CardObject.gameObject.SetActive(true);
        yield return StartCoroutine(gameManager.HideObject(CardObject));
    }

    private Card DrawRandomCard()
    {
        List<Card> cards = new List<Card>(cardDeck.Values);
        int randomIndex = Random.Range(0, cards.Count);
        return cards[randomIndex];
    }



}
