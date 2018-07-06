namespace CardDispenserDigitalConceptDe5245Driver.Internal
{
    internal class CardDispenserCommand : ICardDispenserCommand
    {
        public byte[] Data { get; set; }
        public int DataLength => Data.Length;
        public int MaxExecutionTime { get; set; }
    }
}
