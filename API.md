# API

## Components

## WalletComponent

## CreateWallet(string name)

#### Description

Creates a new Wallet object with the given name and adds it to the
wallets dictionary.

#### Parameters

-   name: The name of the new wallet.

## DeleteWallet(string id)

#### Description

Deletes the Wallet object with the given ID from the wallets dictionary.

#### Parameters

-   id: The ID of the wallet to delete.

## SelectWallet(string id)

#### Description

Sets the currentWallet field to the Wallet object with the given ID.

#### Parameters

-   id: The ID of the wallet to select.

## GetWallet(string id)

#### Description

Returns the Wallet object with the given ID.

#### Parameters

-   id: The ID of the wallet to get.

#### Returns

The Wallet object with the given ID.

## GetWallets()

#### Description

Returns a list of all Wallet objects.

#### Returns

A list of all Wallet objects.

## GetCoinMetadata(string symbol)

#### Description

Returns the CoinMetadata object for the coin with the given symbol.

#### Parameters

-   symbol: The symbol of the coin to get metadata for.

#### Returns

The CoinMetadata object for the coin with the given symbol.

## GetCoinGeckoData(string symbol)

#### Description

Returns the SUIMarketData object for the coin with the given symbol.

#### Parameters

-   symbol: The symbol of the coin to get market data for.

#### Returns

The SUIMarketData object for the coin with the given symbol.

## GetCoinPage(string symbol)

#### Description

Returns the CoinPage object for the coin with the given symbol.

#### Parameters

-   symbol: The symbol of the coin to get the page for.

#### Returns

The CoinPage object for the coin with the given symbol.

## GetCoinImage(string symbol)

#### Description

Returns the Sprite object for the coin with the given symbol.

#### Parameters

-   symbol: The symbol of the coin to get the image for.

#### Returns

The Sprite object for the coin with the given symbol.

## UpdateCoinData()

#### Description

Updates the cached coin data by making an RPC call to the SUI daemon and
parsing the response.

## UpdateCoinMetadata()

#### Description

Updates the coinMetadatas dictionary by making an RPC call to the SUI
daemon and parsing the response.

## UpdateCoinGeckoData()

#### Description

Updates the coinGeckoData dictionary by making an API call to CoinGecko
and parsing the response.

## UpdateCoinPages()

#### Description

Updates the coinPages dictionary by making an API call to CoinGecko and
parsing the response.

## UpdateCoinImages()

#### Description

Updates the coinImages dictionary by loading coin images from the
Resources folder.

## UpdateLastUpdated()

#### Description

Updates the lastUpdated field to the current date and time.

## SaveCoinData()

#### Description

Saves the cached coin data to disk.

## LoadCoinData()

#### Description

Loads the cached coin data from disk.

## GetCoinData()

#### Description

Makes an RPC call to the SUI daemon to get the latest coin data.

#### Returns

A Task\<RpcResponse> object representing the RPC response.

## GetCoinMetadata()

#### Description

Makes an RPC call to the SUI daemon to get the latest coin metadata.

#### Returns

A Task\<RpcResponse> object representing the RPC response.

## GetTransaction(string txid)

#### Description

Returns the Transaction object with the given transaction ID.

#### Parameters

-   txid: The transaction ID of the transaction to get.

#### Returns

The Transaction object with the given transaction ID.

## GetTransactions()

#### Description

Returns a list of all Transaction objects.

#### Returns

A list of all Transaction objects.

## GetTransactionHistory(string address)

#### Description

Returns a list of Transaction objects for the given address.

#### Parameters

-   address: The address to get transaction history for.

#### Returns

A list of Transaction objects for the given address.

## GetBalance(string address)

#### Description

Returns the balance of the given address.

#### Parameters

-   address: The address to get the balance for.

#### Returns

The balance of the given address.

## SendTransaction(string fromAddress, string toAddress, decimal amount)

#### Description

Sends a transaction from the given address to the given address with the
given amount.

#### Parameters

-   fromAddress: The address to send the transaction from.

-   toAddress: The address to send the transaction to.

-   amount: The amount to send in the transaction.

#### Returns

