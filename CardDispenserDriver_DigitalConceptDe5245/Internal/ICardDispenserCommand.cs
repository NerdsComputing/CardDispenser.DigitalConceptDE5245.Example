namespace CardDispenserDigitalConceptDe5245Driver.Internal
{
    internal interface ICardDispenserCommand
    {
        byte[] Data { get; set; }
        int DataLength { get; }
        int MaxExecutionTime { get; set; }
    }
}