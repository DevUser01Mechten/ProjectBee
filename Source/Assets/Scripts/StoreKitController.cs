using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class StoreKitController : MonoBehaviourGUI
{
#if UNITY_IPHONE
	private List<StoreKitProduct> _products;


	void Start()
	{
		// you cannot make any purchases until you have retrieved the products from the server with the requestProductData method
		// we will store the products locally so that we will know what is purchaseable and when we can purchase the products
		StoreKitManager.productListReceivedEvent += allProducts =>
		{
			Debug.Log( "received total products: " + allProducts.Count );
			_products = allProducts;
		};
	}


	public void RequestProductData()
	{
		// array of product ID's from iTunesConnect. MUST match exactly what you have there!
		var productIdentifiers = new string[] { "eu.machten.Bee.extralives","eu.machten.Bee.removeads","eu.machten.Bee.10extralives"};
		StoreKitBinding.requestProductData( productIdentifiers );
	}


	public bool CanMakePayments()
	{
		return StoreKitBinding.canMakePayments();
	}


	public bool PurchaseExtraLives()
	{
		if( _products != null && _products.Count > 0 )
		{
			var product = _products[0];
			Debug.Log( "preparing to purchase product: " + product.productIdentifier );
			StoreKitBinding.purchaseProduct( product.productIdentifier, 1 );
			return true;
		}
		else
		{
			return false;
		}
	}
	

	public bool PurchaseRemoveAds()
	{
		if( _products != null && _products.Count > 0 )
		{
			var product = _products[1];
			Debug.Log( "preparing to purchase product: " + product.productIdentifier );
			StoreKitBinding.purchaseProduct( product.productIdentifier, 1 );
			return true;
		}
		else
		{
			return false;
		}
	}


	public bool Purchase10ExtraLives()
	{
		if( _products != null && _products.Count > 0 )
		{
			var product = _products[2];
			Debug.Log( "preparing to purchase product: " + product.productIdentifier );
			StoreKitBinding.purchaseProduct( product.productIdentifier, 1 );
			return true;
		}
		else
		{
			return false;
		}
	}
	
	
	public void Restore()
	{
		StoreKitBinding.restoreCompletedTransactions();
	}
	
#endif
}
