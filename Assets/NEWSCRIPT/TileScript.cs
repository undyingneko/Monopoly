using UnityEngine;

public class TileScript : MonoBehaviour
{
    public PlayerController associatedPlayer;

    // Handle mouse clicks on the tile
    private void OnMouseDown()
    {
        GameManager.Instance.HandleTileClick(this);
    }
}
