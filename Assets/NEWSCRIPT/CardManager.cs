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
    private StallManager stallManager;

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
            gameManager = FindObjectOfType<GameManager>();
            stallManager = StallManager.Instance;         
            InitializeCardDeck();
            InitializeCardEffects(); 
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate CardManager instances
        }
    }

    private void InitializeCardDeck()
    {
        cardDeck = new Dictionary<string, Card>();

        // // Populate the card deck
        // cardDeck.Add("Birthday Gift", new Card("Birthday Gift", "Collect a Birthday Gift of $15000 from each player."));
        // cardDeck.Add("Lottery Win: $200,000", new Card("Lottery Win", "Congratulations! You have won a lottery prize of $200,000."));
        // cardDeck.Add("Trash Fine", new Card("Trash Fine", "Oops! You have been caught deliberately throwing trash. Pay a fine of $50,000."));
        // cardDeck.Add("Get out of Jail Ticket", new Card("Get out of Jail Ticket", "You can use this card to get out of jail once."));
        // cardDeck.Add("Go to Jail", new Card("Go to Jail", "Go directly to Jail. Do not pass 'Go,' do not collect $300,000"));
        // cardDeck.Add("Advance to Go", new Card("Advance to Go", "Move your character to the \"Go\" space on the board and collect $300,000 from the bank."));
        // cardDeck.Add("Go Back to Go", new Card("Go Back to Go", "Go back to \"Go\" without passing 'Go,' without collecting $300,000"));
        // cardDeck.Add("Advance 1 Space", new Card("Advance 1 Space", "Advance 1 space on the board."));
        // cardDeck.Add("Move Backward 1 Space", new Card("Move Backward 1 Space", "Move your character back one space on the board."));
        // cardDeck.Add("Tax Levy", new Card("Tax Levy", "Pay a tax equal to 10% of the total value of your owned properties."));
        // cardDeck.Add("Vip Experience Ticket", new Card("Vip Experience Ticket", "Receive a ticket for a complimentary meal at any food stall on your next visit."));
        cardDeck.Add("Avenue Demolition", new Card("Avenue Demolition", "Demolish one of the opponent's avenues, leaving it ownerless."));
        // cardDeck.Add("Property Seizure", new Card("Property Seizure", "Force one opponent to sell one property of your choice from their holdings."));
        // cardDeck.Add("Natural Disaster", new Card("Natural Disaster", "An earthquake has destroyed 1 of your food stalls at the festival."));
        // cardDeck.Add("Sales Slump", new Card("Sales Slump", "You're Forced to Sell One Food Stall to Keep Your Business Afloat"));
        // cardDeck.Add("Generous Welcome Event", new Card("Generous Welcome Event", "Select one food stall of the opponent. Any player landing on this stall is treated to a complimentary meal for one turn, no payment necessary."));
        // cardDeck.Add("Firework Spectacle", new Card("Firework Spectacle", "Select one of your stalls to host a firework display, turning it into a hot spot and increasing its value."));
    }

    private void InitializeCardEffects()
    {
        cardEffects = new Dictionary<string, System.Func<PlayerController, IEnumerator>>();

        // Define card effects
        cardEffects.Add("Birthday Gift", BirthdayGiftEffect);
        cardEffects.Add("Lottery Win: $200,000", LotteryWinEffect);
        cardEffects.Add("Trash Fine", DogPoopCleanupFeeEffect);
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
        cardEffects.Add("Generous Welcome Event", GenerousTreatEffect);
        cardEffects.Add("Vip Experience Ticket", VipTicket);
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
            // case "Trash Fine":
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
            // case "Vip Experience Ticket":
            //     yield return VipTicket(player);
            //     break;  
            // case "Generous Welcome Event":
            //     yield return GenerousTreatEffect(player);
            //     break;
            case "Avenue Demolition":
                yield return AvenueDemolitionEffect(player);
                break;
            // case "Property Seizure":
            //     yield return PropertySeizureEffect(player);
            //     break; 
            // case "Natural Disaster":
            //     yield return NaturalDisasterEffect(player);
            //     break;
            // case "Sales Slump":
            //     yield return SalesSlump(player);
            //     break;  
            // case "Firework Spectacle":
            //     yield return FireworkSpectacleEffect(player);
            //     break;
            default:
                Debug.LogWarning("Card effect not found: " + cardName);
                break;
        }
        // yield return new WaitForSecondsRealtime(1f);
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
                yield return StartCoroutine(player.ShowMessage("You gave $15,000 as a birthday gift"));
            }
        }
        yield return null;
    }

    private IEnumerator LotteryWinEffect(PlayerController player)
    {
        player.Money += 200000;
        player.UpdateMoneyText();
        yield return StartCoroutine(player.ShowMessage("Congratulations! You have won a lottery prize of $200,000."));
        yield return null;
    }

    private IEnumerator DogPoopCleanupFeeEffect(PlayerController player)
    {
        player.Money -= 50000;
        player.UpdateMoneyText();
        yield return StartCoroutine(player.ShowMessage("Oops! You have to pay a fee of $50,000 for dog poop cleanup."));
        yield return null;
    }

    private IEnumerator GetOutOfJailEffect(PlayerController player)
    {
        player.hasGetOutOfJailCard = true;
        yield return StartCoroutine(player.ShowMessage("You got a Get out of jail Free Ticket to leave jail."));
        yield return null;
    }

    private IEnumerator GoToJailEffect(PlayerController player)
    {
        player.waypointIndex = 8;
        player.transform.position = player.waypoints[player.waypointIndex].position; 
        player.DisplayGoToJailText();
        player.InJail = true;
        yield return StartCoroutine(player.ShowMessage("You have been sent to jail."));
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
        yield return StartCoroutine(player.ShowMessage("Your character has moved forward to 'Go' and collected $300,000 from the bank."));
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
        yield return StartCoroutine(player.ShowMessage("Your character has moved back to 'Go'"));
        yield return null;  
    }

    private IEnumerator AdvanceOneSpaceEffect(PlayerController player)
    {
        StartCoroutine(player.MovePlayerCoroutine(1));
        yield return StartCoroutine(player.WaitForPropertyDecision());      
        yield return StartCoroutine(player.ShowMessage("Advance 1 space on the board."));
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

        foreach (StallManager.StallData property in player.ownedStalls)
        {
            totalPropertyValue += property.stagePrices[property.currentStageIndex];
        }
        int taxAmount = (int)(totalPropertyValue * 0.1f);
        player.Money -= taxAmount;
        player.UpdateMoneyText();

        yield return StartCoroutine(player.ShowMessage($"You paid a tax of ${taxAmount}"));
        yield return null;
    }

    private IEnumerator GenerousTreatEffect(PlayerController currentPlayer)
    {
        gameManager.isCardEffect = true;
        BeforeSelection(currentPlayer);

        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            yield return StartCoroutine(Selecting(currentPlayer));

            if (currentPlayer.propertyToBeEffected != null)
            {
                currentPlayer.propertyToBeEffected.isWelcomeEvent = true; //EFFECT
                yield return StartCoroutine(currentPlayer.ShowMessage($"The food stall {currentPlayer.propertyToBeEffected.name} will treat any player to a complimentary meal for one turn."));
            }
            foreach (var propety in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }
        }
        else
        {
            StartCoroutine(currentPlayer.ShowMessage("There are no opponent-owned property available for the effect."));
        }

        AfterSelection(currentPlayer);
        yield return null;
    }

    private IEnumerator AvenueDemolitionEffect(PlayerController currentPlayer)
    {
        gameManager.isCardEffect = true;
        BeforeSelection(currentPlayer);//-------------
        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            yield return StartCoroutine(Selecting(currentPlayer));//---------
            if (currentPlayer.propertyToBeEffected != null)
            {
                Destroy(currentPlayer);
                yield return StartCoroutine(currentPlayer.ShowMessage($"You demolished {currentPlayer.propertyToBeEffected.name}, leaving it ownerless."));
            }
            foreach (var propety in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }                   
        }
        else
        {
            StartCoroutine(currentPlayer.ShowMessage("There are no opponent-owned properties available for demolition."));
        }
        AfterSelection(currentPlayer);
    }
    private IEnumerator NaturalDisasterEffect(PlayerController currentPlayer)
    {
        gameManager.isCardEffect = true;
        currentPlayer.ListPropertiesForEffect.Clear();
        currentPlayer.propertyToBeEffected = null;
        foreach (var stalltosell in currentPlayer.ownedStalls)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { stallData = stalltosell });
        }
        foreach (var onsentosell in currentPlayer.ownedOnsens)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { onsenData = onsentosell });
        }

        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {  
            currentPlayer.propertyToBeEffected = currentPlayer.ListPropertiesForEffect[Random.Range(0, currentPlayer.ListPropertiesForEffect.Count)];
            Destroy(currentPlayer);
            yield return StartCoroutine(currentPlayer.ShowMessage($"An earthquake has destroyed your {currentPlayer.propertyToBeEffected.name}"));
        }
        else
        {
            yield return StartCoroutine(currentPlayer.ShowMessage("You have no properties to destroy."));
        }
            
        
        gameManager.isCardEffect = false;
        yield return null;
    }
    private IEnumerator PropertySeizureEffect(PlayerController currentPlayer)
    {
        gameManager.isCardEffect = true;
        BeforeSelection(currentPlayer);
        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            yield return StartCoroutine(Selecting(currentPlayer));//---------
            if (currentPlayer.propertyToBeEffected != null)
            {
                Destroy(currentPlayer);
                int compensationAmount = currentPlayer.propertyToBeEffected.stagePrices[currentPlayer.propertyToBeEffected.currentStageIndex];
                currentPlayer.Money += compensationAmount;
                currentPlayer.UpdateMoneyText();
                yield return StartCoroutine(currentPlayer.ShowMessage($"You seize {currentPlayer.propertyToBeEffected.name} of your opponent, leaving it ownerless.")); 
            }
            foreach (var propety in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }
        }
        else
        {
            yield return StartCoroutine(currentPlayer.ShowMessage("There are no opponent-owned properties available for seizure."));
        }
        AfterSelection(currentPlayer);
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
        currentPlayer.ListPropertiesForEffect.Clear();
        currentPlayer.propertyToBeEffected = null;
        foreach (var stalltosell in currentPlayer.ownedStalls)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { stallData = stalltosell });
        }
        foreach (var onsentosell in currentPlayer.ownedOnsens)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { onsenData = onsentosell });
        }
        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            yield return StartCoroutine(Selecting(currentPlayer));//---------
            if (currentPlayer.propertyToBeEffected != null)
            {
                Destroy(currentPlayer);
                int compensationAmount = currentPlayer.propertyToBeEffected.stagePrices[currentPlayer.propertyToBeEffected.currentStageIndex];
                currentPlayer.Money += compensationAmount;
                currentPlayer.UpdateMoneyText();
                StartCoroutine(currentPlayer.ShowMessage($"You have sold {currentPlayer.propertyToBeEffected.name} for $ {compensationAmount}."));
            }
            foreach (var propety in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }           
        }
        else
        {
            // Inform the player if they don't own any properties to sell
            StartCoroutine(currentPlayer.ShowMessage("You don't own any properties to sell."));
        }
        AfterSelection(currentPlayer);
    }

    private IEnumerator VipTicket(PlayerController currentPlayer)
    {
        currentPlayer.hasVipExperienceTicket = true;
        StartCoroutine(currentPlayer.ShowMessage("You've received a Free Dinner Ticket. Your meal will be complimentary next time."));
        yield return null;
    }

    private IEnumerator FireworkSpectacleEffect(PlayerController currentPlayer)
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
        currentPlayer.ListPropertiesForEffect.Clear();
        currentPlayer.propertyToBeEffected = null;
        foreach (var stalltosell in currentPlayer.ownedStalls)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { stallData = stalltosell });
        }
        foreach (var onsentosell in currentPlayer.ownedOnsens)
        {
            currentPlayer.ListPropertiesForEffect.Add(new Properties { onsenData = onsentosell });
        }
        if (currentPlayer.ListPropertiesForEffect.Count > 0)
        {
            yield return StartCoroutine(currentPlayer.ShowMessage("Select a property to host a firework display:"));
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
            yield return new WaitUntil(() => gameManager.ChanceSelectionMade);
            // Debug.Log("this part");

            currentPlayer.propertyToBeEffected = gameManager.selectedProperty;
            if (currentPlayer.propertyToBeEffected != null )
            {
                if (gameManager.FireworkPlaceIsSet && gameManager.currentHostingFireWork.teamownerID != currentPlayer.teamID)
                {
                    var previousHotspotProperty = gameManager.currentHostingFireWork;
                    // Debug.Log("Previous hotspot:" + previousHotspotProperty.name);
                    previousHotspotProperty.isFireWork = false;
                    if (previousHotspotProperty.stallData != null)
                    {
                        StallManager.StallData stalltoapply = previousHotspotProperty.stallData;
                        stalltoapply.InitializePrices();
                    }
                    if (previousHotspotProperty.onsenData != null)
                    {
                        OnsenManager.OnsenData onsentoapply = previousHotspotProperty.onsenData;
                        onsentoapply.InitializePrices();
                    }
                    // previousHotspotProperty.InitializePrices();

                    // Debug.Log("Old rent price" + previousHotspotProperty.rentPrices[previousHotspotProperty.currentStageIndex]);
                    // stallManager.InitializeRentText(previousHotspotProperty);
                    UpdateRentText(previousHotspotProperty, previousHotspotProperty.currentStageIndex);
                }          
                currentPlayer.propertyToBeEffected.isFireWork = true;
                gameManager.FireworkPlaceIsSet = true;
                float multiplier = 1.2f; 
                for (int i = 0; i < currentPlayer.propertyToBeEffected.rentPrices.Count; i++)
                {
                    currentPlayer.propertyToBeEffected.rentPrices[i] = (int)(currentPlayer.propertyToBeEffected.rentPrices[i] * multiplier);
                }
                for (int i = 0; i < currentPlayer.propertyToBeEffected.stagePrices.Count; i++)
                {
                    currentPlayer.propertyToBeEffected.stagePrices[i] = (int)(currentPlayer.propertyToBeEffected.stagePrices[i] * multiplier);
                }
                for (int i = 0; i < currentPlayer.propertyToBeEffected.buyoutPrices.Count; i++)
                {
                    currentPlayer.propertyToBeEffected.buyoutPrices[i] = (int)(currentPlayer.propertyToBeEffected.buyoutPrices[i] * multiplier);
                }
                // stallManager.InitializeRentText(currentPlayer.propertyToBeEffected);
                UpdateRentText(currentPlayer.propertyToBeEffected, currentPlayer.propertyToBeEffected.currentStageIndex);
                gameManager.currentHostingFireWork = currentPlayer.propertyToBeEffected;
                // Debug.Log("Current hotspot:" + gameManager.currentHostingFireWork.name);
                StartCoroutine(currentPlayer.ShowMessage(currentPlayer.propertyToBeEffected.name + " is now hosting a firework display, becoming a hot spot!"));
            
            }
            foreach (var propety in currentPlayer.ListPropertiesForEffect)
            {
                GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
                tileImage.transform.position += new Vector3(0, -1, 0);
            }     
        }
        else
        {
            StartCoroutine(currentPlayer.ShowMessage("You don't own any properties to host a firework display."));
        }
        gameManager.ChanceSelectionMade = false;
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




