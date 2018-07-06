using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CardDispenserDigitalConceptDe5245Driver;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CardDispenserExample
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ICardDispenserDriver _cardDispenserDriver;

        private ObservableCollection<string> _logList;
        public ObservableCollection<string> LogList
        {
            get => _logList;
            set { _logList = value; OnPropertyChanged(nameof(LogList)); }
        }

        private string _cardDispenserPort;
        public string CardDispenserPort
        {
            get => _cardDispenserPort;
            set { _cardDispenserPort = value; OnPropertyChanged(nameof(CardDispenserPort)); }
        }

        private bool _isWaitingForCommands;
        public bool IsWaitingForCommands
        {
            get => _isWaitingForCommands;
            set { _isWaitingForCommands = value; OnPropertyChanged(nameof(IsWaitingForCommands)); }
        }

        private CardDispenserStatus _cardDispenserStatus;
        public CardDispenserStatus CardDispenserStatus
        {
            get => _cardDispenserStatus;
            set { _cardDispenserStatus = value; OnPropertyChanged(nameof(CardDispenserStatus)); }
        }


        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _cardDispenserDriver = new CardDispenserDe5245Driver();
            CardDispenserPort = "COM8";
            LogList = new ObservableCollection<string>();
            LogOperation("Application started...");
            IsWaitingForCommands = true;
        }

        public DelegateCommand ConnectCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;

            try
            {
                await _cardDispenserDriver.Connect(CardDispenserPort);
                await GetCardDispenserStatus();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation($"Connected to {CardDispenserPort}");
            IsWaitingForCommands = true;
        });

        public DelegateCommand DisconnectCommand => new DelegateCommand(() =>
        {
            IsWaitingForCommands = false;

            try
            {
                _cardDispenserDriver.Disconnect();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation($"Disconnected from {CardDispenserPort}");
            IsWaitingForCommands = true;
        });

        public DelegateCommand StatusCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;

            try
            {
                await _cardDispenserDriver.GetFirmwareVersion();
                await GetCardDispenserStatus();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation("Device status updated");
            IsWaitingForCommands = true;
        });

        public DelegateCommand FirmwareVersionCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;

            try
            {
                var firmwareVersion = await _cardDispenserDriver.GetFirmwareVersion();
                LogOperation($"Firmware version {firmwareVersion}");
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            IsWaitingForCommands = true;
        });

        public DelegateCommand CompleteSequenceCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;
            LogOperation("Executing complete sequence");

            try
            {
                await ExecuteCompleteSequence();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation("Executed complete sequence");
            IsWaitingForCommands = true;
        });

        public DelegateCommand DispenseCardInternallyCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;
            LogOperation("Executing dispense card internally");

            try
            {
                await _cardDispenserDriver.DispenseCardInternally();
                await GetCardDispenserStatus();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation("Executed dispense card internally");
            IsWaitingForCommands = true;
        });

        public DelegateCommand PresentCardCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;
            LogOperation("Executing present card");

            try
            {
                await _cardDispenserDriver.PresentCard();
                await GetCardDispenserStatus();
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation("Executed present card");
            IsWaitingForCommands = true;
        });

        public DelegateCommand Complete20SequencesCommand => new DelegateCommand(async () =>
        {
            IsWaitingForCommands = false;
            LogOperation("Executing 20 complete sequences");

            try
            {
                var currentCard = 0;
                while (currentCard < 20)
                {
                    await ExecuteCompleteSequence();
                    LogOperation($"Executed {++currentCard}/20 complete sequences");
                }
            }
            catch (Exception ex)
            {
                ShowMessageDialog("Error", ex.Message);
            }

            LogOperation("Executed 20 complete sequences");
            IsWaitingForCommands = true;
        });

        private async Task GetCardDispenserStatus()
        {
            CardDispenserStatus = await _cardDispenserDriver.GetStatus();
        }

        private async Task ExecuteCompleteSequence()
        {
            await _cardDispenserDriver.DispenseCardInternally();
            LogOperation("Executed dispense card internally");
            await GetCardDispenserStatus();
            await _cardDispenserDriver.PresentCard();
            LogOperation("Executed present card");
            await GetCardDispenserStatus();
        }

        private void LogOperation(string message)
        {
            LogList.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}>   {message}");
            OnPropertyChanged(nameof(LogList));
        }

        private void ShowMessageDialog(string title, string message)
        {
            _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }
}
