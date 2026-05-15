namespace Chicane.Mobile.Models;

/// <summary>
/// IDrawable for the live track map overlay.
/// Renders driver position dots on a transparent canvas overlaid on the circuit image.
/// </summary>
public class TrackMapDrawable : IDrawable
{
    public List<LiveLocationDot> Locations { get; set; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Locations.Count == 0) return;

        var w = dirtyRect.Width;
        var h = dirtyRect.Height;
        if (w <= 0 || h <= 0) return;

        var minX = Locations.Min(l => l.X);
        var maxX = Locations.Max(l => l.X);
        var minY = Locations.Min(l => l.Y);
        var maxY = Locations.Max(l => l.Y);

        var rangeX = maxX - minX;
        var rangeY = maxY - minY;
        if (rangeX <= 0) rangeX = 1;
        if (rangeY <= 0) rangeY = 1;

        // Scale to fit canvas with 10% padding
        var pad = 0.10;
        var scaleX = w * (1 - pad * 2) / rangeX;
        var scaleY = h * (1 - pad * 2) / rangeY;
        var scale = Math.Min(scaleX, scaleY);
        var offsetX = (w - rangeX * scale) / 2;
        var offsetY = (h - rangeY * scale) / 2;

        foreach (var loc in Locations)
        {
            var px = (float)((loc.X - minX) * scale + offsetX);
            var py = (float)((loc.Y - minY) * scale + offsetY);

            var color = Color.FromArgb(loc.Colour ?? "#FFFFFF");
            var teamAlpha = Color.FromArgb("#4D");
            var teamHex = loc.Colour ?? "333333";
            var alphaColor = Color.FromArgb($"4D{teamHex.TrimStart('#')}");

            // Outer glow
            canvas.FillColor = alphaColor;
            canvas.FillCircle(px, py, 14);

            // Colored dot
            canvas.FillColor = color;
            canvas.FillCircle(px, py, 9);

            // White border
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;
            canvas.DrawCircle(px, py, 9);

            // Driver code text
            canvas.FontColor = Colors.White;
            canvas.FontSize = 10;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.DrawString(loc.Code, px, py - 18, HorizontalAlignment.Center);
        }
    }
}

public class LiveLocationDot
{
    public string Code { get; set; } = "";
    public string Colour { get; set; } = "#333333";
    public int X { get; set; }
    public int Y { get; set; }
}
