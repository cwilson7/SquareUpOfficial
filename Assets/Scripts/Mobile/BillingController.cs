using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class BillingController : MonoBehaviour
{
    BillingProduct[] products, requestedProducts;

    private void Start()
    {
        products = NPSettings.Billing.Products;
        RequestBillingProducts();
    }

    public void RequestBillingProducts()
    {
        NPBinding.Billing.RequestForBillingProducts(NPSettings.Billing.Products);

        // At this point you can display an activity indicator to inform user that task is in progress
    }

    private void OnEnable()
    {
        // Register for callbacks
        Billing.DidFinishRequestForBillingProductsEvent += OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent += OnDidFinishTransaction;
    }

    private void OnDisable()
    {
        // Deregister for callbacks
        Billing.DidFinishRequestForBillingProductsEvent -= OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent -= OnDidFinishTransaction;
    }

    private void OnDidFinishProductsRequest(BillingProduct[] _regProductsList, string _error)
    {
        // Hide activity indicator

        // Handle response
        if (_error != null)
        {
            // Something went wrong
        }
        else
        {
            // Inject code to display received products
        }
    }

    public void BuyItem(BillingProduct _product)
    {
        if (NPBinding.Billing.IsProductPurchased(_product))
        {
            // Show alert message that item is already purchased

            return;
        }

        // Call method to make purchase
        NPBinding.Billing.BuyProduct(_product);        

        // At this point you can display an activity indicator to inform user that task is in progress
    }

    public void Buy(int itemID)
    {
        BuyItem(products[itemID]);
    }

    private void OnDidFinishTransaction(BillingTransaction _transaction)
    {
        if (_transaction != null)
        {

            if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS)
            {
                if (_transaction.TransactionState == eBillingTransactionState.PURCHASED)
                {
                    switch(_transaction.ProductIdentifier)
                    {
                        case "1_cube_coin":
                            //add cube coin to player currency
                            break;
                        case "5_cube_coins":

                            break;
                        case "10_cube_coins":

                            break;
                    }
                }
            }
        }
    }
}
