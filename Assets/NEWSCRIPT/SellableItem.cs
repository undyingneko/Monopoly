using UnityEngine;
using TMPro;

[System.Serializable]
public class SellableItem
{
    public PropertyManager.PropertyData propertyData;
    public HotSpringManager.HotSpringData hotSpringData;

    public string Name
    {
        get
        {
            if (propertyData != null) return propertyData.name;
            if (hotSpringData != null) return hotSpringData.name;
            return string.Empty;
        }
    }

    public int Price
    {
        get
        {
            if (propertyData != null) return propertyData.stagePrices[propertyData.currentStageIndex];
            if (hotSpringData != null) return hotSpringData.priceHotSpring;
            return 0;
        }
    }

    public bool IsOwned
    {
        get
        {
            if (propertyData != null) return propertyData.owned;
            if (hotSpringData != null) return hotSpringData.owned;
            return false;
        }
        set
        {
            if (propertyData != null) propertyData.owned = value;
            if (hotSpringData != null) hotSpringData.owned = value;
        }
    }

    public int OwnerID
    {
        get
        {
            if (propertyData != null) return propertyData.ownerID;
            if (hotSpringData != null) return hotSpringData.ownerID;
            return 0;
        }
        set
        {
            if (propertyData != null) propertyData.ownerID = value;
            if (hotSpringData != null) hotSpringData.ownerID = value;
        }
    }

    public int TeamOwnerID
    {
        get
        {
            if (propertyData != null) return propertyData.teamownerID;
            if (hotSpringData != null) return hotSpringData.teamownerID;
            return 0;
        }
        set
        {
            if (propertyData != null) propertyData.teamownerID = value;
            if (hotSpringData != null) hotSpringData.teamownerID = value;
        }
    }
    
}
