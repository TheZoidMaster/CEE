namespace CEE.Encryptor.Instructions;

abstract class InstructionBase
{
    public abstract byte[] Encrypt(byte[] file, byte[] inputs);
    public abstract byte[] Decrypt(byte[] file, byte[] inputs);
}