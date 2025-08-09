namespace CEE.Compiler;

using System.Text;
using System.Reflection;

class CEEDCompiler
{
    static readonly Dictionary<string, byte> Mappings = new Dictionary<string, byte> {
        { "add", 0x00 },
        { "sub", 0x01 },
        { "grd", 0x02 },
        { "lgr", 0x03 },
        { "lsh", 0x04 },
        { "rsh", 0x05 },
        { "not", 0x06 }
    };

    public static void Compile(string input, string? output)
    {
        var lines = File.ReadAllLines(input);
        var instructions = new List<InstructionToken>();

        foreach (var line in lines)
        {
            if ((line == "") | (line.StartsWith("#")))
            {
                continue;
            }
            var parts = line.Split(' ');
            if (Mappings.TryGetValue(parts[0], out var opcode))
            {
                var operands = new List<byte>();
                foreach (var part in parts.Skip(1))
                {
                    operands.Add(Convert.ToByte(part, 16));
                }
                instructions.Add(new InstructionToken(opcode, operands.ToArray()));
            }
            else
            {
                throw new Exception($"Unknown opcode \"{parts[0]}\" on line {line}");
            }
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