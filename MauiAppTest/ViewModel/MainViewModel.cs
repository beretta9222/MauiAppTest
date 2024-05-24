using CommunityToolkit.Mvvm.Input;
using MauiAppTest.Model;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;


namespace MauiAppTest.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region Переключатели  

        private bool isDeposit = true;
        /// <summary>
        /// Если вклад
        /// </summary>
        public bool IsDeposit
        {
            get
            {
                return isDeposit;
            }
            set
            {
                isDeposit = value;
                WithdrawalEnabled = IsDeposit;
                OnPropertyChanged(nameof(IsDeposit));
            }
        }

        private bool isCredit;
        /// <summary>
        /// Если кредит
        /// </summary>
        public bool IsCredit
        {
            get
            {
                return isCredit;
            }
            set
            {
                isCredit = value;
                WithdrawalEnabled = IsDeposit;
                OnPropertyChanged(nameof(IsCredit));
            }
        }

        private bool withdrawal;
        /// <summary>
        /// Снятие накопленных процентов со вклада
        /// </summary>
        public bool Withdrawal
        {
            get
            {
                return withdrawal;
            }
            set
            {
                withdrawal = value;
                OnPropertyChanged(nameof(Withdrawal));
            }

        }
        #endregion

        #region Поля ввода и ввывод

        private decimal amount;
        /// <summary>
        /// Сумма для расчета
        /// </summary>
        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        private decimal percent;
        /// <summary>
        /// Ставка по кредиту или процент по вкладу
        /// </summary>
        public decimal Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
                OnPropertyChanged(nameof(Percent));
            }
        }

        private int period;
        /// <summary>
        /// Период кредита (в месяцах) или вклад (в днях)
        /// </summary>
        public int Period
        {
            get
            {
                return period;
            }
            set
            {
                period = value;
                OnPropertyChanged(nameof(Period));
            }
        }

        private List<OutputItem> calculateItems;
        public List<OutputItem> CalculateItems
        {
            get
            {
                return calculateItems;
            }
            set
            {
                calculateItems = value;
                OnPropertyChanged(nameof(CalculateItems));
            }
        }

        #endregion

        #region текст и состояние элементов

        public string Amount_txt => "Сумма";
        public string Percent_txt => "Процент";
        public string Period_txt => "Период";
        public string Debit_txt => "Вклад";
        public string Credit_txt => "Кредит";
        public string Withdrawal_txt => "Оставлять на вкладе";

        public string Calculate_txt => "Расчитать";

        private bool withdrawalEnabled;
        /// <summary>
        /// Активный Чекбокс "Снятие накопленных процентов со вклада"
        /// Доступно только для вклада
        /// </summary>
        public bool WithdrawalEnabled
        {
            get
            {
                return withdrawalEnabled;
            }
            set
            {
                withdrawalEnabled = value;
                OnPropertyChanged(nameof(WithdrawalEnabled));
            }
        }
        #endregion

        #region Комманды

        private volatile object _locker = new();

        private RelayCommand calculateCmd;
        public RelayCommand CalculateCmd
        {
            get
            {
                return calculateCmd ?? (new RelayCommand(() =>
                {
                    Task.Run(() =>
                    {
                        Calculate();
                    });

                }));
            }
        }

        private void Calculate()
        {
            lock (_locker)
            {
                List<OutputItem> section = new List<OutputItem>();
                if (IsCredit)
                {
                    try
                    {
                        decimal payed = (Amount * ((Percent / 100.0m) / 12.0m)) / (1 - (decimal)Math.Pow((double)(1 + ((Percent / 100.0m) / 12.0m)), (-1) * Period));
                        section.Add(new()
                        {
                            Text = "Сумма платежа",
                            Value = $"{payed:N2}"
                        });
                    }
                    catch(DivideByZeroException ex)
                    {
                        section.Add(new() { Text = "Ошибка", Value = "Невозможно взять кредит на 0 месецев" });
                    }
                }
                else if(IsDeposit)
                {
                    DateTime from = DateTime.Now;
                    DateTimeFormatInfo info = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat;
                    int i = 0;
                    decimal total_summ = 0, calc_sum = Amount;
                    do
                    {
                        var data = from.AddMonths(1);
                        var tmp_days = (data.Date - from.Date).Days;
                        if (tmp_days > Period - i)
                        {
                            tmp_days = Period - i;
                        }
                        decimal tmp_summ = decimal.Floor(calc_sum * Percent * tmp_days / (DateTime.IsLeapYear(data.Year) ? 366 : 365)) / 100.0m;
                        section.Add(new() 
                        { 
                            Text = $"{info.MonthNames[data.Month - 1]} {data.Year}",
                            Value = $"{tmp_summ:N2}"
                        });
                        i += tmp_days;
                        from = data;
                        if (Withdrawal)
                            calc_sum += tmp_summ;
                        total_summ += tmp_summ;
                    }
                    while (i < Period);
                    section.Add(new() 
                    { 
                        Text = "Итого",
                        Value = $"{total_summ:N2}" 
                    });
                }
                CalculateItems = section;
            }

        }
        #endregion
    }
}
