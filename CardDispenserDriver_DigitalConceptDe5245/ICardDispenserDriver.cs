using System.Threading.Tasks;

namespace CardDispenserDigitalConceptDe5245Driver
{
    public interface ICardDispenserDriver
    {
        Task Connect(string comPortName);
        void Disconnect();
        Task<CardDispenserStatus> GetStatus();
        Task<string> GetFirmwareVersion();
        Task DispenseCardInternally();
        Task PresentCard();
    }
}