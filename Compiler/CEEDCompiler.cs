namespace CEE.Compiler;

using System.Text;
using System.Reflection;

class CEEDCompiler
{
    static readonly Dictionary<string, byte> Mappings = new Dictionary<string, byte> {
        { "add", 0x00 },
        { "sub", 0x01 },
        { "grd", 0x02 },
        { "lgd", 0x03 },
        { "lsh", 0x04 },
        { "rsh", 0x05 },
        { "not", 0x06 }
    };

    static readonly Random rnd = new Random();
    static readonly Dictionary<string, Func<string?, byte>> specialTokens = new Dictionary<string, Func<string?, byte>>
    {
        ["RND"] = arg =>
        {
            if (arg == null)
                return (byte)rnd.Next(0, 256);
            if (byte.TryParse(arg, out var max))
                return (byte)(rnd.Next(0, max + 1) % 256);
            throw new Exception($"Invalid argument to RND: {arg}");
        },
        ["UTC"] = arg =>
        {
            return (byte)(DateTime.UtcNow.Hour % 256);
        },
        ["DEC"] = arg =>
        {
            if (arg == null)
                throw new Exception("DEC requires an argument");
            if (byte.TryParse(arg, out var val))
                return val;
            throw new Exception($"Invalid DEC argument: {arg}");
        },
        ["BIN"] = arg =>
        {
            if (arg == null)
                throw new Exception("BIN requires an argument");
            try
            {
                return Convert.ToByte(arg, 2);
            }
            catch
            {
                throw new Exception($"Invalid BIN argument: {arg}");
            }
        }
    };

    private static byte ParseOperand(string part)
    {
        var openParen = part.IndexOf('(');
        if (openParen != -1 && part.EndsWith(")"))
        {
            var token = part.Substring(0, openParen);
            var argStr = part.Substring(openParen + 1, part.Length - openParen - 2);
            if (specialTokens.TryGetValue(token, out var func))
            {
                return func(argStr);
            }
        }
        else
        {
            if (specialTokens.TryGetValue(part, out var func))
            {
                return func(null);
            }
        }

        if (part.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            part = part.Substring(2);

        if (byte.TryParse(part, System.Globalization.NumberStyles.HexNumber, null, out var val))
            return val;

        throw new Exception($"Invalid operand \"{part}\"");
    }

    public static void Compile(string input, string? output)
    {
        var lines = File.ReadAllLines(input);
        var instructions = new List<InstructionToken>();

        foreach (var line in lines)
        {
            var codeLine = line.Split('#')[0].Trim();
            if (string.IsNullOrEmpty(codeLine))
                continue;

            var parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            if (!Mappings.TryGetValue(parts[0], out var opcode))
            {
                throw new Exception($"Unknown opcode \"{parts[0]}\" on line: {line}");
            }

            var operands = new List<byte>();

            foreach (var part in parts.Skip(1))
            {
                operands.Add(ParseOperand(part));
            }

            instructions.Add(new InstructionToken(opcode, operands.ToArray()));
        }

        if (output == null)
        {
            output = Path.ChangeExtension(input, ".cee");
        }
        using (var fs = new FileStream(output, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(new ASCIIEncoding().GetBytes("CEE"));

            Version version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0");
            byte[] versionCode = [(byte)version.Major, (byte)version.Minor, (byte)version.Build];

            bw.Write(versionCode);

            foreach (var instruction in instructions)
            {
                bw.Write(instruction.Opcode);
                bw.Write((byte)(instruction.Operands?.Length ?? 0));
                if (instruction.Operands != null)
                {
                    bw.Write(instruction.Operands);
                }
            }
        }
    }
}