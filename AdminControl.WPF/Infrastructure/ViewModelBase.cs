using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdminControl.WPF.Infrastructure
{
    /// <summary>
    /// Базовий клас для всіх ViewModel з підтримкою INotifyPropertyChanged та IDataErrorInfo
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDataErrorInfo

        // Словник для зберігання помилок валідації
        private readonly Dictionary<string, string> _errors = new();

        /// <summary>
        /// Загальна помилка об'єкта
        /// </summary>
        public string Error => string.Empty;

        /// <summary>
        /// Індексатор для отримання помилки по імені властивості
        /// </summary>
        public string this[string columnName]
        {
            get
            {
                if (_errors.ContainsKey(columnName))
                    return _errors[columnName];
                return string.Empty;
            }
        }

        /// <summary>
        /// Чи є помилки валідації
        /// </summary>
        public bool HasErrors => _errors.Any();

        /// <summary>
        /// Додати помилку валідації для властивості
        /// </summary>
        protected void AddError(string error, [CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                _errors[propertyName] = error;
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Очистити помилку валідації для властивості
        /// </summary>
        protected void ClearErrors([CallerMemberName] string propertyName = "")
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Очистити всі помилки валідації
        /// </summary>
        protected void ClearAllErrors()
        {
            _errors.Clear();
        }

        #endregion
    }
}
