using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StreamingOffApp.Models
{
    public enum OfferStatus
    {
        Active,
        Inactive,
        Suspended,
        Expired
    }

    public class StreamingOffer : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private int _id;
        private string? _platformName;
        private decimal _price;
        private int _planDays;
        private OfferStatus _status;
        private string? _description;
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        [Required(ErrorMessage = "Platform name is required.")]
        public string? PlatformName
        {
            get => _platformName;
            set
            {
                _platformName = value;
                ValidateProperty(value, nameof(PlatformName));
                OnPropertyChanged(nameof(PlatformName));
            }
        }

        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000.")]
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                ValidateProperty(value, nameof(Price));
                OnPropertyChanged(nameof(Price));
            }
        }

        [Range(1, 365, ErrorMessage = "Plan days must be between 1 and 365.")]
        public int PlanDays
        {
            get => _planDays;
            set
            {
                _planDays = value;
                ValidateProperty(value, nameof(PlanDays));
                OnPropertyChanged(nameof(PlanDays));
            }
        }

        public OfferStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Any();

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }

        private void ValidateProperty<T>(T value, string propertyName)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyName };
            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
            {
                _errors[propertyName] = results.Select(r => r.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}