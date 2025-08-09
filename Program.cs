namespace CEE;

using CEE.Compiler;
using CEE.Encryptor;
using CommandLine;

class Program
{
    [Verb("compile", HelpText = "Compile a CEED file into a CEE file")]
    public class CompileOptions
    {
        [Value(0, MetaName = "input", Required = true, HelpText = "Input file to compile")]
        public required string Input { get; set; }

        [Value(1, MetaName = "output", HelpText = "Output file (will default to <input file name>.cee)")]
        public string? Output { get; set; }
    }

    [Verb("decompile", HelpText = "Decompile a CEE file into a CEED file")]
    public class DecompileOptions
    {
        [Value(0, MetaName = "input", Required = true, HelpText = "Input file to decompile")]
        public required string Input { get; set; }

        [Value(1, MetaName = "output", HelpText = "Output file (will default to <input file name>.ceed)")]
        public string? Output { get; set; }
    }

    [Verb("file", HelpText = "Encrypt or decrypt a single file using a CEE file")]
    public class FileEncryptionOptions
    {
        [Value(0, MetaName = "input", Required = true, HelpText = "Input file to encrypt or decrypt")]
        public required string Input { get; set; }

        [Value(1, MetaName = "key", Required = true, HelpText = "CEE file to encrypt or decrypt with")]
        public required string Key { get; set; }

        [Value(2, MetaName = "output", HelpText = "Output file (if not specified, the input file will be overwritten)")]
        public string? Output { get; set; }

        [Option('d', "decrypt", Default = false, HelpText = "Decrypt the input file instead of encrypting it")]
        public bool Decrypt { get; set; }
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CompileOptions, DecompileOptions, FileEncryptionOptions>(args)
            .WithParsed<CompileOptions>(Compile)
            .WithParsed<DecompileOptions>(Decompile)
            .WithParsed<FileEncryptionOptions>(FileEncryption);
    }

    static void Compile(CompileOptions options)
    {
        CEEDCompiler.Compile(options.Input, options.Output);
    }

    static void Decompile(DecompileOptions options)
    {
        CEEDDecompiler.Decompile(options.Input, options.Output);
    }

    static void FileEncryption(FileEncryptionOptions options)
    {
        FileEncryptor.EncryptFile(options.Input, options.Output ?? options.Input, options.Key, options.Decrypt);
    }
}