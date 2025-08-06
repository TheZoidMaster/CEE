namespace CEE.Compiler;

class Instruction
{
    public Instruction(byte opcode)
    {
        Opcode = opcode;
    }

    public Instruction(byte opcode, byte[] operands)
    {
        Opcode = opcode;
        Operands = operands;
    }

    public byte Opcode { get; }
    public byte[]? Operands { get; }
}