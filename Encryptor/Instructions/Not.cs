namespace CEE.Encryptor.Instructions;

class Not : InstructionBase
{
    public override byte[] Encrypt(byte[] file, byte[] _)
    {
        byte[] output = new byte[file.Length];
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)~file[i];
        }
        return output;
    }

    public override byte[] Decrypt(byte[] file, byte[] _)
    {
        return Encrypt(file, _);
    }
}