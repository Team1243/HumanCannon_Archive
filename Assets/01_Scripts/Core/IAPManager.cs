using UnityEngine.Purchasing;
using UnityEngine;

public class IAPManager : MonoSingleton<IAPManager>, IStoreListener
{
    public readonly string productId_noAd = "noad_id";
    private bool isCanPurchase = true;

    private IStoreController storeController; // 구매 과정을 제어하는 함수 제공자

    public override void Init()
    {
        // InitUnityIAP();
    }

    // Unity IAP를 초기화하는 함수
    private void InitUnityIAP()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
        // 구글 플레이 상품들 추가 
        builder.AddProduct(productId_noAd, ProductType.NonConsumable, new IDs() { { productId_noAd, GooglePlay.Name } });
        
        UnityPurchasing.Initialize(this, builder);
        
        CheckNonConsumable(productId_noAd);
    }

    public void Purchase(string productId)
    {
        // if (isCanPurchase)
        // {
        //     storeController.InitiatePurchase(productId);
        // }
    }

    private void CheckNonConsumable(string productId)
    {
        Product product = storeController.products.WithID(productId); //상품 정의

        if (product != null) //상품이 존재
        {
            // 영수증을 찾을 수 있는지 (구매했었는지)
            bool isPurchased = product.hasReceipt;
            isCanPurchase = !isPurchased;
        }
    }

    #region interface

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        CheckNonConsumable(productId_noAd);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("초기화 실패 : " + error);
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("초기화 실패 : " + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("구매에 실패하였습니다.");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        
        if (product.definition.id == productId_noAd)
        {
            Debug.Log("광고 제거 구매 성공");
            // 실제로 인게임에 광고 제거 기능 추가해주면 돼
        }

        return PurchaseProcessingResult.Complete;
    }

    #endregion
}
