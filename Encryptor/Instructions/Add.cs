namespace CEE.Encryptor.Instructions;

class Add : InstructionBase
{
    public override byte[] Encrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] + inputs[0]);
        }
        return output;
    }

    public override byte[] Decrypt(byte[] file, byte[] inputs)
    {
        byte[] output = new byte[file.Length];
        for (int i = 0; i < file.Length; i++)
        {
            output[i] = (byte)(file[i] - inputs[0]);
        }
        return output;
    }
}