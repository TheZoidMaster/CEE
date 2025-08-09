namespace CEE.Encryptor.Instructions;

using static CEE.Encryptor.Util;

class Gradient : InstructionBase
{
    public override byte[] Encrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        byte[] gradient = GenGradient(inputs, file.Length);
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] + gradient[i]);
        }
        return output;
    }

    public override byte[] Decrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        byte[] gradient = GenGradient(inputs, file.Length);
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] - gradient[i]);
        }
        return output;
    }
}