A Task\<RpcResponse> object representing the RPC response.

# SimpleScreenManager

## Property: screens

#### Description

Gets or sets the array of BaseScreen objects managed by the
SimpleScreenManager.

## Property: currentScreen

#### Description

Gets or sets the current BaseScreen object being displayed by the
SimpleScreenManager.

## Property: previousScreen

#### Description

Gets or sets the previous BaseScreen object that was displayed by the
SimpleScreenManager.

## Property: historyCount

#### Description

Gets the number of BaseScreen objects in the history stack of the
SimpleScreenManager.

## Property: mainHolder

#### Description

Gets or sets the Transform object that holds the BaseScreen objects
managed by the SimpleScreenManager.

## Hide()

#### Description

Hides the mainHolder object of the SimpleScreenManager.

## Show()

#### Description

Shows the mainHolder object of the SimpleScreenManager.

## ToggleAllScreens()

#### Description

Toggles the visibility of the mainHolder object of the
SimpleScreenManager.

## GoBack()

#### Description

Goes back to the previous BaseScreen object in the history stack of the
SimpleScreenManager.

## ClearHistory(BaseScreen baseScreen)

#### Description

Clears the history stack of the SimpleScreenManager and optionally
pushes a BaseScreen object onto the stack.

#### Parameters

-   baseScreen: The BaseScreen object to push onto the history stack.

## ShowScreen(BaseScreen curScreen, BaseScreen screen)

#### Description

Shows the given BaseScreen object and sets it as the current screen of
the SimpleScreenManager.

#### Parameters

-   curScreen: The current BaseScreen object being displayed by the
    > SimpleScreenManager.

-   screen: The BaseScreen object to show.

## ShowScreen(string name)

#### Description

Shows the BaseScreen object with the given name and sets it as the
current screen of the SimpleScreenManager.

#### Parameters

-   name: The name of the BaseScreen object to show.

## ShowScreen(string name, object data = default)

#### Description

Shows the BaseScreen object with the given name and sets it as the
current screen of the SimpleScreenManager, passing the given data to the
ShowScreen method of the BaseScreen object.

#### Parameters

-   name: The name of the BaseScreen object to show.

-   data: The data to pass to the ShowScreen method of the BaseScreen
    > object.

## ShowScreen(BaseScreen curScreen, int index)

#### Description

Shows the BaseScreen object at the given index and sets it as the
current screen of the SimpleScreenManager.

#### Parameters

-   curScreen: The current BaseScreen object being displayed by the
    > SimpleScreenManager.

-   index: The index of the BaseScreen object to show.

## ShowScreen(BaseScreen curScreen, string name, object data = null)

#### Description

Shows the BaseScreen object with the given name and sets it as the
current screen of the SimpleScreenManager, passing the given data to the
ShowScreen method of the BaseScreen object.

#### Parameters

-   curScreen: The current BaseScreen object being displayed by the
    > SimpleScreenManager.

-   name: The name of the BaseScreen object to show.

-   data: The data to pass to the ShowScreen method of the BaseScreen
    > object.

## HideScreen(string name)

#### Description

Hides the BaseScreen object with the given name.

#### Parameters

-   name: The name of the BaseScreen object to hide.

## HideAll(int screenIndex = 0)

#### Description

Hides all BaseScreen objects managed by the SimpleScreenManager except
for the BaseScreen object at the given index.

#### Parameters

-   screenIndex: The index of the BaseScreen object to show. Default
    > value is 0.

# BaseScreen

## Property: manager

#### Description

Gets or sets the SimpleScreenManager object that manages the BaseScreen
object.

## Property: tween

#### Description

Gets or sets the IScreenAnimation object that animates the BaseScreen
object.

## GoTo(string page, object data = null)

#### Description

Navigates to the BaseScreen object with the given name and passes the
given data to the ShowScreen method of the BaseScreen object.

#### Parameters

-   page: The name of the BaseScreen object to navigate to.

-   data: The data to pass to the ShowScreen method of the BaseScreen
    > object.

## HideScreen()

#### Description

Hides the BaseScreen object.

## InitScreen()

