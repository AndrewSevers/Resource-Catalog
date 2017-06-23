#if PURCHASING
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Extensions {

    public class InAppPurchaseManager : Singleton<InAppPurchaseManager>, IStoreListener {
        [SerializeField]
        private GameProduct[] gameProducts;

        private IStoreController controller;
        private IExtensionProvider extensions;
        private bool initialized = false;

        Dictionary<string, GameProduct> gameProductMap;

        public enum Store {
            AppleAppStore = 1,
            GooglePlay = 2,
            MacAppStore = 4
        }

        #region Events
        public delegate void OnCompleteEvent(GameProduct aGameProduct);
        public OnCompleteEvent OnPurchaseComplete;

        public delegate void OnFailureEvent(GameProduct aGameProduct);
        public OnFailureEvent OnPurchaseFailure;

        public delegate void OnPurchasesEvent(bool aSuccess);
        public OnPurchasesEvent OnPurchaseRestored;
        #endregion

        #region Getters & Setters
        public Product[] StoreProducts {
            get { return controller.products.all; }
        }
        #endregion

        #region Initialization
        public override void Initialize() {
            base.Initialize();

            if (initialized == false) {
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                gameProductMap = new Dictionary<string, GameProduct>();

                ProductCatalog catalog = ProductCatalog.LoadDefaultCatalog();
                if (catalog != null) {
                    foreach (ProductCatalogItem product in catalog.allProducts) {
                        if (product.allStoreIDs.Count > 0) {
                            IDs ids = new IDs();
                            foreach (var storeID in product.allStoreIDs) {
                                ids.Add(storeID.id, storeID.store);
                            }
                            builder.AddProduct(product.id, product.type, ids);
                        } else {
                            builder.AddProduct(product.id, product.type);
                        }
                    }

                    foreach (GameProduct product in gameProducts) {
                        gameProductMap[product.ID] = product;
                    }
                }

                // Initialize unity's purchasing with the pre-configured builder we setup
                UnityPurchasing.Initialize(this, builder);
            }

            initialized = true;
        }

        public virtual void OnInitialized(IStoreController aController, IExtensionProvider aExtensions) {
            initialized = true;
            controller = aController;
            extensions = aExtensions;
        }

        // Upon failing initiailization this will be called and Unity IAP will attempt to reconnect until it succeeds
        public virtual void OnInitializeFailed(InitializationFailureReason aError) {
            Debug.LogWarning("[InAppPurchasingManager] Failure to initialize: " + aError);

#if UNITY_EDITOR
            initialized = true;
#endif
        }
        #endregion

        #region Purchasing
        /// <summary>
        /// Buy a product with the given ID
        /// </summary>
        /// <param name="aProductId">ID of the product to buy</param>
        public virtual void BuyProduct(string aProductId) {
#if UNITY_IOS || UNITY_ANDROID
            if (initialized == false) {
                Debug.LogWarning("[InAppPurchasingManager] Manager is not initialized can't make purchase");
                return;
            }

            Product product = controller.products.WithID(aProductId);
            if (product != null && product.availableToPurchase) {
                controller.InitiatePurchase(product);
            } else {
                Debug.LogError("[InAppPurchaseManager] Unable to make purchase with ID: " + aProductId);
            }
#else
            OnPurchaseComplete(gameProductMap[aProductId]);
#endif
        }

        public virtual PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs aEventArgs) {
            if (OnPurchaseComplete != null) {
                OnPurchaseComplete(gameProductMap[aEventArgs.purchasedProduct.definition.id]);
            }

            return PurchaseProcessingResult.Complete;
        }

        public virtual void OnPurchaseFailed(Product aProduct, PurchaseFailureReason aReason) {
            if (OnPurchaseFailure != null) {
                OnPurchaseFailure(gameProductMap[aProduct.definition.id]);
            }

            Debug.LogWarning(string.Format("[InAppPurchasingManager] Failed to purchase product: {0} with reason: {1}", aProduct.transactionID, aReason));
        }
        #endregion

        #region Product Management
        /// <summary>
        /// Restore non-consumable and subscription purchases
        /// </summary>
        public virtual void RestorePurchases() {
            if (initialized == false) {
                Debug.LogWarning("[InAppPurchasingManager] Manager can't restore purchases");
                return;
            }

#if UNITY_EDITOR
            CompleteRestoring(true);
#elif UNITY_IOS
            extensions.GetExtension<IAppleExtensions>().RestoreTransactions(CompleteRestoring);
#endif
        }

        protected virtual void CompleteRestoring(bool aSuccess) {
            Debug.Log("[InAppPurchaseManager] Completed restoration with result of: " + aSuccess);

            if (OnPurchaseRestored != null) {
                OnPurchaseRestored(aSuccess);
            }
        }
        #endregion

        #region Utilities
        public GameProduct GetGameProduct(string aProductID) {
            GameProduct product = null;
            gameProductMap.TryGetValue(aProductID, out product);
            return product;
        }
        #endregion

    }

    #region Game Product Class
    /// <summary>
    /// Game Product is meant to be a modifiable class that depicts game specific values for products
    /// </summary>
    [System.Serializable]
    public class GameProduct {
        [SerializeField]
        private string id;
        [SerializeField]
        private GameProductType type;
        [SerializeField]
        private int amount;

        #region Getters & Setters
        public string ID {
            get { return id; }
        }

        public GameProductType Type {
            get { return type; }
        }

        public int Amount {
            get { return amount; }
        }
        #endregion

        #region Constructor
        public GameProduct(GameProductType aType, int aAmount = 0) {
            type = aType;
            amount = aAmount;
        }
        #endregion
    }
    #endregion

    public enum GameProductType {
        Clams,
        SharkPoints,
        AdRemoval
    }

}
#endif
