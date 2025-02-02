using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClassPlanner.Extensions;

public static class ColorGenerator
{
    private static readonly List<string> Colors =
    [
      "#394146", "#107c10", "#eaa300", "#c239b3", "#c19c00",
      "#004e8c", "#750b1c", "#b146c2", "#13a10e", "#0027b4",
      "#fde300", "#5c2e91", "#ff8c00", "#498205", "#0b6a0b",
      "#bf0077", "#0078d4", "#7160e8", "#da3b01", "#881798",
      "#c50f1f", "#00cc6a", "#d13438", "#7a7574", "#e43ba6",
      "#00b7c3", "#8e562e", "#77004d", "#4f6bed", "#a4262c",
      "#ffb900", "#005e50", "#bad80a", "#b4009e", "#8c8c8c",
      "#e3008c", "#adadad", "#008272", "#32145a", "#ff6262",
      "#ff00ff", "#ff4500", "#1e90ff", "#32cd32", "#ff1493"
    ];


    public static SolidColorBrush GetBrush(string key)
    {
        if (string.IsNullOrEmpty(key))
            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);

        int hash = Math.Abs(GetStableHash(key));
        int index = hash % Colors.Count;
        string hex = Colors[index];

        return new SolidColorBrush(GetColorFromHex(hex));
    }

    private static Windows.UI.Color GetColorFromHex(string hex)
    {
        hex = hex.TrimStart('#');
        byte r = Convert.ToByte(hex[..2], 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return Windows.UI.Color.FromArgb(255, r, g, b);
    }

    public static int GetStableHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return 0;
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        return BitConverter.ToInt32(hashBytes, 0);
    }
}
