using System;
using System.Collections.Generic;
using Code_For_Nummi;
using Microsoft.Xna.Framework;
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
        }

        public bool CanAfford(int cost) => Coins >= cost;

        public bool TrySpend(int cost)
        {
            if (!CanAfford(cost)) return false;
            Coins -= cost;
            return true;
        }

        public void AddCoins(int amount) => Coins += amount;
    }

    public class Shop
    {
        private List<ShopItem> _stock = new();
        private CurrencySystem _currency;
        private BuildingSystem _buildingSystem;
    
        public IReadOnlyList<ShopItem> Stock => _stock;
        public bool IsOpen { get; private set; }
    
        public Shop(CurrencySystem currency, BuildingSystem buildingSystem)
        {
            _currency = currency;
            _buildingSystem = buildingSystem;
        }
    
        public void AddItem(ShopItem item) => _stock.Add(item);
        public void ClearStock() => _stock.Clear();
    
        public void Open() => IsOpen = true;
        public void Close() => IsOpen = false;
        public void Toggle() => IsOpen = !IsOpen;

        public bool TryBuy(ShopItem item)
        {
            if (!_currency.CanAfford(item.Cost)) return false;
            if (item.Building != null && !CanPlaceMoreBuilding(item.Building.Name)) return false;

            if (item.Building != null)
                _buildingSystem.SelectBuilding(item.Building);

            return true;
        }

        public bool CanPlaceMoreBuilding(string name) => _buildingSystem.CanPlaceMore(name);

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

        private const int PanelX = 50;
        private const int PanelY = 50;
        private const int PanelWidth = 320;
        private const int ItemHeight = 80;
        private const int Padding = 10;
        private const int HeaderHeight = 35;

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

            if (!GBL.LeftClick) return;

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

            int panelHeight = HeaderHeight + Padding + (_shop.Stock.Count * (ItemHeight + Padding));

            // Panel background
            DrawRect(new Rectangle(PanelX, PanelY, PanelWidth, panelHeight), Color.Black * 0.9f, 0.05f);

            // Header bar
            DrawRect(new Rectangle(PanelX, PanelY, PanelWidth, HeaderHeight), Color.DarkSlateGray * 0.95f, 0.049f);

            // Title
            GBL.spriteBatch.DrawString(_font, "SHOP",
                new Vector2(PanelX + Padding, PanelY + 8),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.048f);

            // Coin display
            string coinText = $"Coins: {_currency.Coins}";
            Vector2 coinSize = _font.MeasureString(coinText);

            // X close button - top right of header
            int closeSize = HeaderHeight - 8;
            _closeButtonRect = new Rectangle(
                PanelX + PanelWidth - closeSize - 4,
                PanelY + 4,
                closeSize,
                closeSize);

            GBL.spriteBatch.DrawString(_font, coinText,
                new Vector2(PanelX + PanelWidth - coinSize.X - closeSize - Padding - 8, PanelY + 8),
                Color.Gold, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.048f);

            MouseState mouse = Mouse.GetState();
            bool hoveringClose = _closeButtonRect.Contains(mouse.Position);

            // Button background - red on hover
            DrawRect(_closeButtonRect, hoveringClose ? Color.Red * 0.9f : Color.DarkRed * 0.7f, 0.047f);

            // X text centred in button
            Vector2 xSize = _font.MeasureString("X");
            GBL.spriteBatch.DrawString(_font, "X",
                new Vector2(
                    _closeButtonRect.X + (_closeButtonRect.Width - xSize.X) / 2f,
                    _closeButtonRect.Y + (_closeButtonRect.Height - xSize.Y) / 2f),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.046f);

            // Items
            for (int i = 0; i < _shop.Stock.Count; i++)
            {
                var item = _shop.Stock[i];
                int itemY = PanelY + HeaderHeight + Padding + i * (ItemHeight + Padding);
                Rectangle itemRect = new Rectangle(PanelX + Padding, itemY, PanelWidth - Padding * 2, ItemHeight);
                _itemRects.Add(itemRect);

                bool canAfford = _currency.CanAfford(item.Cost);
                bool canPlace = item.Building == null || _buildingSystem_CanPlaceMore(item);

                // Dark gray border
                Rectangle borderRect = new Rectangle(itemRect.X - 2, itemRect.Y - 2, itemRect.Width + 4, itemRect.Height + 4);
                DrawRect(borderRect, Color.DarkGray * 0.95f, 0.0471f);

                Color bgColor = (canAfford && canPlace) ? Color.LightGray * 0.85f : Color.Gray * 0.4f;
                DrawRect(itemRect, bgColor, 0.047f);

                if (item.Icon != null)
                {
                    GBL.spriteBatch.Draw(item.Icon,
                        new Rectangle(itemRect.X + 5, itemRect.Y + 10, 60, 60),
                        null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.046f);
                }

                GBL.spriteBatch.DrawString(_font, item.Name,
                    new Vector2(itemRect.X + 75, itemRect.Y + 8),
                    Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.046f);

                GBL.spriteBatch.DrawString(_font, item.Description,
                    new Vector2(itemRect.X + 75, itemRect.Y + 28),
                    Color.DarkGray, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.046f);

                Color costColor = canAfford ? Color.DarkGoldenrod : Color.DarkRed;
                GBL.spriteBatch.DrawString(_font, $"{item.Cost} coins",
                    new Vector2(itemRect.X + 75, itemRect.Y + 50),
                    costColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.046f);

                if (item.Building != null)
                {
                    int remaining = _currency_Remaining(item);
                    string remainText = remaining < 0 ? "Unlimited" : $"{remaining} left";
                    GBL.spriteBatch.DrawString(_font, remainText,
                        new Vector2(itemRect.X + PanelWidth - 100, itemRect.Y + 50),
                        Color.SteelBlue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.046f);
                }
            }
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
