using StreamingOffApp.Models;
using StreamingOffApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace StreamingOffApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IStreamingService _streamingService;
        private ObservableCollection<StreamingOffer> _offers;
        private StreamingOffer _selectedOffer;
        private CollectionViewSource _sortedOffersViewSource;
        private string _sortColumn = "PlatformName";
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private readonly Dictionary<string, string> _sortIndicators = new()
        {
            { "PlatformName", "↑" },
            { "Price", "" },
            { "PlanDays", "" },
            { "Status", "" },
            { "Description", "" }
        };

        public ObservableCollection<StreamingOffer> Offers
        {
            get => _offers;
            set
            {
                _offers = value;
                OnPropertyChanged();
                _sortedOffersViewSource.Source = _offers;
                ApplyDefaultSort();
            }
        }

        public StreamingOffer SelectedOffer
        {
            get => _selectedOffer;
            set
            {
                _selectedOffer = value;
                OnPropertyChanged();
                AddCommand?.RaiseCanExecuteChanged();
                UpdateCommand?.RaiseCanExecuteChanged();
                DeleteCommand?.RaiseCanExecuteChanged();
            }
        }

        public string SortColumn
        {
            get => _sortColumn;
            set
            {
                _sortColumn = value;
                UpdateSortIndicators();
                OnPropertyChanged();
            }
        }

        public ListSortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                UpdateSortIndicators();
                OnPropertyChanged();
            }
        }

        public string PlatformSortIndicator => _sortIndicators["PlatformName"];
        public string PriceSortIndicator => _sortIndicators["Price"];
        public string PlanDaysSortIndicator => _sortIndicators["PlanDays"];
        public string StatusSortIndicator => _sortIndicators["Status"];
        public string DescriptionSortIndicator => _sortIndicators["Description"];

        public ICollectionView SortedOffersView => _sortedOffersViewSource.View;

        public AsyncRelayCommand AddCommand { get; }
        public AsyncRelayCommand UpdateCommand { get; }
        public AsyncRelayCommand DeleteCommand { get; }
        public AsyncRelayCommand RefreshCommand { get; }
        public ICommand SortCommand { get; }

        public MainViewModel(IStreamingService streamingService)
        {
            _streamingService = streamingService ?? throw new ArgumentNullException(nameof(streamingService));
            _offers = new ObservableCollection<StreamingOffer>();
            _selectedOffer = new StreamingOffer();
            _sortedOffersViewSource = new CollectionViewSource { Source = _offers };

            AddCommand = new AsyncRelayCommand(Add, CanAdd);
            UpdateCommand = new AsyncRelayCommand(Update, CanUpdate);
            DeleteCommand = new AsyncRelayCommand(Delete, CanDelete);
            RefreshCommand = new AsyncRelayCommand(() => Refresh(true));
            SortCommand = new RelayCommand(Sort);

            InitializeAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSortIndicators()
        {
            foreach (var key in _sortIndicators.Keys.ToList())
            {
                _sortIndicators[key] = key == _sortColumn
                    ? _sortDirection == ListSortDirection.Ascending ? "↑" : "↓"
                    : "";
            }
            OnPropertyChanged(nameof(PlatformSortIndicator));
            OnPropertyChanged(nameof(PriceSortIndicator));
            OnPropertyChanged(nameof(PlanDaysSortIndicator));
            OnPropertyChanged(nameof(StatusSortIndicator));
            OnPropertyChanged(nameof(DescriptionSortIndicator));
        }

        private async void InitializeAsync()
        {
            try
            {
                await Refresh(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd inicjalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task Add()
        {
            try
            {
                if (SelectedOffer.HasErrors)
                {
                    MessageBox.Show("Niepoprawne dane w formularzu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _streamingService.AddOffer(SelectedOffer);
                var addedPlatformName = SelectedOffer.PlatformName;
                await Refresh(false);
                SelectedOffer = new StreamingOffer();
                MessageBox.Show($"Dodano ofertę {addedPlatformName}.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd dodawania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAdd() => SelectedOffer != null && !SelectedOffer.HasErrors;

        private async Task Update()
        {
            try
            {
                if (SelectedOffer == null || SelectedOffer.Id <= 0)
                {
                    MessageBox.Show("Wybierz ofertę do aktualizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _streamingService.UpdateOffer(SelectedOffer);
                var updatedPlatformName = SelectedOffer.PlatformName;
                await Refresh(false);
                SelectedOffer = Offers.FirstOrDefault(o => o.Id == SelectedOffer.Id) ?? new StreamingOffer();
                MessageBox.Show($"Zaktualizowano ofertę {updatedPlatformName}.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd aktualizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdate() => SelectedOffer != null && !SelectedOffer.HasErrors && SelectedOffer.Id > 0;

        private async Task Delete()
        {
            try
            {
                if (SelectedOffer == null || SelectedOffer.Id <= 0)
                {
                    MessageBox.Show("Wybierz ofertę do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var deletedPlatformName = SelectedOffer.PlatformName;
                await _streamingService.DeleteOffer(SelectedOffer.Id);
                await Refresh(false);
                SelectedOffer = new StreamingOffer();
                MessageBox.Show($"Usunięto ofertę {deletedPlatformName}.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd usuwania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete() => SelectedOffer != null && SelectedOffer.Id > 0;

        private async Task Refresh(bool showLoadMessage)
        {
            try
            {
                var offers = await _streamingService.GetAllOffers();
                Offers.Clear();
                foreach (var offer in offers ?? new List<StreamingOffer>())
                {
                    Offers.Add(offer);
                }

                if (SelectedOffer != null && SelectedOffer.Id > 0)
                {
                    SelectedOffer = Offers.FirstOrDefault(o => o.Id == SelectedOffer.Id) ?? new StreamingOffer();
                }
                else
                {
                    SelectedOffer = new StreamingOffer();
                }

                if (showLoadMessage)
                {
                    MessageBox.Show($"Załadowano {Offers.Count} ofert z bazy danych.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Offers.Clear();
                MessageBox.Show($"Błąd odświeżania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Sort(object parameter)
        {
            if (parameter is string newSortColumn && !string.IsNullOrEmpty(newSortColumn))
            {
                if (SortColumn == newSortColumn)
                {
                    SortDirection = SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                else
                {
                    SortColumn = newSortColumn;
                    SortDirection = ListSortDirection.Ascending;
                }

                _sortedOffersViewSource.SortDescriptions.Clear();
                _sortedOffersViewSource.SortDescriptions.Add(new SortDescription(SortColumn, SortDirection));
                _sortedOffersViewSource.View.Refresh();
            }
        }

        private void ApplyDefaultSort()
        {
            _sortedOffersViewSource.SortDescriptions.Clear();
            _sortedOffersViewSource.SortDescriptions.Add(new SortDescription("PlatformName", ListSortDirection.Ascending));
            SortColumn = "PlatformName";
            SortDirection = ListSortDirection.Ascending;
            _sortedOffersViewSource.View.Refresh();
            UpdateSortIndicators();
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
    }

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged;

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}