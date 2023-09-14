# AllArt SUI Blockchain Wallet Implementation for Unity

Welcome to the AllArt SUI Blockchain Wallet Implementation for Unity.
This Unity package provides a wallet implementation for the SUI
Blockchain, enabling straightforward integration of the SUI Blockchain
into your Unity projects.


## Table of Contents

- [Getting Started](#getting-started)
- [Usage](#usage)
- [Development](#development)
- [License](#license)
- [API](API.md)

## Getting Started

These instructions will get you a copy of the project up and running on
your local machine for development and testing purposes.

### Prerequisites

-   Unity Version 2020.3.7f1 or later.

-   A SUI Blockchain Node that the Unity client can interact with.

### Installation

To install this package, follow these steps:

1.  Open your Unity project.

2.  Open the package manager Window -> Package Manager

3.  Click on + and then Add package from git URL...

4.  Enter https://github.com/allartprotocol/allart-unity-sui-wallet.git

Unity should download and import the package automatically.

## Usage

After package has been imported, import prefabs from samples by
following these steps:

1.  Go to Package Manager

2.  Find AllArt Unity SUI Wallet in the list of packages added to your project.

3.  Under Samples you will find a SUI Wallet sample.

4.  Import it.

5.  Open the WalletSampleScene.

Wallet implementation has all the features you can expect from any
wallet and is more than enough for kick start your project. Most of the
functionalities are easily accessible from the WalletComponent.cs. Even
if you do not want to use the provided interface you can create your
implementation around it.

Wallets are saved locally by saving its mnemonic phrase encrypted with
the provided password. If you want to add new wallet you can do that
through interface or externally:

```c#
    WalletComponent.Instance.CreateWallet(this.mnemonic, password);
```

Restoring saved wallets:

```c#
    string password = "1234";
    WalletComponent.Instance.RestoreAllWallets(password);<br />
    Wallet wallet = WalletComponent.Instance.GetWalletByIndex(0);</th>
```

The **WalletComponent** class contains methods for creating, restoring,
and removing wallets, as well as retrieving wallet information.

The **CreateWalletWithNewMnemonic** method creates a new wallet with a
randomly generated mnemonic, password, and wallet name, and saves it to
the device. The **CreateWallet** method creates a new wallet with the
specified mnemonic, password, and wallet name, and saves it to the
device. The **CreateWalletFromPrivateKey** method creates a new wallet
with the specified private key and password, and returns the wallet
object.

The **RestoreAllWallets** method restores all wallets saved on the
device with the given password and adds them to the list of wallets. The
**DoesWalletWithMnemonicExists** method checks if a wallet with the
specified mnemonic exists by iterating through all saved wallets and
comparing their mnemonics. The **DoesWalletWithPublicKeyAlreadyExists**
method checks if a wallet with the specified public key already exists
by iterating through all saved wallets and comparing their public keys.

The **GetAllWallets** method returns a dictionary of all wallets managed
by the **WalletComponent**. The **GetWalletByIndex** method returns the
wallet at the specified index in the list of saved wallets. The
**GetWalletByName** method returns the wallet with the specified name.
The **GetWalletByPublicKey** method returns the wallet with the
specified public key.

The **RemoveWalletByName** method removes the wallet with the specified
name from the list of wallets and clears it from memory. The
**RemoveWalletByPublicKey** method removes the wallet with the specified
public key from the list of wallets and clears it from memory. The
**RemoveWallet** method removes the specified wallet from the list of
wallets and clears it from memory. The **RemoveAllWallets** method
removes all wallets from the list of wallets and clears the current
wallet and password.

Overall, this script provides a simple and efficient way to manage
cryptocurrency wallets in a Unity game, allowing for easy creation,
restoration, and removal of wallets, as well as retrieval of wallet
information.

Most used RPC methods are implemented and accessible from the
WalletComponent script as well as the websocket subscriptions for
currently active wallet.

**WalletComponent** manages RPC Client and websocket connection.

**Disclaimer**: Wallet currently uses shared SUI nodes and we cannot
guarantee their stability and availability. In the final product, we
recommend using a dedicated node.

**Wallet user interface**

If you are in need of extending wallet with new screen it is simple as:

1.  Creating user interface for the page

2.  Creating **MonoBehaviour** script that inherits **BaseScreen** class and attaching it to the parent of the UI.

```c#
public class ChangePassword : BaseScreen
{
    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
```

3.  Add a new screen to the list of screens in **SimpleScreenManager**.

4.  To go to the next page you simply call the **GoTo** method and pass the screen name. Screen name is the name of the game object that the new script is attached to.

```c#
    GoTo("ImportPrivateKeyScreen"); 
```

## Development

Want to contribute? Great!

To fix a bug or enhance an existing module, follow these steps:

- Fork the repo
- Create a new branch (`git checkout -b improve-feature`)
- Make the appropriate changes in the files
- Add changes to reflect the changes made
- Commit your changes (`git commit -am 'Improve feature'`)
- Push to the branch (`git push origin improve-feature`)
- Create a Pull Request

## License

This project is licensed under the Open Source License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

Packages used for this project:
1. https://www.bouncycastle.org/
2. https://github.com/somdoron/NaCl.net.git
3. https://github.com/endel/NativeWebSocket.git

