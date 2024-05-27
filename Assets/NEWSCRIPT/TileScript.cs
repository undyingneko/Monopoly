using UnityEngine;
using UnityEngine.UI; 

public class TileScript : MonoBehaviour
{
    private GameManager gameManager;

    private StallManager stallManager;
    private OnsenManager onsenManager;

    private void Start()
    {
        stallManager = StallManager.Instance;
        onsenManager = OnsenManager.Instance;
        gameManager = FindObjectOfType<GameManager>();
    }
    // Handle mouse clicks on the tile
    public void HandleTileClick(PlayerController clickingPlayer)
    {
        clickingPlayer.CloseActivePopup();
        if (gameManager.isCardEffect || gameManager.isSelling)
        {
            return;
        }

        BuyPropertyPopup012 buyPopup = FindObjectOfType<BuyPropertyPopup012>();
        if (buyPopup != null && buyPopup.isActiveAndEnabled)
        {
            return;
        }

        BuyOutPopUp buyoutPopup = FindObjectOfType<BuyOutPopUp>();
        if (buyoutPopup != null && buyoutPopup.isActiveAndEnabled)
        {
            return;
        }

        if (gameManager.isCardEffect || gameManager.isSelling)
        {
            return;
        }
        
        // clickingPlayer.tilePopup.SetActive(true);
        if (clickingPlayer.tilePopup != null)
        {
            clickingPlayer.tilePopup.SetActive(true);
            
            clickingPlayer.Tilepopup_closeButton.onClick.AddListener(clickingPlayer.CloseActivePopup);
            
            // InitializePopupComponents();
            int Tilepopup_waypointIndex = clickingPlayer.GetWaypointIndexFromName(gameObject.name);

            StallManager.StallData stallData = stallManager.GetStallByWaypointIndex(Tilepopup_waypointIndex);
            OnsenManager.OnsenData onsenData = onsenManager.GetOnsenByWaypointIndex(Tilepopup_waypointIndex);


            if (stallData != null)
            {
                clickingPlayer.UpdatePopupContentStall(stallData);
            }
            else if (onsenData != null)
            {
                clickingPlayer.UpdatePopupContentOnsen(onsenData);
            }
            else
            {
                Debug.LogWarning("No property found for this tile.");
            } 
        }
    }
}