#### Description

Initializes the BaseScreen object.

## ShowScreen(object data = null)

#### Description

Shows the BaseScreen object and passes the given data to the ShowScreen
method of the BaseScreen object.

#### Parameters

-   data: The data to pass to the ShowScreen method of the BaseScreen
    > object.

## ShowScreen(T data = default)

#### Description

Shows the BaseScreen object and passes the given data to the ShowScreen
method of the BaseScreen object.

#### Parameters

-   data: The data to pass to the ShowScreen method of the BaseScreen
    > object.

## GetInputByName(string name) where T : Object

#### Description

Gets the Object with the given name from the BaseScreen object.

#### Parameters

-   name: The name of the Object to get.

#### Returns

The Object with the given name.

# InfoPopupManager

The **InfoPopupManager** class is responsible for managing notification
popups in a Unity game. It contains an enumeration for different types
of notification popups, as well as properties for the content holder,
notification prefab, colors, and sprites used for the notification
popups. The class also has a list of **NotificationPopup** objects that
are managed by the InfoPopupManager, and a transform object used as an
underlay for the notification popups.

The **AddNotif** method adds a notification popup of the given type and
message to the **InfoPopupManager**. If the message is the same as the
previous notification popup, it will not be added. The **ClearNotif**
method clears all notification popups managed by the
**InfoPopupManager**. The **RemoveNotif** method removes the given
**NotificationPopup** object from the list of notification popups
managed by the **InfoPopupManager**.

#### Example

| infoPopupManager.AddNotif(InfoType.Info, "This is an info message."); |
|-----------------------------------------------------------------------|

The above example adds an info notification popup with the message "This
is an info message." to the **InfoPopupManager**.

The **AddNotif** method first checks if the message is the same as the
previous notification popup. If it is, the method returns without adding
a new notification popup. If the message is different, the method
instantiates a new notification popup using the *notifPrefab* object and
sets its color and sprite based on the given type. The new notification
popup is then added to the notifQueue list. If there are more than three
notification popups in the queue, the oldest one is removed using the
**Destroy** method and the **Remove** method of the *notifQueue* list.
Finally, the **SetActive** method of the underlay object is called to
make the underlay visible.

## Property: contentHolder

#### Description

Gets or sets the Transform object that holds the notification popups
managed by the InfoPopupManager.

## Property: notifPrefab

#### Description

Gets or sets the GameObject prefab used for creating notification
popups.

## Property: instance

#### Description

Gets or sets the InfoPopupManager instance.

## Property: warningColor

#### Description

Gets or sets the color used for warning notification popups.

## Property: errorColor

#### Description

Gets or sets the color used for error notification popups.

## Property: infoColor

#### Description

Gets or sets the color used for info notification popups.

## Property: warningSprite

#### Description

Gets or sets the sprite used for warning notification popups.

## Property: errorSprite

#### Description

Gets or sets the sprite used for error notification popups.

## Property: infoSprite

#### Description

Gets or sets the sprite used for info notification popups.

## Property: notifQueue

#### Description

Gets or sets the list of NotificationPopup objects managed by the
InfoPopupManager.

## Property: underlay

#### Description

Gets or sets the Transform object used as an underlay for the
notification popups managed by the InfoPopupManager.

## AddNotif(InfoType type, string message)

#### Description

Adds a notification popup of the given type and message to the
InfoPopupManager.

#### Parameters

-   type: The type of the notification popup to add.

-   message: The message to display in the notification popup.

## ClearNotif()

#### Description

Clears all notification popups managed by the InfoPopupManager.

## RemoveNotif(NotificationPopup notif)

#### Description

Removes the given NotificationPopup object from the list of notification
popups managed by the InfoPopupManager.

#### Parameters

-   notif: The NotificationPopup object to remove.

SDK

# WebsocketController

The WebsocketController is a class that provides functionality for
setting up and managing a WebSocket connection to a server. Here's an
overview of the methods contained in the class:

## SetupConnection(string url)

#### Description

This method sets up a WebSocket connection to the specified URL. It
takes in a string url as a parameter.

## Subscribe(object filterParams)

