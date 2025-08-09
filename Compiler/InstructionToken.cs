namespace CEE.Compiler;

class InstructionToken
{
    public InstructionToken(byte opcode)
    {
        Opcode = opcode;
    }

    public InstructionToken(byte opcode, byte[] operands)
    {
        Opcode = opcode;
        Operands = operands;
    }

    public byte Opcode { get; }
    public byte[]? Operands { get; }
}