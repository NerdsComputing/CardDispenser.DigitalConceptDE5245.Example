using System.Collections.Generic;

namespace CardDispenserDigitalConceptDe5245Driver.Internal
{
    internal static class CardDispenserDe5245Commands
    {
        public static readonly byte PackageStartCharacter = 0x21;
        public static readonly byte PackageEndCharacter = 0x0D;
        public static readonly List<byte> InhibitedCharacters = new List<byte> { 0x00, 0x0A };

        public static readonly ICardDispenserCommand GetStatus = new CardDispenserCommand { Data = new byte[] { 0x21, 0x41, 0x0D }, MaxExecutionTime = 100 };
        public static readonly ICardDispenserCommand DispenseCardInternally = new CardDispenserCommand { Data = new byte[] { 0x21, 0x42, 0x0D }, MaxExecutionTime = 5000 };
        public static readonly ICardDispenserCommand PresentCard = new CardDispenserCommand { Data = new byte[] { 0x21, 0x43, 0x0D }, MaxExecutionTime = 5000 };
        public static readonly ICardDispenserCommand GetFirmwareVersion = new CardDispenserCommand { Data = new byte[] { 0x21, 0x56, 0x0D }, MaxExecutionTime = 100 };
    }
}
