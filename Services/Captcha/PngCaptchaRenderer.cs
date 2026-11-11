using System.Buffers.Binary;
using System.IO.Compression;
using System.Security.Cryptography;

namespace darooyab_rewrite.Services.Captcha;

internal sealed class PngCaptchaRenderer
{
    private const int Channels = 3;
    private static readonly IReadOnlyDictionary<char, string[]> Glyphs =
        new Dictionary<char, string[]>
        {
            ['2'] = ["01110", "10001", "00001", "00010", "00100", "01000", "11111"],
            ['3'] = ["11110", "00001", "00001", "01110", "00001", "00001", "11110"],
            ['4'] = ["00010", "00110", "01010", "10010", "11111", "00010", "00010"],
            ['5'] = ["11111", "10000", "10000", "11110", "00001", "00001", "11110"],
            ['6'] = ["01110", "10000", "10000", "11110", "10001", "10001", "01110"],
            ['7'] = ["11111", "00001", "00010", "00100", "01000", "01000", "01000"],
            ['8'] = ["01110", "10001", "10001", "01110", "10001", "10001", "01110"],
            ['9'] = ["01110", "10001", "10001", "01111", "00001", "00001", "01110"],
            ['A'] = ["01110", "10001", "10001", "11111", "10001", "10001", "10001"],
            ['B'] = ["11110", "10001", "10001", "11110", "10001", "10001", "11110"],
            ['C'] = ["01111", "10000", "10000", "10000", "10000", "10000", "01111"],
            ['D'] = ["11110", "10001", "10001", "10001", "10001", "10001", "11110"],
            ['E'] = ["11111", "10000", "10000", "11110", "10000", "10000", "11111"],
            ['F'] = ["11111", "10000", "10000", "11110", "10000", "10000", "10000"],
            ['G'] = ["01111", "10000", "10000", "10111", "10001", "10001", "01111"],
            ['H'] = ["10001", "10001", "10001", "11111", "10001", "10001", "10001"],
            ['J'] = ["00111", "00010", "00010", "00010", "10010", "10010", "01100"],
            ['K'] = ["10001", "10010", "10100", "11000", "10100", "10010", "10001"],
            ['L'] = ["10000", "10000", "10000", "10000", "10000", "10000", "11111"],
            ['M'] = ["10001", "11011", "10101", "10101", "10001", "10001", "10001"],
            ['N'] = ["10001", "11001", "11001", "10101", "10011", "10011", "10001"],
            ['P'] = ["11110", "10001", "10001", "11110", "10000", "10000", "10000"],
            ['Q'] = ["01110", "10001", "10001", "10001", "10101", "10010", "01101"],
            ['R'] = ["11110", "10001", "10001", "11110", "10100", "10010", "10001"],
            ['S'] = ["01111", "10000", "10000", "01110", "00001", "00001", "11110"],
            ['T'] = ["11111", "00100", "00100", "00100", "00100", "00100", "00100"],
            ['U'] = ["10001", "10001", "10001", "10001", "10001", "10001", "01110"],
            ['V'] = ["10001", "10001", "10001", "10001", "10001", "01010", "00100"],
            ['W'] = ["10001", "10001", "10001", "10101", "10101", "11011", "10001"],
            ['X'] = ["10001", "10001", "01010", "00100", "01010", "10001", "10001"],
            ['Y'] = ["10001", "10001", "01010", "00100", "00100", "00100", "00100"],
            ['Z'] = ["11111", "00001", "00010", "00100", "01000", "10000", "11111"]
        };

    public byte[] Render(string answer, CaptchaOptions options)
    {
        var width = options.ImageWidth;
        var height = options.ImageHeight;
        var pixels = new byte[width * height * Channels];
        Fill(pixels, [247, 250, 252]);

        for (var index = 0; index < options.NoisePixelCount; index++)
        {
            var color = RandomColor(170, 225);
            SetPixel(
                pixels,
                width,
                height,
                RandomNumberGenerator.GetInt32(width),
                RandomNumberGenerator.GetInt32(height),
                color);
        }

        for (var index = 0; index < options.InterferenceLineCount; index++)
        {
            DrawLine(
                pixels,
                width,
                height,
                RandomNumberGenerator.GetInt32(width),
                RandomNumberGenerator.GetInt32(height),
                RandomNumberGenerator.GetInt32(width),
                RandomNumberGenerator.GetInt32(height),
                RandomColor(100, 190));
        }

        var scale = Math.Max(3, Math.Min(5, (height - 16) / 7));
        var glyphWidth = 5 * scale;
        var glyphSpacing = 5;
        var totalWidth = answer.Length * glyphWidth + (answer.Length - 1) * glyphSpacing;
        var startX = Math.Max(2, (width - totalWidth) / 2);

        for (var index = 0; index < answer.Length; index++)
        {
            DrawGlyph(
                pixels,
                width,
                height,
                answer[index],
                startX + index * (glyphWidth + glyphSpacing),
                (height - 7 * scale) / 2,
                scale,
                RandomNumberGenerator.GetInt32(-2, 3),
                (RandomNumberGenerator.GetInt32(37) - 18) * Math.PI / 180,
                RandomColor(35, 115));
        }

        return EncodePng(pixels, width, height);
    }