#### Description

This method subscribes to a WebSocket event with the specified filter
parameters. It takes in an object filterParams as a parameter.

## UnsubscribeCurrent()

#### Description

This method unsubscribes from the current WebSocket event.

## Unsubscribe(string id)

#### Description

This method unsubscribes from a WebSocket event with the specified ID.
It takes in a string id as a parameter.

## Stop()

#### Description

This method closes the WebSocket connection.

## Event Handlers

The class also contains several event handlers that are triggered when
certain WebSocket events occur:

-   OnOpen: This event handler is triggered when the WebSocket
    > connection is opened.

-   OnError: This event handler is triggered when an error occurs with
    > the WebSocket connection.

-   OnClose: This event handler is triggered when the WebSocket
    > connection is closed.

-   OnMessage: This event handler is triggered when a message is
    > received from the WebSocket server.

The class also contains a Update method that is called every frame. This
method dispatches any messages in the WebSocket message queue.

The WebsocketController class uses the NativeWebSocket library to handle
WebSocket connections. It also uses the JsonConvert class from the
Newtonsoft.Json library to serialize and deserialize JSON objects.

Overall, the WebsocketController class provides a simple and convenient
way to set up and manage WebSocket connections in a Unity project.

# RPCClient

The **RPCClient** class is a that provides methods for making remote
procedure calls (RPCs) to a server using Unity's **UnityWebRequest**
class. The class contains several public and internal methods for
sending requests to the server, downloading images, and sending requests
using coroutines.

The **RPCClient** constructor takes a URI string as an argument and sets
it as the \_uri field. The **SendRequest** method sends a JSON-RPC
request to the server and returns a **JsonRpcResponse\<T>** object,
which contains the result of the request. The **SendAPIRequest** method
is similar to **SendRequest**, but it returns an object of type **T**
instead of a **JsonRpcResponse\<T>** object.

The **Get** method sends a GET request to the specified URL and returns
an object of type **T**. The **DownloadImage** method downloads an image
from the specified URL and returns it as a Sprite object.

The **SendRequestCoroutine** method sends a request to the server using
a coroutine and calls the specified callback function when the request
is complete.

Overall, the **RPCClient** class provides a simple and efficient way to
make RPCs to a server using Unity's **UnityWebRequest** class, allowing
for easy communication with a remote server.

## RPCClient(string uri)

The constructor for the RPCClient class takes a URI string as an
argument and sets it as the \_uri field.

## async Task\<JsonRpcResponse\<T>\> SendRequest\<T>(object data)

##### Description

The SendRequest method takes an object of type T and sends it to the
server as a JSON-RPC request. The method returns a JsonRpcResponse\<T>
object, which contains the result of the request.

##### Parameters

-   data: The data to be sent to the server as a JSON-RPC request.

##### Returns

-   Task\<JsonRpcResponse\<T>\>: A Task object that represents the
    > asynchronous operation. The JsonRpcResponse\<T> object contains
    > the result of the request.

## async Task\<T> SendAPIRequest\<T>(object data)

##### Description

The SendAPIRequest method is similar to SendRequest, but it returns an
object of type T instead of a JsonRpcResponse\<T> object.

##### Parameters

-   data: The data to be sent to the server as a JSON-RPC request.

##### Returns

-   Task\<T>: A Task object that represents the asynchronous operation.
    > The object of type T contains the result of the request.

## async Task\<T> Get\<T>(string url)

##### Description

The Get method sends a GET request to the specified URL and returns an
object of type T.

##### Parameters

-   url: The URL to send the GET request to.

##### Returns

-   Task\<T>: A Task object that represents the asynchronous operation.
    > The object of type T contains the result of the request.

## async Task\<Sprite> DownloadImage(string url)

##### Description

The DownloadImage method downloads an image from the specified URL and
returns it as a Sprite object.

##### Parameters

-   url: The URL to download the image from.

##### Returns

-   Task\<Sprite>: A Task object that represents the asynchronous
    > operation. The Sprite object contains the downloaded image.

## IEnumerator SendRequestCoroutine(string uri, Action\<UnityWebRequest> callback)

