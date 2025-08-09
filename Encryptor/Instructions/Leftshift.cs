namespace CEE.Encryptor.Instructions;

using static CEE.Encryptor.Util;

class Leftshift : InstructionBase
{
    public override byte[] Encrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = RotateLeft(file[i], inputs[0]);
        }
        return output;
    }

    public override byte[] Decrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = RotateRight(file[i], inputs[0]);
        }
        return output;
    }
}