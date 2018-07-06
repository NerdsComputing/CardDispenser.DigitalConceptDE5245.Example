namespace CardDispenserDigitalConceptDe5245Driver
{
    public class CardDispenserStatus
    {
        public bool IsHopperLowLevel { get; set; }
        public Position Position { get; set; }
        public string FirmwareVersion { get; set; }
        public bool IsExecutingCommand { get; set; }
    }

    public enum Position
    {
        Unknown,
        Initial,
        UnderAntenna
    }
}
