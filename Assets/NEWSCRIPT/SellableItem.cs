using UnityEngine;
using TMPro;
using System.Collections.Generic;


[System.Serializable]
public class SellableItem
{
    public StallManager.StallData stallData;
    public HotSpringManager.HotSpringData hotSpringData;

    public string name
    {
        get
        {
            if (stallData != null) return stallData.name;
            if (hotSpringData != null) return hotSpringData.name;
            return string.Empty;
        }
    }

    // public int Price
    // {
    //     get
    //     {
    //         if (stallData != null) return stallData.stagePrices[stallData.currentStageIndex];
    //         if (hotSpringData != null) return hotSpringData.priceHotSpring;
    //         return 0;
    //     }
    // }
    public int Price
    {
        get
        {
            if (stallData != null)
            {
                if (stallData.currentStageIndex >= 0 && stallData.currentStageIndex < stallData.stagePrices.Count)
                {
                    return stallData.stagePrices[stallData.currentStageIndex];
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
            if (stallData != null) return stallData.JSONwaypointIndex;
            if (hotSpringData != null) return hotSpringData.HOTSPRINGwaypointIndex;
            return -1;
        }
        set
        {
            if (stallData != null) stallData.JSONwaypointIndex = value;
            if (hotSpringData != null) hotSpringData.HOTSPRINGwaypointIndex = value;
        }
    }    
    public int currentStageIndex
    {
        get
        {
            if (stallData != null) return stallData.currentStageIndex;
            if (hotSpringData != null) return hotSpringData.currentStageIndex;
            return -1;
        }
        set
        {
            if (stallData != null) stallData.currentStageIndex = value;
            if (hotSpringData != null) hotSpringData.currentStageIndex = value;
        }
    }
    public bool owned
    {
        get
        {
            if (stallData != null) return stallData.owned;
            if (hotSpringData != null) return hotSpringData.owned;
            return false;
        }
        set
        {
            if (stallData != null) stallData.owned = value;
            if (hotSpringData != null) hotSpringData.owned = value;
        }
    }
    public bool isFireWork
    {
        get
        {
            if (stallData != null) return stallData.isFireWork;
            if (hotSpringData != null) return hotSpringData.isFireWork;
            return false;
        }
        set
        {
            if (stallData != null) stallData.isFireWork = value;
            if (hotSpringData != null) hotSpringData.isFireWork = value;
        }
    }
    public bool isWelcomeEvent
    {
        get
        {
            if (stallData != null) return stallData.isWelcomeEvent;
            if (hotSpringData != null) return hotSpringData.isWelcomeEvent;
            return false;
        }
        set
        {
            if (stallData != null) stallData.isWelcomeEvent = value;
            if (hotSpringData != null) hotSpringData.isWelcomeEvent = value;
        }
    }    
    public int ownerID
    {
        get
        {
            if (stallData != null) return stallData.ownerID;
            if (hotSpringData != null) return hotSpringData.ownerID;
            return 0;
        }
        set
        {
            if (stallData != null) stallData.ownerID = value;
            if (hotSpringData != null) hotSpringData.ownerID = value;
        }
    }

    public int teamownerID
    {
        get
        {
            if (stallData != null) return stallData.teamownerID;
            if (hotSpringData != null) return hotSpringData.teamownerID;
            return 0;
        }
        set
        {
            if (stallData != null) stallData.teamownerID = value;
            if (hotSpringData != null) hotSpringData.teamownerID = value;
        }
    }
    public List<GameObject> RentTagImages
    {
        get
        {
            if (stallData != null) return stallData.rentTagImages;
            if (hotSpringData != null) return hotSpringData.rentTagImages;
            return null;
        }
    }

    public TextMeshProUGUI rentText
    {
        get
        {
            if (stallData != null) return stallData.rentText;
            if (hotSpringData != null) return hotSpringData.hotspringRentText;
            return null;
        }
    }
    public List<GameObject> StageImages
    {
        get
        {
            if (stallData != null)
            {
                // If it's a property, return the list of stage images
                return stallData.stageImages;
            }
            else if (hotSpringData != null)
            {
                // If it's a hot spring, return a list containing the HotSpringImage
                return new List<GameObject> { hotSpringData.HotSpringImage };
            }
            else
            {
                // Neither stallData nor hotSpringData is set, return null
                return null;
            }
        }
    }

    // public List<GameObject> StageImages
    // {
    //     get
    //     {
    //         if (stallData != null && stallData.stageImages != null && stallData.currentStageIndex >= 0 && stallData.currentStageIndex < stallData.stageImages.Count)
    //         {
    //             return stallData.stageImages[stallData.currentStageIndex];
    //         }
    //         else if (hotSpringData != null)
    //         {
    //             return hotSpringData.HotSpringImage;
    //         }
    //         else
    //         {
    //             // Handle the case where neither stallData nor hotSpringData is set
    //             Debug.LogWarning("Both stallData and hotSpringData are null.");
    //             return null; // Or return a default GameObject if appropriate
    //         }
    //     }
    // }

}
