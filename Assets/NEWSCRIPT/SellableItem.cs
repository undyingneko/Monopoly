using UnityEngine;
using TMPro;
using System.Collections.Generic;


[System.Serializable]
public class SellableItem
{
    public PropertyManager.PropertyData propertyData;
    public HotSpringManager.HotSpringData hotSpringData;

    public string name
    {
        get
        {
            if (propertyData != null) return propertyData.name;
            if (hotSpringData != null) return hotSpringData.name;
            return string.Empty;
        }
    }

    // public int Price
    // {
    //     get
    //     {
    //         if (propertyData != null) return propertyData.stagePrices[propertyData.currentStageIndex];
    //         if (hotSpringData != null) return hotSpringData.priceHotSpring;
    //         return 0;
    //     }
    // }
    public int Price
    {
        get
        {
            if (propertyData != null)
            {
                if (propertyData.currentStageIndex >= 0 && propertyData.currentStageIndex < propertyData.stagePrices.Count)
                {
                    return propertyData.stagePrices[propertyData.currentStageIndex];
                }
                else
                {
                    return 0;
                }
            }
            else if (hotSpringData != null)
            {
                return hotSpringData.priceHotSpring;
            }
            else
            {
                return 0;
            }
        }
    }
   
    public int JSONwaypointIndex
    {
        get
        {
            if (propertyData != null) return propertyData.JSONwaypointIndex;
            if (hotSpringData != null) return hotSpringData.HOTSPRINGwaypointIndex;
            return -1;
        }
        set
        {
            if (propertyData != null) propertyData.JSONwaypointIndex = value;
            if (hotSpringData != null) hotSpringData.HOTSPRINGwaypointIndex = value;
        }
    }    
    public int currentStageIndex
    {
        get
        {
            if (propertyData != null) return propertyData.currentStageIndex;
            if (hotSpringData != null) return hotSpringData.currentStageIndex;
            return -1;
        }
        set
        {
            if (propertyData != null) propertyData.currentStageIndex = value;
            if (hotSpringData != null) hotSpringData.currentStageIndex = value;
        }
    }
    public bool owned
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
    public bool isHotSpot
    {
        get
        {
            if (propertyData != null) return propertyData.isHotSpot;
            if (hotSpringData != null) return hotSpringData.isHotSpot;
            return false;
        }
        set
        {
            if (propertyData != null) propertyData.isHotSpot = value;
            if (hotSpringData != null) hotSpringData.isHotSpot = value;
        }
    }
    public int ownerID
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

    public int teamownerID
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
    public List<GameObject> RentTagImages
    {
        get
        {
            if (propertyData != null) return propertyData.rentTagImages;
            if (hotSpringData != null) return hotSpringData.rentTagImages;
            return null;
        }
    }

    public TextMeshProUGUI rentText
    {
        get
        {
            if (propertyData != null) return propertyData.rentText;
            if (hotSpringData != null) return hotSpringData.hotspringRentText;
            return null;
        }
    }
    public List<GameObject> StageImages
    {
        get
        {
            if (propertyData != null)
            {
                // If it's a property, return the list of stage images
                return propertyData.stageImages;
            }
            else if (hotSpringData != null)
            {
                // If it's a hot spring, return a list containing the HotSpringImage
                return new List<GameObject> { hotSpringData.HotSpringImage };
            }
            else
            {
                // Neither propertyData nor hotSpringData is set, return null
                return null;
            }
        }
    }

    // public List<GameObject> StageImages
    // {
    //     get
    //     {
    //         if (propertyData != null && propertyData.stageImages != null && propertyData.currentStageIndex >= 0 && propertyData.currentStageIndex < propertyData.stageImages.Count)
    //         {
    //             return propertyData.stageImages[propertyData.currentStageIndex];
    //         }
    //         else if (hotSpringData != null)
    //         {
    //             return hotSpringData.HotSpringImage;
    //         }
    //         else
    //         {
    //             // Handle the case where neither propertyData nor hotSpringData is set
    //             Debug.LogWarning("Both propertyData and hotSpringData are null.");
    //             return null; // Or return a default GameObject if appropriate
    //         }
    //     }
    // }

}