//------------------------------
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
        // yield return new WaitForSecondsRealtime(1f);
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

    private void BeforeSelection(PlayerController currentPlayer)
    {
        foreach (var tile in gameManager.waypointIndexToTileMap.Values)
        {
            var tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null)
            {
                tileScript.enabled = false;
            }
        }                
        currentPlayer.ListPropertiesForEffect.Clear();
        currentPlayer.propertyToBeEffected = null;
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {          
            if (player.teamID != currentPlayer.teamID)  // Exclude the current player
            {
                foreach (var stalltosell in player.ownedStalls)
                {
                    currentPlayer.ListPropertiesForEffect.Add(new Properties { stallData = stalltosell });
                }
                foreach (var onsentosell in player.ownedOnsens)
                {
                    currentPlayer.ListPropertiesForEffect.Add(new Properties { onsenData = onsentosell });
                }
                // currentPlayer.ListPropertiesForEffect.AddRange(player.ownedStalls);
            }
        }
    }

    private IEnumerator Selecting(PlayerController currentPlayer)
    {
        yield return StartCoroutine(currentPlayer.ShowMessage("Select an opponent's property to apply the effect"));
        foreach (var propety in currentPlayer.ListPropertiesForEffect)
        {
            GameObject tileImage = gameManager.waypointIndexToTileMap[propety.JSONwaypointIndex];
            tileImage.transform.position += new Vector3(0, 1, 0);
            gameManager.AssignPropertyToTile(tileImage, propety);
            TileClickHandler clickHandler = tileImage.GetComponent<TileClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = tileImage.AddComponent<TileClickHandler>();
            }
            clickHandler.SetAssociatedProperty(propety);
        }

        yield return new WaitUntil(() => gameManager.ChanceSelectionMade);
        currentPlayer.propertyToBeEffected = gameManager.selectedProperty;
        // Debug.Log("Selected property " + currentPlayer.propertyToBeEffected.name);
    }

    private void Destroy(PlayerController currentPlayer)
    {
        PlayerController ownerPlayer = currentPlayer.FindPlayerByID(currentPlayer.propertyToBeEffected.ownerID);
        if (ownerPlayer == null)
        {
            Debug.LogError("Owner player not found for property: " + currentPlayer.propertyToBeEffected.name);
            return; // Exit if owner player is not found
        }
        if (currentPlayer.propertyToBeEffected.stallData != null)
        {
            StallManager.StallData stalltoapply = currentPlayer.propertyToBeEffected.stallData;
            ownerPlayer.ownedStalls.Remove(stalltoapply);
        }
        if (currentPlayer.propertyToBeEffected.onsenData != null)
        {
            OnsenManager.OnsenData onsentoapply = currentPlayer.propertyToBeEffected.onsenData;
            ownerPlayer.ownedOnsens.Remove(onsentoapply);
        }

        currentPlayer.propertyToBeEffected.owned = false;
        currentPlayer.propertyToBeEffected.ownerID = 0; // Set ownerID to -1 (ownerless)
        currentPlayer.propertyToBeEffected.teamownerID = 0; // Set teamownerID to -1 (ownerless)
        currentPlayer.propertyToBeEffected.currentStageIndex = -1;
        currentPlayer.propertyToBeEffected.isFireWork = false;
        currentPlayer.propertyToBeEffected.isWelcomeEvent = false;
        if (gameManager.currentHostingFireWork == currentPlayer.propertyToBeEffected)
        {
            gameManager.currentHostingFireWork = null;
            gameManager.FireworkPlaceIsSet = false;
        }

        foreach (GameObject stageImage in currentPlayer.propertyToBeEffected.StageImages)
        {
            stageImage.SetActive(false);
        }
        foreach (GameObject rentTagImage in currentPlayer.propertyToBeEffected.RentTagImages)
        {
            rentTagImage.SetActive(false);
        }
        currentPlayer.propertyToBeEffected.rentText.gameObject.SetActive(false);
    }

    private void AfterSelection(PlayerController currentPlayer)
    {
        gameManager.ChanceSelectionMade = false;
        foreach (var tile in gameManager.waypointIndexToTileMap.Values)
        {
            var tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null)
            {
                tileScript.enabled = true;
            }
        }
        gameManager.isCardEffect = false;
    }

    private void UpdateRentText(Properties propertyToBeEffected, int stageIndex)
    {
        // Debug.Log("Updating rent text for stall: " + propertyToBeEffected.name + " at stage index: " + stageIndex);
        // Debug.Log(System.Environment.StackTrace);
        if (propertyToBeEffected.rentText != null)
        {
            // Debug.Log("Rent text is assigned for stall: " + propertyToBeEffected.name);
            propertyToBeEffected.rentText.text = StallManager.Instance.FormatPrice(propertyToBeEffected.rentPrices[stageIndex]);
            propertyToBeEffected.rentText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rent text not found for stall: " + propertyToBeEffected.name);
        }
    }

}
