namespace CEE;
using System;
using CommandLine;

class Program
{
    static bool InstructionsLeft = true;
    static bool Decrypt = false;
    static byte[] FileContent = Array.Empty<byte>();
    static List<int> KeeInstructions = new List<int>();

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to encrypt/decrypt.")]
        public required string? InputFile { get; set; }

        [Option('k', "kee", Required = true, HelpText = "KEE file to use.")]
        public required string? KeeFile { get; set; }

        [Option('d', "decrypt", Default = false, HelpText = "Decrypt the input file.")]
        public bool Decrypt { get; set; }
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }

    static void RunOptions(Options opts)
    {
        if (!ValidateInputs(opts.InputFile, opts.KeeFile)) return;

        Decrypt = opts.Decrypt;
        FileContent = File.ReadAllBytes(opts.InputFile ?? throw new ArgumentNullException(nameof(opts.InputFile)));
        var keeContent = File.ReadAllBytes(opts.KeeFile ?? throw new ArgumentNullException(nameof(opts.KeeFile)));
        KeeInstructions = keeContent.Select(b => (int)b).ToList();

        if (FileContent is null)
        {
            Console.WriteLine("File content could not be loaded.");
            return;
        }

        ProcessInstructions();
        File.WriteAllBytes(opts.InputFile, FileContent);
    }

    static void HandleParseError(IEnumerable<Error> errors)
    {
        // me when i ignore errors :trol:
    }

    static bool ValidateInputs(string? inputFile, string? keeFile)
    {
        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Input file could not be found or does not exist.");
            return false;
        }

        if (!File.Exists(keeFile))
        {
            Console.WriteLine("Key file could not be found does not exist.");
            return false;
        }

        return true;
    }

    static void ModifyFile(bool list, int[] value)
    {
        var newContent = new List<byte>();
        int factor = Decrypt ? -1 : 1;

        if (list)
        {
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] + factor * value[i]) % 0x100));
            }
        }
        else
        {
            int singleValue = factor * value[0];
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] + singleValue) % 0x100));
            }
        }

        FileContent = newContent.ToArray();
    }

    static void LinearGradient()
    {
        int a = KeeInstructions[1], b = KeeInstructions[2];
        int totalSteps = FileContent.Length;

        int[] gradientAB = Enumerable.Range(0, totalSteps)
            .Select(i => a + (b - a) * i / (totalSteps - 1))
            .ToArray();

        ModifyFile(true, gradientAB);
    }

    static void Gradient()
    {
        Console.WriteLine("Gradient not yet implemented.\nAborting.");
        Environment.Exit(1);
    }


    static void Add()
    {
        ModifyFile(false, new int[] { KeeInstructions[1] });
    }

    static void ProcessInstructions()
    {
        var length = new Dictionary<int, int>
    {
        { 0x23, 3 },
        { 0x3f, 5 },
        { 0xe4, 2 }
    };

        var instructions = new Dictionary<int, Action>
    {
        { 0x23, LinearGradient },
        { 0x3f, Gradient },
        { 0xe4, Add }
    };

        int index = 0;
        while (index < KeeInstructions.Count)
        {
            int instruction = KeeInstructions[index];
            try
            {
                instructions[instruction]();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Unknown instruction 0x{instruction:X} at index {index}.\nAborting.");
                Environment.Exit(1);
            }
            index += length[instruction];
        }
    }
}