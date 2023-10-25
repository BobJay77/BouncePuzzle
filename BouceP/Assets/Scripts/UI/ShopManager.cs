using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

[Serializable]
public class NonConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
}

public class ShopManager : MonoBehaviour, IDetailedStoreListener
{
    IStoreController m_storeController;

    public GameObject purchaseNoAdsShopPopup;
    public GameObject thankYouPopup;

    public NonConsumableItem ncItem;

    public ShopManager smanager;

    private void Awake()
    {
        smanager = this;
        SetupBuilder();
    }

    private void Update()
    {
        //print("controller " + m_storeController);
    }

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

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(ncItem.Id, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    // Non Consumable button to initiate purchase
    public void NonConsumableButtonClicked()
    {
        m_storeController.InitiatePurchase(ncItem.Id);
    }

    // Processing Purchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        // Retrieve the purchased product
        var product = purchaseEvent.purchasedProduct;

        print("Purchase Complete " + product.definition.id);

        if (product.definition.id == ncItem.Id) // Nonconsumable
        {
            AdMobAds.instance.NoAdsBought();

            OpenShop();
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print("initialize failed " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print("initialize failed " + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        print("purchase failed");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        print("purchase failed " + failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        print("Success");
        m_storeController = controller;
        CheckNonConsumable(ncItem.Id);
    }

    void CheckNonConsumable(string id)
    {
        if(m_storeController != null)
        {
            var product = m_storeController.products.WithID(id);
            if(product != null)
            {
                if(product.hasReceipt)
                {
                    AdMobAds.instance.NoAds = true;
                    GameSystem.instance.AccountSettings.NoAds = AdMobAds.instance.NoAds; //updating it in the account settings
                }
                else
                {
                    AdMobAds.instance.NoAds = false;
                    GameSystem.instance.AccountSettings.NoAds = AdMobAds.instance.NoAds; //updating it in the account settings
                }
            }
        }
    }
}
