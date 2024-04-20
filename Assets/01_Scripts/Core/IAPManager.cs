using UnityEngine.Purchasing;
using UnityEngine;

public class IAPManager : MonoSingleton<IAPManager>, IStoreListener
{
    public readonly string productId_noAd = "noad_id";
    private bool isCanPurchase = true;

    private IStoreController storeController; // ���� ������ �����ϴ� �Լ� ������

    public override void Init()
    {
        // InitUnityIAP();
    }

    // Unity IAP�� �ʱ�ȭ�ϴ� �Լ�
    private void InitUnityIAP()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
        // ���� �÷��� ��ǰ�� �߰� 
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
        Product product = storeController.products.WithID(productId); //��ǰ ����

        if (product != null) //��ǰ�� ����
        {
            // �������� ã�� �� �ִ��� (�����߾�����)
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
        Debug.Log("�ʱ�ȭ ���� : " + error);
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("�ʱ�ȭ ���� : " + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("���ſ� �����Ͽ����ϴ�.");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        
        if (product.definition.id == productId_noAd)
        {
            Debug.Log("���� ���� ���� ����");
            // ������ �ΰ��ӿ� ���� ���� ��� �߰����ָ� ��
        }

        return PurchaseProcessingResult.Complete;
    }

    #endregion
}
