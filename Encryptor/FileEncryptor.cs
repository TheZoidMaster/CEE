namespace CEE.Encryptor;

using System.Reflection;
using CEE.Encryptor.Instructions;

class FileEncryptor
{
    static readonly Dictionary<byte, InstructionBase> Mappings = new Dictionary<byte, InstructionBase> {
        { 0x00, new Add() },
        { 0x01, new Sub() },
        { 0x02, new Gradient() },
        { 0x03, new LinearGradient() },
        { 0x04, new Leftshift() },
        { 0x05, new Rightshift() }
    };

    public static void EncryptFile(string inputPath, string outputPath, string keyPath, bool decrypt)
    {
        byte[] input = File.ReadAllBytes(inputPath);
        byte[] key = File.ReadAllBytes(keyPath);

        byte[] output = input;

        if (!(key.Length >= 3 && key[0] == 0x43 && key[1] == 0x45 && key[2] == 0x45))
        {
            Console.WriteLine("The specified key file is not a valid CEE key; aborting");
            Environment.Exit(0);
        }

        Version version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0");
        string versionString = $"{version.Major}.{version.Minor}.{version.Build}";
        string keyVersionString = $"{(int)key[3]}.{(int)key[4]}.{(int)key[5]}";

        if (!(versionString == keyVersionString))
        {
            Console.WriteLine($"The specified key's version ({keyVersionString}) does not match the current CEE version ({versionString}); aborting");
            Environment.Exit(0);
        }

        int i = 6;
        while (i < key.Length)
        {
            byte opcode = key[i++];
            byte operandCount = key[i++];
            byte[] operands = key.Skip(i).Take(operandCount).ToArray();
            i += operandCount;

            if (Mappings.TryGetValue(opcode, out var instruction))
            {
                if (!decrypt)
                {
                    output = instruction.Encrypt(output, operands);
                }
                else
                {
                    output = instruction.Decrypt(output, operands);
                }
            }
            else
            {
                Console.WriteLine($"Unknown opcode: {opcode:X2}; skipping");
            }
        }

        using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs)) { bw.Write(output); }
    }
}