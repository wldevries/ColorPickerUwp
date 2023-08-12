using System.IO;
using Windows.UI;

namespace ColorPickerShared;

public static class StreamExtensions
{
    public static void WriteBGRA(this Stream stream, Color color)
    {
        stream.WriteByte(color.B);
        stream.WriteByte(color.G);
        stream.WriteByte(color.R);
        stream.WriteByte(color.A);
    }
}
