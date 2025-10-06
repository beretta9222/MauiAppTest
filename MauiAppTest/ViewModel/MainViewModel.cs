// This is an open source non-commercial project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using CommunityToolkit.Mvvm.Input;
using MauiAppTest.Model;
using System.ComponentModel;
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

        public List<string> Headers => ["1", "2"];

        private List<OutputItem> calculateItems = new();

        /// <summary>
        /// Вывод данных дохода/сумма платежа
        /// </summary>
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

        private RelayCommand? calculateCmd;
        public RelayCommand CalculateCmd => calculateCmd ?? (new RelayCommand(() => { Calculate(); }));

        private async Task Calculate()
        {
            lock (_locker)
            {
                ICalculation calculation;

                if (IsCredit)
                {
                    calculation = new CreditCalculation(Amount, Percent, (uint)Period);
                }
                else
                {
                    calculation = new DepositCalculation(Amount, Percent, (uint)Period, Withdrawal);
                }
                CalculateItems = calculation.Clalculation();
            }
        }
        #endregion
    }
}
