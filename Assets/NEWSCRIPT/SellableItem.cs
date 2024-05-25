using UnityEngine;
using TMPro;
using System.Collections.Generic;


[System.Serializable]
public class Properties
{
    // public Properties currentHotspotProperty = null;
    public StallManager.StallData stallData;
    public OnsenManager.OnsenData onsenData;

    public string name
    {
        get
        {
            if (stallData != null) return stallData.name;
            if (onsenData != null) return onsenData.name;
            return string.Empty;
        }
    }

    // public int Price
    // {
    //     get
    //     {
    //         if (stallData != null) return stallData.stagePrices[stallData.currentStageIndex];
    //         if (onsenData != null) return onsenData.priceOnsen;
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
            else if (onsenData != null)
            {
                return onsenData.priceOnsen;
            }
            else
            {
                return 0;
            }
        }
    }

    public List<int> rentPrices
    {
        get
        {
            if (stallData != null)
            {
                return stallData.rentPrices;
            }
            else if (onsenData != null)
            {
                return new List<int> { onsenData.rentPriceOnsen };
            }
            else
            {
                return new List<int>();
            }
        }
    }
    public List<int> buyoutPrices
    {
        get
        {
            if (stallData != null)
            {
                return stallData.buyoutPrices;
            }
            else if (onsenData != null)
            {
                return new List<int>();
            }
            else
            {
                return new List<int>();
            }
        }
    }
    public List<int> stagePrices
    {
        get
        {
            if (stallData != null)
            {
                return stallData.stagePrices;
            }
            else if (onsenData != null)
            {
                return new List<int> { onsenData.priceOnsen };
            }
            else
            {
                return new List<int>();
            }
        }
    }

    public int JSONwaypointIndex
    {
        get
        {
            if (stallData != null) return stallData.JSONwaypointIndex;
            if (onsenData != null) return onsenData.ONSENwaypointIndex;
            return -1;
        }
        set
        {
            if (stallData != null) stallData.JSONwaypointIndex = value;
            if (onsenData != null) onsenData.ONSENwaypointIndex = value;
        }
    }  

    public int currentStageIndex
    {
        get
        {
            if (stallData != null) return stallData.currentStageIndex;
            if (onsenData != null) return onsenData.currentStageIndex;
            return -1;
        }
        set
        {
            if (stallData != null) stallData.currentStageIndex = value;
            if (onsenData != null) onsenData.currentStageIndex = value;
        }
    }
    public bool owned
    {
        get
        {
            if (stallData != null) return stallData.owned;
            if (onsenData != null) return onsenData.owned;
            return false;
        }
        set
        {
            if (stallData != null) stallData.owned = value;
            if (onsenData != null) onsenData.owned = value;
        }
    }
    public bool isFireWork
    {
        get
        {
            if (stallData != null) return stallData.isFireWork;
            if (onsenData != null) return onsenData.isFireWork;
            return false;
        }
        set
        {
            if (stallData != null) stallData.isFireWork = value;
            if (onsenData != null) onsenData.isFireWork = value;
        }
    }
    public bool isWelcomeEvent
    {
        get
        {
            if (stallData != null) return stallData.isWelcomeEvent;
            if (onsenData != null) return onsenData.isWelcomeEvent;
            return false;
        }
        set
        {
            if (stallData != null) stallData.isWelcomeEvent = value;
            if (onsenData != null) onsenData.isWelcomeEvent = value;
        }
    }    
    public int ownerID
    {
        get
        {
            if (stallData != null) return stallData.ownerID;
            if (onsenData != null) return onsenData.ownerID;
            return 0;
        }
        set
        {
            if (stallData != null) stallData.ownerID = value;
            if (onsenData != null) onsenData.ownerID = value;
        }
    }

    public int teamownerID
    {
        get
        {
            if (stallData != null) return stallData.teamownerID;
            if (onsenData != null) return onsenData.teamownerID;
            return 0;
        }
        set
        {
            if (stallData != null) stallData.teamownerID = value;
            if (onsenData != null) onsenData.teamownerID = value;
        }
    }
    public List<GameObject> RentTagImages
    {
        get
        {
            if (stallData != null) return stallData.rentTagImages;
            if (onsenData != null) return onsenData.rentTagImages;
            return null;
        }
    }

    public TextMeshProUGUI rentText
    {
        get
        {
            if (stallData != null) return stallData.rentText;
            if (onsenData != null) return onsenData.onsenRentText;
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
            else if (onsenData != null)
            {
                // If it's a hot spring, return a list containing the OnsenImage
                return new List<GameObject> { onsenData.OnsenImage };
            }
            else
            {
                // Neither stallData nor onsenData is set, return null
                return null;
            }
        }
    }

    // public List<Properties> ietms = new List<ItemData>();
    // public Properties GetItemByWaypointIndex(int JSONwaypointIndex)
    // {
    //     foreach (var item in items)
    //     {
    //         if (item.JSONwaypointIndex == JSONwaypointIndex)
    //         {
    //             Debug.Log("Property found: " + item.name);

    //             return item;
    //         }
    //     }
    //     return null;
    // }
}
