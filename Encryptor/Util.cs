namespace CEE.Encryptor;

class Util
{
    public static byte[] GenLinearGradient(float start, float end, bool hasEnd, int length)
    {
        if (length == 0)
        {
            return Array.Empty<byte>();
        }
        if (length == 1)
        {
            return [(byte)start];
        }

        int div = hasEnd ? length - 1 : length;
        float step = (end - start) / div;
        byte[] result = new byte[length];

        for (int i = 0; i < length; i++)
        {
            float value = start + (step * i);
            result[i] = (byte)MathF.Round(value, MidpointRounding.AwayFromZero);
        }

        return result;
    }

    public static byte[] GenGradient(byte[] points, int length)
    {
        if (length == 0)
        {
            return Array.Empty<byte>();
        }
        else if (length == points.Length)
        {
            return (byte[])points.Clone();
        }
        else if (length < points.Length)
        {
            return GenLinearGradient(points[0], points[^1], true, length);
        }

        byte[] result = new byte[length];
        int index = 0;

        int remainder = length % (points.Length - 1);
        int stepLength = (length - remainder) / (points.Length - 1);

        for (int i = 1; i < points.Length; i++)
        {
            bool hasEnd = i == points.Length - 1;
            int stepCount;
            if (remainder != 0)
            {
                remainder--;
                stepCount = stepLength + 1;
            }
            else
            {
                stepCount = stepLength;
            }

            byte[] segment = GenLinearGradient(points[i - 1], points[i], hasEnd, stepCount);
            Array.Copy(segment, 0, result, index, segment.Length);
            index += segment.Length;
        }

        return result;
    }

    public static byte RotateLeft(byte value, byte amount)
    {
        amount %= 8;
        return (byte)(((value << amount) | (value >> (8 - amount))) & 0xFF);
    }

    public static byte RotateRight(byte value, byte amount)
    {
        amount %= 8;
        return (byte)(((value >> amount) | (value << (8 - amount))) & 0xFF);
    }
}