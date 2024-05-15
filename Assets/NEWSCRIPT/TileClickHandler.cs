using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private PropertyManager.PropertyData associatedProperty;

    private void OnMouseDown()
    {
        if (GameManager.Instance.isCardEffect)
        {
            if (associatedProperty == null)
            {
                Debug.LogError("Associated property is null!");
                return;
            }

            Debug.Log("Tile clicked for demolition: " + associatedProperty.name);
            GameManager.Instance.selectedProperty = associatedProperty;

            // Additional debug log
            Debug.Log("GameManager selected property: " + (GameManager.Instance.selectedProperty != null ? GameManager.Instance.selectedProperty.name : "null"));
        }
        else
        {
            Debug.Log("Tile clicked outside of demolition mode.");
        }
    }

    public void SetAssociatedProperty(PropertyManager.PropertyData property)
    {
        associatedProperty = property;
        Debug.Log("Associated property set: " + property.name);
    }
}
