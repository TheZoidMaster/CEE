namespace CEE;
using System;
using CommandLine;

class Program
{
    static bool InstructionsLeft = true;
    static bool Decrypt = false;
    static byte[]? FileContent;
    static List<int> KeeInstructions = new List<int>();

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to encrypt/decrypt.")]
        public string? InputFile { get; set; }

        [Option('k', "kee", Required = true, HelpText = "KEE file to use.")]
        public string? KeeFile { get; set; }

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
        Decrypt = opts.Decrypt;

        if (File.Exists(opts.InputFile))
        {
            FileContent = File.ReadAllBytes(opts.InputFile);
        }
        else
        {
            Console.WriteLine("Input file does not exist.");
            return;
        }

        if (File.Exists(opts.KeeFile))
        {
            var keeContent = File.ReadAllBytes(opts.KeeFile);
            KeeInstructions = keeContent.Select(b => (int)b).ToList();
        }
        else
        {
            Console.WriteLine("Key file does not exist.");
            return;
        }

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

    static void ModifyFile(bool list, int[] value)
    {
        var newContent = new List<byte>();

        if (list && !Decrypt)
        {
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] + value[i]) % 0x100));
            }
        }
        else if (list && Decrypt)
        {
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] - value[i]) % 0x100));
            }
        }
        else if (!list && !Decrypt)
        {
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] + value[0]) % 0x100));
            }
        }
        else if (!list && Decrypt)
        {
            for (int i = 0; i < FileContent.Length; i++)
            {
                newContent.Add((byte)((FileContent[i] - value[0]) % 0x100));
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
        // TODO: Implement gradient
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

        while (InstructionsLeft)
        {
            instructions[KeeInstructions[0]]();
            int InstructionLength = length[KeeInstructions[0]];
            for (int i = 0; i < InstructionLength; i++)
            {
                KeeInstructions.RemoveAt(0);
            }
            if (KeeInstructions.Count == 0)
            {
                InstructionsLeft = false;
            }
        }
    }
}