##### Description

The SendRequestCoroutine method sends a request to the server using a
coroutine and calls the specified callback function when the request is
complete.

##### Parameters

-   uri: The URI to send the request to.

-   callback: The callback function to call when the request is
    > complete.

##### Returns

-   IEnumerator: An IEnumerator object that represents the coroutine.

# SUIRPCClient

The SUIRPCClient class is a class that provides functionality for
sending requests to a SUI RPC server. Class extends RPCClient class.

## Constructor

## public SUIRPCClient(string uri)

Creates a new instance of the SUIRPCClient class with the specified URI.

##### Parameters

-   uri - The URI of the SUI RPC server.

#### Public Methods

## SendRequestAsync

##### Description

Sends a JSON-RPC request to the SUI RPC server and returns the response.

##### Parameters

-   data - The data to send with the request.

##### Returns

-   A JsonRpcResponse\<T> object representing the response from the
    > server.

## SendRequestAsync

##### Description

Sends a JSON-RPC request to the SUI RPC server and returns the response.

##### Parameters

-   data - The RPCRequestBase object representing the request to send.

##### Returns

-   A JsonRpcResponse\<T> object representing the response from the
    > server.

## SendRequestAsync

##### Description

Sends a JSON-RPC request to the SUI RPC server and returns the response.

##### Parameters

-   method - The name of the method to call.

-   parameters - The parameters to pass to the method.

##### Returns

-   A JsonRpcResponse\<T> object representing the response from the
    > server.

## GetAllBalances

##### Description

Gets the balances of all coins for the specified wallet.

##### Parameters

-   wallet - The wallet to get the balances for.

##### Returns

-   A JsonRpcResponse\<List\<Balance>\> object representing the response
    > from the server.

## GetAllBalances

##### Description

Gets the balances of all coins for the specified public key.

##### Parameters

-   publicKey - The public key to get the balances for.

##### Returns

-   A JsonRpcResponse\<List\<Balance>\> object representing the response
    > from the server.

## GetBalance

##### Description

Gets the balance of the specified coin for the specified wallet.

##### Parameters

-   wallet - The wallet to get the balance for.

-   cointType - The type of coin to get the balance for.

##### Returns

-   A JsonRpcResponse\<Balance> object representing the response from
    > the server.

## GetBalance

##### Description

Gets the balance of the specified coin for the specified public key.

##### Parameters

-   publicKey - The public key to get the balance for.

-   cointType - The type of coin to get the balance for.

##### Returns

-   A JsonRpcResponse\<Balance> object representing the response from
    > the server.

## GetAllCoins

##### Description

Gets all coins for the specified wallet.

##### Parameters

-   wallet - The wallet to get the coins for.

-   cursor - The cursor to use for pagination.

-   limit - The maximum number of coins to return.

##### Returns

-   A JsonRpcResponse\<PageForCoinAndObjectID> object representing the
    > response from the server.

## GetAllCoins

##### Description

Gets all coins for the specified public key.

##### Parameters

-   publicKey - The public key to get the coins for.

-   cursor - The cursor to use for pagination.

-   limit - The maximum number of coins to return.

##### Returns

-   A JsonRpcResponse\<PageForCoinAndObjectID> object representing the
    > response from the server.

## GetCoins

##### Description

Gets the coins of the specified type for the specified wallet.

##### Parameters

-   wallet - The wallet to get the coins for.

-   cointType - The type of coin to get.

-   cursor - The cursor to use for pagination.

-   limit - The maximum number of coins to return.

##### Returns

-   A JsonRpcResponse\<CoinPage> object representing the response from
    > the server.

## GetCoinMetadata

##### Description

Gets the metadata for the specified coin.

##### Parameters

-   coinType - The type of coin to get the metadata for.

##### Returns

-   A JsonRpcResponse\<CoinMetadata> object representing the response
    > from the server.

## GetReferenceGasPrice

##### Description

Gets the reference gas price.

##### Returns

-   A JsonRpcResponse\<string> object representing the response from the
    > server.

## GetObject

##### Description

Gets the object with the specified ID.

##### Parameters

