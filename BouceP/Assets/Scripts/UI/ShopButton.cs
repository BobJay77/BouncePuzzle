using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public GameObject purchaseNoAdsShopPopup;
    public GameObject thankYouPopup;

    //opens the specified popup based on whether they bought the no ads option
    public void OpenShop()
    {
        if (AdMobAds.instance == null)
            return;

        if (AdMobAds.instance.NoAds)
        {
            thankYouPopup.SetActive(true);
            purchaseNoAdsShopPopup.SetActive(false);
        }

        else
        {
            purchaseNoAdsShopPopup.SetActive(true);
            thankYouPopup.SetActive(false);
        }
    }
}
