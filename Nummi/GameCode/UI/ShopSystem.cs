using System;
using System.Collections.Generic;
using Code_For_Nummi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class ShopItem
    {
        public string Name;
        public string Description;
        public Texture2D Icon;
        public int Cost;
        public BuildingType Building;

        public ShopItem(string name, string description, Texture2D icon, int cost, BuildingType building = null)
        {
            Name = name;
            Description = description;
            Icon = icon;
            Cost = cost;
            Building = building;
        }
    }

    // This class manages the player's currency and resources. It keeps track of coins, population, food, and energy. It also provides methods for checking if the player can afford something and for spending coins.
    // TODO: Make it so you can get resources at start of every tails side flip based on amount of buildings like food and energy go down based on poulation and up based on farms and nuclear reactors, population goes up based on houses and down based on lack of food or energy, etc. This will make the game more dynamic and give the player more things to manage.
    public class CurrencySystem
    {
        public int Coins { get; private set; }
        public int Population { get; private set; }
        public int Food { get; private set; }
        public int Energy { get; private set; }

        public CurrencySystem(int startingCoins = 0)
        {
            Coins = startingCoins;
            Population = 1;
            Food = 100;
            Energy = 100;
        }

        // Checks if the player has enough coins to afford a cost. This is used by the shop to determine if an item can be purchased.
        public bool CanAfford(int cost) => Coins >= cost;
        // Tries to spend coins. Returns true if the purchase was successful, false if the player couldn't afford it. This is used by the shop when an item is bought to deduct the cost from the player's coins.
        public bool TrySpend(int cost)
        {
            if (!CanAfford(cost)) return false;
            Coins -= cost;
            return true;
        }
        // Use these when adding to the currencys dont hard code it with the variables
        public void AddCoins(int amount) => Coins += amount;
        public void AddPopulation(int amount) => Population += amount;
        public void AddFood(int amount) => Food += amount;
        public void AddEnergy(int amount) => Energy += amount;

    }

    public class Shop
    {
        private List<ShopItem> _stock = new();
        private CurrencySystem _currency;
        private BuildingSystem _buildingSystem;
    
        // List for stock
        public IReadOnlyList<ShopItem> Stock => _stock;
        // This property tracks whether the shop is currently open or closed, which can be used by the UI to determine whether to display the shop interface and by the game logic to prevent purchases when the shop is closed.
        public bool IsOpen { get; private set; }
    
        public Shop(CurrencySystem currency, BuildingSystem buildingSystem)
        {
            _currency = currency;
            _buildingSystem = buildingSystem;
        }
        // Call this to add an item to the shop's stock. This is used during game setup to populate the shop with items for sale.
        public void AddItem(ShopItem item) => _stock.Add(item);
        // Call this to clear all items from the shop's stock, useful for resetting the shop or starting a new game.
        public void ClearStock() => _stock.Clear();
        // These methods control the shop's open/closed state. Open() sets IsOpen to true, allowing purchases to be made. Close() sets IsOpen to false, preventing purchases. Toggle() switches between open and closed states. This allows for flexible control of the shop's availability in the game.
        public void Open() => IsOpen = true;
        public void Close() => IsOpen = false;
        public void Toggle() => IsOpen = !IsOpen;
        // This method attempts to purchase an item from the shop. It checks if the player can afford the item and if they can place the associated building (if any). If both checks pass, it selects the building for placement and returns true. If either check fails, it returns false, preventing the purchase.
        public bool TryBuy(ShopItem item)
        {
            if (!_currency.CanAfford(item.Cost)) return false;
            if (item.Building != null && !CanPlaceMoreBuilding(item.Building.Name)) return false;

            if (item.Building != null)
                _buildingSystem.SelectBuilding(item.Building);

            return true;
        }
        // This literally just takes from the building systems canplacemore method
        public bool CanPlaceMoreBuilding(string name) => _buildingSystem.CanPlaceMore(name);
        // This method is used when the player tries to place a building after purchasing it. It checks if the player can afford the building and if they can place more of that building type. If both checks pass, it spends the coins and returns true, allowing the placement to proceed. If either check fails, it returns false, preventing the placement.
        public bool TryPayForPlacement(string buildingName)
        {
            // Find the item in stock by name
            foreach (var item in _stock)
            {
                if (item.Building != null && item.Building.Name == buildingName)
                {
                    if (!_currency.CanAfford(item.Cost)) return false;
                    _currency.TrySpend(item.Cost);
                    return true;
                }
            }
            return true;
        }
    }

    public class ShopUI
    {
        private Shop _shop;
        private CurrencySystem _currency;
        private SpriteFont _font;
        private Texture2D _pixel;

        private const int PanelX = 100;
        private const int PanelY = 50;
        private const int ItemWidth = 280; 
        private const int ItemHeight = 80;
        private const int Padding = 10;
        private const int HeaderHeight = 35;
        private const int ItemsPerColumn = 3;

        private List<Rectangle> _itemRects = new();

        public ShopUI(Shop shop, CurrencySystem currency, SpriteFont font)
        {
            _shop = shop;
            _currency = currency;
            _font = font;

            _pixel = new Texture2D(GBL.GD, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        private Rectangle _closeButtonRect;

        public void Update()
        {
            // Don't do anything if the shop isn't open
            if (!_shop.IsOpen) return;

            MouseState mouse = Mouse.GetState();

            // Close button
            if (_closeButtonRect.Contains(mouse.Position) && GBL.LeftClick)
            {
                _shop.Close();
                GBL.Game.buildingSystem.buildMode = false;
                GBL.Game.buildingSystem.selectedBuilding = null;
                return;
            }
            // Check item clicks
            if (!GBL.LeftClick) return;
            // Loop through item rectangles and check if any were clicked
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (i >= _shop.Stock.Count) break;
                if (_itemRects[i].Contains(mouse.Position))
                    _shop.TryBuy(_shop.Stock[i]);
            }
        }

        public void Draw()
        {
            if (!_shop.IsOpen) return;

            _itemRects.Clear();

            int columnCount = (int)Math.Ceiling((float)_shop.Stock.Count / ItemsPerColumn);
            int PanelWidth = Padding + columnCount * (ItemWidth + Padding);
            int panelHeight = HeaderHeight + Padding + (Math.Min(_shop.Stock.Count, ItemsPerColumn) * (ItemHeight + Padding));

            // Panel background
            DrawRect(new Rectangle(PanelX, PanelY, PanelWidth, panelHeight), Color.Black * 0.9f, 0.05f);

            // Header bar
            DrawRect(new Rectangle(PanelX, PanelY, PanelWidth, HeaderHeight), Color.DarkSlateGray * 0.95f, 0.049f);

            // Title
            GBL.spriteBatch.DrawString(_font, "SHOP",
                new Vector2(PanelX + Padding, PanelY + 8),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.048f);

            // Close button
            int closeSize = HeaderHeight - 8;
            _closeButtonRect = new Rectangle(
                PanelX + PanelWidth - closeSize - 4,
                PanelY + 4,
                closeSize,
                closeSize);

            MouseState mouse = Mouse.GetState();
            bool hoveringClose = _closeButtonRect.Contains(mouse.Position);
            DrawRect(_closeButtonRect, hoveringClose ? Color.Red * 0.9f : Color.DarkRed * 0.7f, 0.047f);

            Vector2 xSize = _font.MeasureString("X");
            GBL.spriteBatch.DrawString(_font, "X",
                new Vector2(
                    _closeButtonRect.X + (_closeButtonRect.Width - xSize.X) / 2f,
                    _closeButtonRect.Y + (_closeButtonRect.Height - xSize.Y) / 2f),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.046f);

            // Coin display
            string coinText = $"Coins: {_currency.Coins}";
            Vector2 coinSize = _font.MeasureString(coinText);
            GBL.spriteBatch.DrawString(_font, coinText,
                new Vector2(PanelX + PanelWidth - coinSize.X - closeSize - Padding - 8, PanelY + 8),
                Color.Gold, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.048f);

            // Items - laid out in columns
            for (int i = 0; i < _shop.Stock.Count; i++)
            {
                var item = _shop.Stock[i];

                // Calculate column and row from index
                int col = i / ItemsPerColumn;
                int row = i % ItemsPerColumn;

                int itemX = PanelX + Padding + col * (ItemWidth + Padding);
                int itemY = PanelY + HeaderHeight + Padding + row * (ItemHeight + Padding);

                Rectangle itemRect = new Rectangle(itemX, itemY, ItemWidth, ItemHeight);
                _itemRects.Add(itemRect);

                bool canAfford = _currency.CanAfford(item.Cost);
                bool canPlace = item.Building == null || _buildingSystem_CanPlaceMore(item);

                // Border
                Rectangle borderRect = new Rectangle(itemRect.X - 2, itemRect.Y - 2, itemRect.Width + 4, itemRect.Height + 4);
                DrawRect(borderRect, Color.DarkGray * 0.95f, 0.0471f);

                // Background
                Color bgColor = (canAfford && canPlace) ? Color.LightGray * 0.85f : Color.Gray * 0.4f;
                DrawRect(itemRect, bgColor, 0.047f);

                // Icon
                if (item.Icon != null)
                {
                    GBL.spriteBatch.Draw(item.Icon,
                        new Rectangle(itemRect.X + 5, itemRect.Y + 10, 60, 60),
                        null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.046f);
                }

                // Name
                GBL.spriteBatch.DrawString(_font, item.Name,
                    new Vector2(itemRect.X + 75, itemRect.Y + 8),
                    Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.046f);

                // Description
                GBL.spriteBatch.DrawString(_font, item.Description,
                    new Vector2(itemRect.X + 75, itemRect.Y + 28),
                    Color.DarkGray, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.046f);

                // Cost
                Color costColor = canAfford ? Color.DarkGoldenrod : Color.DarkRed;
                GBL.spriteBatch.DrawString(_font, $"{item.Cost} coins",
                    new Vector2(itemRect.X + 75, itemRect.Y + 50),
                    costColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.046f);

                // Remaining placements
                if (item.Building != null)
                {
                    int remaining = _currency_Remaining(item);
                    string remainText = remaining < 0 ? "Unlimited" : $"{remaining} left";
                    GBL.spriteBatch.DrawString(_font, remainText,
                        new Vector2(itemRect.X + ItemWidth - 70, itemRect.Y + 50),
                        Color.SteelBlue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.046f);
                }
            }
        
        }
    

        public void DrawCurrency(int x, int y, int currency, Texture2D txr, string currencyType)
        {
            int panelWidth = 100;
            int panelHeight = 24;

            // Background panel
            DrawRect(new Rectangle(x, y, panelWidth, panelHeight), Color.Black * 0.8f, 0.05f);
            DrawRect(new Rectangle(x, y, panelWidth, panelHeight), Color.DarkGoldenrod * 0.4f, 0.051f);

            // Coin icon placeholder - replace with your coin texture if you have one
            GBL.spriteBatch.Draw(txr,
                new Rectangle(x + 8, y + 5, 16, 16),
                null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.048f);

            // Coin amount
            string coinText = $"{currency}{currencyType}";
            GBL.spriteBatch.DrawString(_font, coinText,
                new Vector2(x + 28, y + 4),
                Color.Gold, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.047f);
        }

        private bool _buildingSystem_CanPlaceMore(ShopItem item) =>
            item.Building == null || GBL.Game.buildingSystem.CanPlaceMore(item.Building.Name);

        private int _currency_Remaining(ShopItem item) =>
            item.Building == null ? -1 : GBL.Game.buildingSystem.RemainingCount(item.Building.Name);

        private void DrawRect(Rectangle rect, Color color, float depth)
        {
            GBL.spriteBatch.Draw(_pixel, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }
    }

}
