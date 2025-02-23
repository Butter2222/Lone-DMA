using eft_dma_radar.Tarkov.EFTPlayer;
using eft_dma_radar.Tarkov.Loot;
using eft_dma_radar.UI.Radar;
using eft_dma_shared.Common.Misc;
using eft_dma_shared.Common.Misc.Data;

namespace eft_dma_radar.UI.SKWidgetControl
{
    public sealed class LootInfoWidget : SKWidget
    {

        /// <summary>
        /// Constructs a Player Info Overlay.
        /// </summary>
        public LootInfoWidget(SKGLControl parent, SKRect location, bool minimized, float scale)
            : base(parent, "Loot Info", new SKPoint(location.Left, location.Top),
                new SKSize(location.Width, location.Height), scale, false)
        {
            Minimized = minimized;
            SetScaleFactor(scale);
        }

        /// <summary>
        /// All Filtered Loot on the map (Grouped by Position).
        /// </summary>
        /// <summary>
        private static IEnumerable<(LootItem, int)> Loot =>
            Memory.Loot?.FilteredLoot
                ?.Where(item => item is not LootCorpse && item is not QuestItem) // Remove corpses and quest items
                .GroupBy(item => item.Position) // Group items by their position
                .Select(group => (group.First(), group.Count())) // Take the first item in each group and count duplicates
                .OrderByDescending(entry => entry.Item1.Price) // ✅ Use Item1 to access LootItem
                ?? Enumerable.Empty<(LootItem, int)>(); // Ensure it's never null


        private LootItem selectedLoot;

        public void SelectLoot(LootItem item)
        {
            selectedLoot = item;
        }
        public LootItem GetSelectedLoot()
        {
            return selectedLoot;
        }

        internal static SKPaint TextPlayersOverlay { get; } = new()
        {
            SubpixelText = true,
            Color = SKColors.White,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Consolas"),
            FilterQuality = SKFilterQuality.High
        };

        public void Draw(SKCanvas canvas, Player localPlayer, SKPoint mousePosition, bool mouseClicked)
        {
            if (Minimized)
            {
                Draw(canvas);
                return;
            }

            var lootItems = Loot.ToList();
            var lootCount = lootItems.Count;
            var sb = new StringBuilder();

            // Table headers
            sb.AppendFormat("{0,-18}", "Name")
                .AppendFormat("{0,-8}", "Qty")
                .AppendFormat("{0,-8}", "Price")
                .AppendFormat("{0,-8}", "Dist")
                .AppendLine();

            var drawPt = new SKPoint(ClientRectangle.Left + 5, ClientRectangle.Top + 20);
            LootItem hoveredItem = null;

            float textHeight = TextPlayersOverlay.TextSize * 1.2f;

            foreach (var (item, count) in lootItems) 
            {
                string name = item.ShortName;
                string quantity = count.ToString(); 
                string price = item.Price.ToString("N0");
                float dist = Vector3.Distance(item.Position, localPlayer.Position);

                sb.AppendFormat("{0,-18}", name)
                    .AppendFormat("{0,-8}", quantity)
                    .AppendFormat("{0,-8}", price)
                    .AppendFormat("{0,-8:F1}", dist)
                    .AppendLine();

                // **Improve mouse hit detection**
                var itemRect = new SKRect(
                    drawPt.X, drawPt.Y, 
                    drawPt.X + 250, drawPt.Y + textHeight
                );

                if (itemRect.Contains(mousePosition))
                {
                    hoveredItem = item;

                    if (mouseClicked)
                    {
                        if (selectedLoot == item) 
                            selectedLoot = null;
                        else 
                            selectedLoot = item;
                    }
                }

                drawPt.Y += textHeight;
            }

            var data = sb.ToString().Split(Environment.NewLine);
            var lineSpacing = TextPlayersOverlay.FontSpacing;
            var maxLength = data.Max(x => TextPlayersOverlay.MeasureText(x));
            var pad = 2.5f * ScaleFactor;

            Size = new SKSize(maxLength + pad, data.Length * lineSpacing + pad);
            Location = Location;
            Draw(canvas);

            drawPt = new SKPoint(ClientRectangle.Left + pad, ClientRectangle.Top + lineSpacing / 2 + pad);
            canvas.DrawText($"Total Loot: {lootCount}", drawPt, TextPlayersOverlay);
            drawPt.Y += lineSpacing;

            foreach (var line in data)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var color = (hoveredItem != null && line.Contains(hoveredItem.ShortName)) ? SKColors.Yellow : SKColors.White;
                    var textPaint = new SKPaint
                    {
                        SubpixelText = TextPlayersOverlay.SubpixelText,
                        Color = color,
                        IsStroke = TextPlayersOverlay.IsStroke,
                        TextSize = TextPlayersOverlay.TextSize,
                        TextEncoding = TextPlayersOverlay.TextEncoding,
                        IsAntialias = TextPlayersOverlay.IsAntialias,
                        Typeface = TextPlayersOverlay.Typeface,
                        FilterQuality = TextPlayersOverlay.FilterQuality
                    };
                    canvas.DrawText(line, drawPt, textPaint);
                    drawPt.Y += textHeight;
                }
            }
        }
        public override void SetScaleFactor(float newScale)
        {
            base.SetScaleFactor(newScale);
            TextPlayersOverlay.TextSize = 12 * newScale;
        }
    }
}