# InventorySystem
This is a repo I created for implementing an Inventory System in Unity C#.

Assets/Resources contains the Scriptable Object Instances of UI Items that are loaded before the scene starts. I have used Resources.Load() function to load the items in the shop. I also use the same folder to load items at runtime such as when the player needs to gather resources by selecting the "Gather Resources" button.

All the scripts are present in the Assets/Scripts folder.

Main Folder:
- GameService Handles input and also acts as the main Service Locator.

Services Folder:
- StorageService.cs handles the creation of ShopService and InventoryService
- ShopService.cs maintains data related to the items in the shop. It also handles other data such as the different UI panels for each filtered item type.
- InventoryService.cs does the same as ShopService except it contains other logic related to the amount of money the player owns as well as the weight of the Inventory.
- TransactionService.cs handles Buy and Sell Transactions.
- EventService handles all the events used in scripts(implemented for Observer Pattern).

UI folder:
- StorageUI.cs is a script that is dynamically instantiated as an object during runtime by both ShopService.cs and InventoryService.cs. This handles the data related to the UI of the items displayed. It takes in a panel and a list of items to be created in it's constructor declaration. Using this data, it fills the respective UI panel with items.
- UIService.cs handles all dynamic UI information that is displayed when the game is running.


Design Patterns Used:
- Service Locator
- Dependency Injection 
    - Used when I create StorageUI from ShopService and InventoryService.
    - Used when I create ShopService and InventoryService from StorageController

- Observer Pattern
    - Used to enable loosely-coupled interaction between UI_InfoManager and StorageController

Below is the Architecture Diagram:

![alt text](https://github.com/gaurdian2701/InventorySystem/blob/Feature5-CodeRefactoring/InventorySystemArch.jpg)

