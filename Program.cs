namespace CEE;

using System;
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

    [Verb("folder", HelpText = "Encrypt or decrypt a folder using a CEE file")]
    public class FolderEncryptionOptions
    {
        [Value(0, MetaName = "input", Required = true, HelpText = "Input folder to encrypt or decrypt")]
        public required string Input { get; set; }

        [Value(1, MetaName = "key", Required = true, HelpText = "CEE file to encrypt or decrypt with")]
        public required string Key { get; set; }

        [Value(2, MetaName = "output", HelpText = "Output folder (if not specified, the input folder will be overwritten)")]
        public string? Output { get; set; }

        [Option('d', "decrypt", Default = false, HelpText = "Decrypt the input folder instead of encrypting it")]
        public bool Decrypt { get; set; }
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CompileOptions, FileEncryptionOptions, FolderEncryptionOptions>(args)
            .WithParsed<CompileOptions>(Compile)
            .WithParsed<FileEncryptionOptions>(FileEncryption)
            .WithParsed<FolderEncryptionOptions>(FolderEncryption);
    }

    static void Compile(CompileOptions options)
    {
        CEEDCompiler.Compile(options.Input, options.Output);
    }

    static void FileEncryption(FileEncryptionOptions options)
    {
        FileEncryptor.EncryptFile(options.Input, options.Output ?? options.Input, options.Key, options.Decrypt);
    }

    static void FolderEncryption(FolderEncryptionOptions options)
    {
        Console.WriteLine($"Input: {options.Input}");
        Console.WriteLine($"Output: {options.Output ?? "<overwrite>"}");
        Console.WriteLine($"Key: {options.Key}");
    }
}