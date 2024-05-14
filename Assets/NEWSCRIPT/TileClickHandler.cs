using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isAvenueDemolitionActive)
        {
            var property = GameManager.Instance.GetPropertyFromTile(gameObject);
            Debug.Log("Property from GameManager: " + (property != null ? property.name : "null"));

            if (property == null)
            {
                Debug.LogError("Associated property is null!");
                return;
            }

            Debug.Log("Tile clicked for demolition: " + property.name);
            GameManager.Instance.selectedProperty = property;

            // Additional debug log
            Debug.Log("GameManager selected property: " + (GameManager.Instance.selectedProperty != null ? GameManager.Instance.selectedProperty.name : "null"));
        }
        else
        {
            Debug.Log("Tile clicked outside of demolition mode.");
        }
    }
}
