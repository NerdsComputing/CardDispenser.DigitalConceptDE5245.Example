using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardDispenserDigitalConceptDe5245Driver.Internal;

namespace CardDispenserDigitalConceptDe5245Driver
{
    public class CardDispenserDe5245Driver : ICardDispenserDriver
    {
        private readonly List<byte> _readBuffer;
        private readonly CardDispenserStatus _status;
        private readonly SerialPort _serialPort;

        public CardDispenserDe5245Driver()
        {
            _readBuffer = new List<byte>();
            _status = new CardDispenserStatus();
            _serialPort = new SerialPort("default", 9600, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += PortDataReceived;
            _serialPort.ErrorReceived += PortErrorReceived;
        }

        public async Task Connect(string comPortName)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.PortName = comPortName;
                _serialPort.Open();

                await ExecuteCommand(CardDispenserDe5245Commands.GetStatus);
            }
            catch (System.Exception ex)
            {
                throw new CardDispenserException(ex);
            }
        }

        public void Disconnect()
        {
            try { _serialPort.Close(); }
            catch (System.Exception ex) { throw new CardDispenserException(ex); }
        }

        public async Task<CardDispenserStatus> GetStatus()
        {
            try
            {
                await ExecuteCommand(CardDispenserDe5245Commands.GetStatus);
                return new CardDispenserStatus
                {
                    Position = _status.Position,
                    IsExecutingCommand = _status.IsExecutingCommand,
                    FirmwareVersion = _status.FirmwareVersion,
                    IsHopperLowLevel = _status.IsHopperLowLevel
                };
            }
            catch (System.Exception ex)
            {
                throw new CardDispenserException(ex);
            }
        }

        public async Task<string> GetFirmwareVersion()
        {
            try { await ExecuteCommand(CardDispenserDe5245Commands.GetFirmwareVersion); return _status.FirmwareVersion; }
            catch (System.Exception ex) { throw new CardDispenserException(ex); }
        }

        public async Task DispenseCardInternally()
        {
            try { await ExecuteCommand(CardDispenserDe5245Commands.DispenseCardInternally); }
            catch (System.Exception ex) { throw new CardDispenserException(ex); }
        }

        public async Task PresentCard()
        {
            try { await ExecuteCommand(CardDispenserDe5245Commands.PresentCard); }
            catch (System.Exception ex) { throw new CardDispenserException(ex); }
        }

        private async Task ExecuteCommand(ICardDispenserCommand command)
        {
            _status.IsExecutingCommand = true;
            _serialPort.Write(command.Data, 0, command.DataLength);
            await WaitCommandExecution(command);
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var dataRead = _serialPort.ReadExisting();
            var bytesRead = Encoding.GetEncoding("UTF-8").GetBytes(dataRead.ToCharArray());
            HandleReadData(bytesRead);
        }

        private void PortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new CardDispenserException(e.EventType.ToString());
        }

        private async Task WaitCommandExecution(ICardDispenserCommand command)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (_status.IsExecutingCommand && stopWatch.ElapsedMilliseconds < command.MaxExecutionTime)
            {
                await Task.Delay(50);
            }
        }

        private void HandleReadData(byte[] data)
        {
            var dataWithoutInhibitedCharacters = data.Where(x => CardDispenserDe5245Commands.InhibitedCharacters.All(y => x != y)).ToList();
            _readBuffer.AddRange(dataWithoutInhibitedCharacters);

            if (!_readBuffer.Contains(CardDispenserDe5245Commands.PackageEndCharacter))
                return;

            if (_readBuffer.Count == 3 && 
                _readBuffer[0] == CardDispenserDe5245Commands.PackageStartCharacter && 
                _readBuffer[2] == CardDispenserDe5245Commands.PackageEndCharacter)
            {   
                _status.Position = (_readBuffer[1] & 0x03) == 0x01 ? Position.UnderAntenna : Position.Initial;
                _status.IsHopperLowLevel = (_readBuffer[1] & 0x04) == 0x04;
                _status.IsExecutingCommand = false;
            }
            else if (_readBuffer.Count > 3)
            {   
                _status.FirmwareVersion = Encoding.Default.GetString(_readBuffer.ToArray());
            }
            else
            {
                //Something is wrong in my implementation or there are leftovers that can be ignored
            }

            _readBuffer.Clear();
        }
    }
}
