# AllArt SUI Blockchain Wallet Implementation for Unity

![GitHub](https://img.shields.io/github/license/allartprotocol/allart-unity-sui-wallet)
![GitHub issues](https://img.shields.io/github/issues-raw/allartprotocol/allart-unity-sui-wallet)
![GitHub pull requests](https://img.shields.io/github/issues-pr/allartprotocol/allart-unity-sui-wallet)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/allartprotocol/allart-unity-sui-wallet)

Welcome to the AllArt SUI Blockchain Wallet Implementation for Unity. This Unity package provides a wallet implementation for the SUI Blockchain, enabling straightforward integration of the SUI Blockchain into your Unity projects.

## Table of Contents

- [Getting Started](#getting-started)
- [Usage](#usage)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- Unity Version 2020.3.7f1 or later.
- A SUI Blockchain Node that the Unity client can interact with.

### Installation

To install this package, follow these steps:

1. Open your Unity project.
2. Open the package manager `Window -> Package Manager`
3. Click on `+` and then `Add package from git URL...`
4. Enter `https://github.com/allartprotocol/allart-unity-sui-wallet.git`

Unity should download and import the package automatically.

## Usage

After package has been imported, import prefabs from samples by following these steps:

1. Go to Package Manager
2. Find AllArt Unity SUI Wallet in the list of packages added to your project.
3. Under Samples you will find Wallet Implementation sample.
4. Import it.
5. After that you can simply drag wallet prefab into your scene.

Wallet implementation has all the features you can expect from any wallet and is more than enough for kick start your project.
Most of the functionalities are easily accessible from the WalletComponent.cs. Even if you do not want to use provided interface you can create you implementation around it.

Wallets are saved localy by saving its mnemonic phrase encrypted with provided password. If you Want to add new wallet you can do that through interface or externaly:

```c#
WalletComponent.Instance.CreateWallet(this.mnemonic, password);

```

Restoring saved wallets:

```c#
string password = "1234";
WalletComponent.Instance.RestoreAllWallets(password);
Wallet wallet = WalletComponent.Instance.GetWalletByIndex(0);
```
Most used RPC methods are implemented and accessible from the WalletComponent script as well as the websocket subscriptios for currently active wallet.

If you are in need of extending wallet with new screen it is simple as:

1. Creating user interface for the page
2. Creating MonoBehaviour script that inherits BaseScreen class and attaching it to the parent of the UI.

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
   
3. Add new screen to the list of screens in SimpleScreenManager.
4. To go to the next page you simply call GoTo method and pass screen name. Screen name is the name of the game object that the new script is attached to.
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

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## License

This project is licensed under the Open Source License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

_To be filled_