-   objectId - The ID of the object to get.

##### Returns

-   A SUIObjectResponse object representing the response from the
    > server.

## GetObject

##### Description

Gets the object with the specified ID.

##### Parameters

-   objectId - The ID of the object to get.

##### Returns

-   A SUIObjectResponse object representing the response from the
    > server.

## GetOwnedObjects

##### Description

Gets the objects owned by the specified address.

##### Parameters

-   address - The address to get the objects for.

-   query - The query to use for filtering the objects.

-   objectId - The ID of the object to start the query from.

-   limit - The maximum number of objects to return.

##### Returns

-   A Page_for_SuiObjectResponse_and_ObjectID object representing the
    > response from the server.

## GetTotalSupply

##### Description

Gets the total supply of the specified coin.

##### Parameters

-   coinType - The type of coin to get the total supply for.

##### Returns

-   A Supply object representing the response from the server.

## ExecuteTransactionBlock

##### Description

Executes a transaction block.

##### Parameters

-   args - The arguments to use for the transaction block.

##### Returns

-   A SuiTransactionBlockResponse object representing the response from
    > the server.

## ExecuteTransactionBlock

##### Description

Executes a transaction block.

##### Parameters

-   txBytes - The transaction bytes to use for the transaction block.

-   serializedSignatures - The serialized signatures to use for the
    > transaction block.

-   options - The options to use for the transaction block.

-   requestType - The type of request to use for the transaction block.

##### Returns

-   A JsonRpcResponse\<SuiTransactionBlockResponse> object representing
    > the response from the server.

## DryRunTransactionBlock

##### Description

Dry runs a transaction block.

##### Parameters

-   txBytes - The transaction bytes to use for the transaction block.

##### Returns

-   A JsonRpcResponse\<SuiTransactionBlockResponse> object representing
    > the response from the server.

## PaySui

##### Description

Pays SUI.

##### Parameters

-   signer - The wallet to use for signing the transaction.

-   inputCoins - The input coins to use for the transaction.

-   recipients - The recipients of the transaction.

-   amounts - The amounts to send to each recipient.

-   gasBudget - The gas budget for the transaction.

##### Returns

-   A JsonRpcResponse\<TransactionBlockBytes> object representing the
    > response from the server.

## PayAllSui

##### Description

Pays all SUI.

##### Parameters

-   signer - The wallet to use for signing the transaction.

-   inputCoins - The input coins to use for the transaction.

-   recipients - The recipient of the transaction.

-   gasBudget - The gas budget for the transaction.

##### Returns

-   A JsonRpcResponse\<TransactionBlockBytes> object representing the
    > response from the server.

## Pay

##### Description

Returns built transaction with passed parameters.

##### Parameters

-   signer - The wallet to use for signing the transaction.

-   inputCoins - The input coins to use for the transaction.

-   recipients - The recipients of the transaction.

-   amounts - The amounts to send to each recipient.

-   gas - The gas to use for the transaction.

-   gasBudget - The gas budget for the transaction.

##### Returns

-   A JsonRpcResponse\<TransactionBlockBytes> object representing the
    > response from the server.

## QueryEvents

##### Description

Queries events.

##### Parameters

-   filter - The filter to use for the query.

##### Returns

-   A JsonRpcResponse\<PageForEventAndEventID> object representing the
    > response from the server.

## QueryTransactionBlocks

##### Description

Queries transaction blocks.

##### Parameters

-   filters - The filters to use for the query.

##### Returns

-   A
    > JsonRpcResponse\<PageForTransactionBlockResponseAndTransactionDigest>
    > object representing the response from the server.

## MultiGetTransactionBlocks

##### Description

Gets multiple transaction blocks.

##### Parameters

-   digests - The digests of the transaction blocks to get.

-   options - The options to use for the transaction blocks.

##### Returns

-   A List\<SuiTransactionBlockResponse> object representing the
    > response from the server.

# Wallet.cs

## Wallet()

#### Description

Constructor for the Wallet class.

## Wallet(KeyPair keyPair, string password = "", string walletName = "")

#### Description

Constructor for the Wallet class that takes a KeyPair object, password,
and wallet name as parameters.

