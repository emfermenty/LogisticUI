using LogisticWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LogisticWPF.Command;
using LogisticWPF.Models.Enums;
using LogisticWPF.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogisticWPF.DTO;
using LogisticWPF;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public ObservableCollection<Shipping> ActiveShippings { get; set; }
    public ObservableCollection<ShippingQuote> Quotes { get; set; }

    private Shipping _selectedShipping;
    public Shipping SelectedShipping
    {
        get => _selectedShipping;
        set
        {
            _selectedShipping = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedShippingInfo));
            RaiseCommandStates();
        }
    }

    public string SelectedShippingInfo =>
        SelectedShipping == null
            ? "Выберите перевозку слева"
            : $"Информация о: {SelectedShipping.TrackingNumber}\n" +
              $"Начало перевозки: {SelectedShipping.StartShipping:dd.MM.yyyy HH:mm}\n" +
              $"Примерная длительность: {SelectedShipping.Duration}\n" +
              $"Дистанция: {SelectedShipping.Distance}\n" +
              $"Вес перевозки: {SelectedShipping.Weight}\n" +
              $"Обьем перевозки: {SelectedShipping.Volume}\n" +
              $"Тип перевозки: {SelectedShipping.TypeDescription}\n" +
              $"Стоимость перевозки: {SelectedShipping.Cost}\n\n" +
              $"Информация о Транспорте: {SelectedShipping.Vehicle.Model}\n" +
              $"Максимальный вес: {SelectedShipping.Vehicle.MaxWeight}\n" +
              $"Максимальный объем: {SelectedShipping.Vehicle.MaxVolume}\n" +
              $"Максимальная скорость: {SelectedShipping.Vehicle.Speed}\n" +
              $"Потребление топлива {SelectedShipping.Vehicle.FuelConsumption}\n\n" +
              $"Статус перевозки: {SelectedShipping.Status}";

    // Ввод для новых котировок
    private double _inputDistance;
    public double InputDistance { get => _inputDistance; set { _inputDistance = value; OnPropertyChanged(); } }

    private double _inputWeight;
    public double InputWeight { get => _inputWeight; set { _inputWeight = value; OnPropertyChanged(); } }

    private double _inputVolume;
    public double InputVolume { get => _inputVolume; set { _inputVolume = value; OnPropertyChanged(); } }

    private ShippingQuote _optimizedQuote;
    public ShippingQuote OptimizedQuote { get => _optimizedQuote; set { _optimizedQuote = value; OnPropertyChanged(); } }

    public ICommand CreateShippingCommand { get; }
    public ICommand SelectShippingCommand { get; }
    public ICommand AcceptCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand CancelCommand { get; }

    public ICommand GetQuotesCommand { get; }
    public ICommand OptimizeQuoteCommand { get; }

    public MainViewModel()
    {
        _apiService = new ApiService();

        ActiveShippings = new ObservableCollection<Shipping>();
        Quotes = new ObservableCollection<ShippingQuote>();

        LoadActiveShippings();

        CreateShippingCommand = new RelayCommand(CreateShipping);
        SelectShippingCommand = new RelayCommand<Shipping>(SelectShipping);
        AcceptCommand = new RelayCommand(AcceptShipping, CanAcceptShipping);
        CompleteCommand = new RelayCommand(CompleteShipping, CanCompleteShipping);
        CancelCommand = new RelayCommand(CancelShipping, CanCancelShipping);

        GetQuotesCommand = new RelayCommand(async () => await LoadQuotes());
        OptimizeQuoteCommand = new RelayCommand(async () => await LoadOptimizedQuote());
    }

    private async void LoadActiveShippings()
    {
        var shippings = await _apiService.GetActiveShippings();
        ActiveShippings.Clear();
        foreach (var shipping in shippings)
            ActiveShippings.Add(shipping);
    }

    private void SelectShipping(Shipping shipping)
    {
        SelectedShipping = shipping;
    }

    private void CreateShipping()
    {
        ActiveShippings.Add(new Shipping
        {
            TrackingNumber = $"Перевозка {ActiveShippings.Count + 1}",
            Status = ShippingStatus.Created
        });
    }

    private bool CanAcceptShipping() => SelectedShipping != null && SelectedShipping.Status == ShippingStatus.Created;
    private async void AcceptShipping()
    {
        if (SelectedShipping != null)
        {
            SelectedShipping.Status = ShippingStatus.InTransit;
            await _apiService.ChangeStatus(SelectedShipping.TrackingNumber, ShippingStatus.InTransit);
            OnPropertyChanged(nameof(SelectedShippingInfo));
            RaiseCommandStates();
        }
    }

    private bool CanCompleteShipping() => SelectedShipping != null && SelectedShipping.Status == ShippingStatus.InTransit;
    private async void CompleteShipping()
    {
        if (SelectedShipping != null)
        {
            SelectedShipping.Status = ShippingStatus.Delivered;
            await _apiService.ChangeStatus(SelectedShipping.TrackingNumber, ShippingStatus.Delivered);
            OnPropertyChanged(nameof(SelectedShippingInfo));
            RaiseCommandStates();
        }
    }

    private bool CanCancelShipping() => SelectedShipping != null &&
                                        (SelectedShipping.Status == ShippingStatus.Created || SelectedShipping.Status == ShippingStatus.InTransit);
    private async void CancelShipping()
    {
        if (SelectedShipping != null)
        {
            SelectedShipping.Status = ShippingStatus.Cancelled;
            await _apiService.ChangeStatus(SelectedShipping.TrackingNumber, ShippingStatus.Cancelled);
            OnPropertyChanged(nameof(SelectedShippingInfo));
            RaiseCommandStates();
        }
    }

    private void RaiseCommandStates()
    {
        ((RelayCommand)AcceptCommand).RaiseCanExecuteChanged();
        ((RelayCommand)CompleteCommand).RaiseCanExecuteChanged();
        ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
    }

    // ------------------ Новые функции ------------------

    private async Task LoadQuotes()
    {
        try
        {
            var quotes = await _apiService.GetShippingQuotes(InputDistance, InputWeight, InputVolume);
            Quotes.Clear();
            foreach (var q in quotes)
                Quotes.Add(q);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения котировок: {ex.Message}");
        }
    }

    private async Task LoadOptimizedQuote()
    {
        try
        {
            OptimizedQuote = await _apiService.GetOptimizedQuote(InputDistance, InputWeight, InputVolume);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения оптимальной котировки: {ex.Message}");
        }
    }

    // ------------------ INotifyPropertyChanged ------------------
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
