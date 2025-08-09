namespace CEE.Encryptor.Instructions;

using static CEE.Encryptor.Util;

class LinearGradient : InstructionBase
{
    public override byte[] Encrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        Console.WriteLine($"lgr {inputs[0]}-{inputs[1]} with length {file.Length}");
        byte[] gradient = GenLinearGradient(inputs[0], inputs[1], true, file.Length);
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] + gradient[i]);
        }
        return output;
    }

    public override byte[] Decrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        byte[] gradient = GenLinearGradient(inputs[0], inputs[1], true, file.Length);
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] - gradient[i]);
        }
        return output;
    }
}