#### Parameters

-   keyPair: The KeyPair object to use for the wallet.

-   password: The password to use for the wallet.

-   walletName: The name of the wallet.

## Wallet(string mnemonic, string password = "", string walletName = "")

#### Description

Constructor for the Wallet class that takes a mnemonic, password, and
wallet name as parameters.

#### Parameters

-   mnemonic: The mnemonic to use for the wallet.

-   password: The password to use for the wallet.

-   walletName: The name of the wallet.

## CreateAccount()

#### Description

Generates a new mnemonic and key pair for the wallet.

#### Returns

The KeyPair object for the new account.

## RestoreAccount(string mnemonic)

#### Description

Restores a key pair from the given mnemonic.

#### Parameters

-   mnemonic: The mnemonic to use for restoring the key pair.

#### Returns

The KeyPair object for the restored account.

## GetWalletSavedKeys()

#### Description

Returns a list of all saved wallet names.

#### Returns

A list of all saved wallet names.

## SaveWallet()

#### Description

Saves the wallet to disk.

## SaveWallet(string newPassword)

#### Description

Saves the wallet to disk with the given password.

#### Parameters

-   newPassword: The password to use for saving the wallet.

## RestoreWallet(string walletName, string password)

#### Description

Restores a wallet from disk.

#### Parameters

-   walletName: The name of the wallet to restore.

-   password: The password to use for restoring the wallet.

#### Returns

The restored Wallet object.

## RestoreWalletFromPrivateKey(string privateKey)

#### Description

Restores a wallet from the given private key.

#### Parameters

-   privateKey: The private key to use for restoring the wallet.

## RemoveWallet()

#### Description

Removes the wallet from disk.

## Sign(byte\[\] message)

#### Description

Signs the given message with the wallet's private key.

#### Parameters

-   message: The message to sign.

#### Returns

The signature for the message.

## SignData(byte\[\] data)

#### Description

Signs the given data with the wallet's private key.

#### Parameters

-   data: The data to sign.

#### Returns

The signature for the data.

## GetMessageWithIntent(byte\[\] message)

#### Description

Adds an intent to the given message.

#### Parameters

-   message: The message to add an intent to.

#### Returns

The message with the added intent.

I hope this helps! Let me know if you have any further questions.

# Mnemonic.cs

## GenerateNewMnemonic()

#### Description

Generates a new BIP39 mnemonic.

#### Returns

The generated mnemonic.

## CheckMnemonicValidity(string mnemonic)

#### Description

Checks if the given mnemonic is valid.

#### Parameters

-   mnemonic: The mnemonic to check.

#### Returns

true if the mnemonic is valid, false otherwise.

## StringToByteArrayFastest(string hex)

#### Description

Converts a hexadecimal string to a byte array.

#### Parameters

-   hex: The hexadecimal string to convert.

#### Returns

The byte array representation of the hexadecimal string.

## GetBIP39SeedBytes(string seed)

#### Description

Converts a BIP39 mnemonic to a byte array seed.

#### Parameters

-   seed: The BIP39 mnemonic to convert.

#### Returns

The byte array seed.

## MnemonicToSeedHex(string seed)

#### Description

Converts a BIP39 mnemonic to a hexadecimal seed.

#### Parameters

-   seed: The BIP39 mnemonic to convert.

#### Returns

The hexadecimal seed.

## GenerateSeedFromMnemonic(string mnemonic)

#### Description

Generates a seed from a BIP39 mnemonic.

#### Parameters

-   mnemonic: The BIP39 mnemonic to generate the seed from.

#### Returns

The generated seed.

## IsValidPath(string path)

#### Description

Checks if the given derivation path is valid.

#### Parameters

-   path: The derivation path to check.

#### Returns

true if the derivation path is valid, false otherwise.

## IsValidMnemonic(string mnemonic)

#### Description

Checks if the given mnemonic is valid.

#### Parameters

-   mnemonic: The mnemonic to check.

#### Returns

true if the mnemonic is valid, false otherwise.

## SanitizeMnemonic(string mnemonic)

#### Description