    private static void DrawGlyph(
        byte[] pixels,
        int width,
        int height,
        char character,
        int x,
        int y,
        int scale,
        int verticalOffset,
        double angle,
        byte[] color)
    {
        var cosine = Math.Cos(angle);
        var sine = Math.Sin(angle);
        var centerX = 2.5 * scale;
        var centerY = 3.5 * scale;
        foreach (var (row, rowIndex) in Glyphs[character].Select((pattern, index) => (pattern, index)))
        {
            for (var column = 0; column < row.Length; column++)
            {
                if (row[column] != '1')
                {
                    continue;
                }

                for (var pixelY = 0; pixelY < scale; pixelY++)
                {
                    for (var pixelX = 0; pixelX < scale; pixelX++)
                    {
                        var sourceX = column * scale + pixelX - centerX;
                        var sourceY = rowIndex * scale + pixelY - centerY;
                        var rotatedX = (int)Math.Round(sourceX * cosine - sourceY * sine + centerX);
                        var rotatedY = (int)Math.Round(sourceX * sine + sourceY * cosine + centerY);
                        SetPixel(
                            pixels,
                            width,
                            height,
                            x + rotatedX,
                            y + rotatedY + verticalOffset,
                            color);
                    }
                }
            }
        }
    }

    private static byte[] EncodePng(byte[] pixels, int width, int height)
    {
        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
        {
            var rowLength = width * Channels;
            var row = new byte[rowLength + 1];
            for (var y = 0; y < height; y++)
            {
                row[0] = 0;
                Buffer.BlockCopy(pixels, y * rowLength, row, 1, rowLength);
                zlib.Write(row);
            }
        }

        using var output = new MemoryStream();
        output.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], height);
        header[8] = 8;
        header[9] = 2;
        header[10] = 0;
        header[11] = 0;
        header[12] = 0;
        WriteChunk(output, "IHDR", header);
        WriteChunk(output, "IDAT", compressed.ToArray());
        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        output.Write(length);

        var typeBytes = System.Text.Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        var crcData = new byte[typeBytes.Length + data.Length];
        Buffer.BlockCopy(typeBytes, 0, crcData, 0, typeBytes.Length);
        data.CopyTo(crcData.AsSpan(typeBytes.Length));
        Span<byte> crc = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crc, ComputeCrc32(crcData));
        output.Write(crc);
    }

    private static uint ComputeCrc32(ReadOnlySpan<byte> data)
    {
        var crc = uint.MaxValue;
        foreach (var value in data)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) == 1 ? (crc >> 1) ^ 0xedb88320u : crc >> 1;
            }
        }

        return ~crc;
    }

    private static void Fill(byte[] pixels, byte[] color)
    {
        for (var index = 0; index < pixels.Length; index += Channels)
        {
            pixels[index] = color[0];
            pixels[index + 1] = color[1];
            pixels[index + 2] = color[2];
        }
    }

    private static void DrawLine(
        byte[] pixels,
        int width,
        int height,
        int x0,
        int y0,
        int x1,
        int y1,
        byte[] color)
    {
        var dx = Math.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Math.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var error = dx + dy;

        while (true)
        {
            SetPixel(pixels, width, height, x0, y0, color);
            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            var doubledError = 2 * error;
            if (doubledError >= dy)
            {
                error += dy;
                x0 += sx;
            }

            if (doubledError <= dx)
            {
                error += dx;
                y0 += sy;
            }
        }
    }

    private static void SetPixel(
        byte[] pixels,
        int width,
        int height,
        int x,
        int y,
        byte[] color)
    {
        if ((uint)x >= (uint)width || (uint)y >= (uint)height)
        {
            return;
        }

        var offset = (y * width + x) * Channels;
        pixels[offset] = color[0];
        pixels[offset + 1] = color[1];
        pixels[offset + 2] = color[2];
    }

    private static byte[] RandomColor(int minimum, int maximum)
    {
        return
        [
            (byte)RandomNumberGenerator.GetInt32(minimum, maximum),
            (byte)RandomNumberGenerator.GetInt32(minimum, maximum),
            (byte)RandomNumberGenerator.GetInt32(minimum, maximum)
        ];
    }
}
