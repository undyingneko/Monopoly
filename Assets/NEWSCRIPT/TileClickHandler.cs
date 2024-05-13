using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickHandler : MonoBehaviour, IPointerClickHandler
{
    private PropertyManager.PropertyData associatedProperty;

    public void SetAssociatedProperty(PropertyManager.PropertyData property)
    {
        associatedProperty = property;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.HandleTileClick(associatedProperty);
    }
}