Sanitizes the given mnemonic.

#### Parameters

-   mnemonic: The mnemonic to sanitize.

#### Returns

The sanitized mnemonic.

## DerivePath(string path, byte\[\] \_masterKey, byte\[\] \_chainCode)

#### Description

Derives a key pair from the given derivation path, master key, and chain
code.

#### Parameters

-   path: The derivation path to use.

-   \_masterKey: The master key to use.

-   \_chainCode: The chain code to use.

#### Returns

A tuple containing the derived key and chain code.

## GenerateKeyPairFromMnemonic(string mnemonics)

#### Description

Generates a key pair from the given BIP39 mnemonic.

#### Parameters

-   mnemonics: The BIP39 mnemonic to use.

#### Returns

A KeyPair object containing the generated public and private keys.

## EncryptMnemonicWithPassword(string mnemonic, string password)

#### Description

Encrypts the given BIP39 mnemonic with the given password.

#### Parameters

-   mnemonic: The BIP39 mnemonic to encrypt.

-   password: The password to use for encryption.

#### Returns

The encrypted mnemonic.

## DecryptMnemonicWithPassword(string encryptedMnemonic, string password)

#### Description

Decrypts the given encrypted BIP39 mnemonic with the given password.

#### Parameters

-   encryptedMnemonic: The encrypted BIP39 mnemonic to decrypt.

-   password: The password to use for decryption.

#### Returns

The decrypted BIP39 mnemonic.

## GetPasswordWithMenmonic(string encryptedMnemonic, string mnemonic)

#### Description

Decrypts the given encrypted BIP39 mnemonic with the given BIP39
mnemonic.

#### Parameters

-   encryptedMnemonic: The encrypted BIP39 mnemonic to decrypt.

-   mnemonic: The BIP39 mnemonic to use for decryption.

#### Returns

The password used for encryption.

# KeyPair.cs

## Property: publicKey

#### Description

Gets or sets the public key for the KeyPair object.

## Property: privateKey

#### Description

Gets or sets the private key for the KeyPair object.

## Property: publicKeyString

#### Description

Gets or sets the public key as a base64-encoded string.

## Property: suiAddress

#### Description

Gets or sets the SUI address for the KeyPair object.

## Property: suiSecret

#### Description

Gets the SUI secret for the KeyPair object.

## IsSuiAddressInCorrectFormat(string address)

#### Description

Checks if the given SUI address is in the correct format.

#### Parameters

-   address: The SUI address to check.

#### Returns

true if the SUI address is in the correct format, false otherwise.

## KeyPair(byte\[\] publicKey, byte\[\] privateKey)

#### Description

Constructor for the KeyPair class that takes a public key and private
key as parameters.

#### Parameters

-   publicKey: The public key to use for the KeyPair object.

-   privateKey: The private key to use for the KeyPair object.

## ToSuiAddress(byte\[\] publicKeyBytes)

#### Description

Converts the given public key to an SUI address.

#### Parameters

-   publicKeyBytes: The public key to convert.

#### Returns

The SUI address for the public key.

## GetPrivateKeyFromSuiSecret(string suiSecret)

#### Description

Gets the private key from the given SUI secret.

#### Parameters

-   suiSecret: The SUI secret to use for getting the private key.

#### Returns

The private key for the SUI secret.

## GenerateKeyPairFromPrivateKey(string privateKey)

#### Description

Generates a KeyPair object from the given private key.

#### Parameters

-   privateKey: The private key to use for generating the KeyPair
    > object.

#### Returns

The generated KeyPair object.

# StringCypher

## Encrypt(string plainText, string passPhrase)

#### Description

Encrypts the given plain text using the given pass phrase.

#### Parameters

-   plainText: The plain text to encrypt.

-   passPhrase: The pass phrase to use for encryption.

#### Returns

The encrypted cipher text.

## Decrypt(string cipherText, string passPhrase)

#### Description

Decrypts the given cipher text using the given pass phrase.

#### Parameters

-   cipherText: The cipher text to decrypt.

-   passPhrase: The pass phrase to use for decryption.

#### Returns

The decrypted plain text.
