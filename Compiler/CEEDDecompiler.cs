namespace CEE.Compiler;

using System.Text;

class CEEDDecompiler
{
    static readonly Dictionary<byte, string> ReverseMappings = new Dictionary<byte, string>
    {
        { 0x00, "ADD" },
        { 0x01, "SUB" },
        { 0x02, "GRD" },
        { 0x03, "LGD" },
        { 0x04, "LSH" },
        { 0x05, "RSH" },
        { 0x06, "NOT" }
    };

    public static void Decompile(string input, string? output)
    {
        var bytes = File.ReadAllBytes(input);
        int index = 0;

        if (bytes.Length < 6 || bytes[0] != 'C' || bytes[1] != 'E' || bytes[2] != 'E')
            throw new Exception("Not a valid CEE file");

        index = 3;

        byte major = bytes[index++];
        byte minor = bytes[index++];
        byte build = bytes[index++];

        var versionComment = $"# Detected CEE file version {major}.{minor}.{build}";

        var sb = new StringBuilder();
        sb.AppendLine(versionComment);

        while (index < bytes.Length)
        {
            var opcode = bytes[index++];
            if (!ReverseMappings.TryGetValue(opcode, out var mnemonic))
                throw new Exception($"Unknown opcode byte: 0x{opcode:X2}");

            var operandCount = bytes[index++];
            var operands = new byte[operandCount];
            Array.Copy(bytes, index, operands, 0, operandCount);
            index += operandCount;


            var operandsStr = string.Join(" ", operands.Select(b => $"0x{b:X2}"));
            sb.AppendLine($"{mnemonic} {operandsStr}".TrimEnd());
        }

        if (output == null)
        {
            output = Path.ChangeExtension(input, ".ceed");
        }

        File.WriteAllText(output, sb.ToString());
    }
}
