namespace darooyab_rewrite.Services.PharmacySpecialties;

public static class PharmacyImageFileValidator
{
    public const int MaximumFileSizeBytes = 50 * 1024;
    public const int MaximumWidth = 600;
    public const int MaximumHeight = 400;

    public static bool TryValidate(
        ReadOnlySpan<byte> content,
        string extension,
        string? contentType,
        out string error)
    {
        error = string.Empty;
        if (content.Length is 0 or > MaximumFileSizeBytes)
        {
            error = "حجم هر تصویر باید حداکثر ۵۰ کیلوبایت باشد.";
            return false;
        }

        var isJpeg = extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                     extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase);
        var isWebp = extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        if ((!isJpeg && !isWebp) ||
            (isJpeg && !string.Equals(contentType, "image/jpeg", StringComparison.OrdinalIgnoreCase)) ||
            (isWebp && !string.Equals(contentType, "image/webp", StringComparison.OrdinalIgnoreCase)))
        {
            error = "فقط تصویرهای JPG یا WebP معتبر پذیرفته می‌شوند.";
            return false;
        }

        var hasDimensions = isJpeg
            ? TryReadJpegDimensions(content, out var width, out var height)
            : TryReadWebpDimensions(content, out width, out height);
        if (!hasDimensions)
        {
            error = "فایل انتخاب‌شده تصویر JPG یا WebP معتبر نیست.";
            return false;
        }

        if (width > MaximumWidth || height > MaximumHeight)
        {
            error = "عرض تصویر نباید بیشتر از ۶۰۰ و ارتفاع آن نباید بیشتر از ۴۰۰ پیکسل باشد.";
            return false;
        }

        return true;
    }

    private static bool TryReadJpegDimensions(
        ReadOnlySpan<byte> content,
        out int width,
        out int height)
    {
        width = 0;
        height = 0;
        if (content.Length < 4 || content[0] != 0xff || content[1] != 0xd8)
        {
            return false;
        }

        var offset = 2;
        while (offset + 3 < content.Length)
        {
            if (content[offset++] != 0xff)
            {
                return false;
            }

            while (offset < content.Length && content[offset] == 0xff)
            {
                offset++;
            }

            if (offset >= content.Length)
            {
                return false;
            }

            var marker = content[offset++];
            if (marker is 0xd8 or 0xd9 or 0x01 or >= 0xd0 and <= 0xd7)
            {
                continue;
            }

            if (offset + 2 > content.Length)
            {
                return false;
            }

            var segmentLength = ReadBigEndianUInt16(content, offset);
            if (segmentLength < 2 || offset + segmentLength > content.Length)
            {
                return false;
            }

            if (marker is 0xc0 or 0xc1 or 0xc2 or 0xc3 or 0xc5 or 0xc6 or 0xc7 or
                0xc9 or 0xca or 0xcb or 0xcd or 0xce or 0xcf)
            {
                if (segmentLength < 7)
                {
                    return false;
                }

                height = ReadBigEndianUInt16(content, offset + 3);
                width = ReadBigEndianUInt16(content, offset + 5);
                return width > 0 && height > 0;
            }

            offset += segmentLength;
        }

        return false;
    }

    private static bool TryReadWebpDimensions(
        ReadOnlySpan<byte> content,
        out int width,
        out int height)
    {
        width = 0;
        height = 0;
        if (content.Length < 30 ||
            !content[..4].SequenceEqual("RIFF"u8) ||
            !content.Slice(8, 4).SequenceEqual("WEBP"u8))
        {
            return false;
        }

        var chunkType = content.Slice(12, 4);
        if (chunkType.SequenceEqual("VP8X"u8))
        {
            width = ReadLittleEndianUInt24(content, 24) + 1;
            height = ReadLittleEndianUInt24(content, 27) + 1;
            return true;
        }

        if (chunkType.SequenceEqual("VP8L"u8) && content[20] == 0x2f)
        {
            width = 1 + content[21] + ((content[22] & 0x3f) << 8);
            height = 1 + (content[22] >> 6) + (content[23] << 2) + ((content[24] & 0x0f) << 10);
            return true;
        }

        if (chunkType.SequenceEqual("VP8 "u8) &&
            content[23] == 0x9d &&
            content[24] == 0x01 &&
            content[25] == 0x2a)
        {
            width = ReadLittleEndianUInt16(content, 26) & 0x3fff;
            height = ReadLittleEndianUInt16(content, 28) & 0x3fff;
            return width > 0 && height > 0;
        }

        return false;
    }

    private static int ReadBigEndianUInt16(ReadOnlySpan<byte> content, int offset) =>
        (content[offset] << 8) | content[offset + 1];

    private static int ReadLittleEndianUInt16(ReadOnlySpan<byte> content, int offset) =>
        content[offset] | (content[offset + 1] << 8);

    private static int ReadLittleEndianUInt24(ReadOnlySpan<byte> content, int offset) =>
        content[offset] | (content[offset + 1] << 8) | (content[offset + 2] << 16);
}
