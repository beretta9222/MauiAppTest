
namespace MauiAppTest.Model
{
    internal class DepositCalculation : ICalculation
    {
        private decimal _amount;
        private decimal _percent;
        private uint _period;
        private bool _inEndPeriod;
        public DepositCalculation(decimal amount, decimal percent, uint period, bool inEndPeriod) 
        {
            _amount = amount;
            _percent = percent;
            _period = period;
            _inEndPeriod = inEndPeriod;
        }
        public List<OutputItem> Clalculation()
        {
            List<OutputItem> result = new();
            if (_period < 1)
                return result;
            decimal tmp_amount = _amount;
            int tmp_period = (int)_period;
            DateTime from = DateTime.Now;
            decimal total = 0; 
            do
            {
                DateTime to = from.AddMonths(1);
                int tmp_period_month = (to - from).Days;
                tmp_period_month = tmp_period > tmp_period_month ? tmp_period_month : tmp_period;
                decimal tmp_summ = decimal.Floor(tmp_amount * _percent * tmp_period_month / (DateTime.IsLeapYear(from.Year) ? 366 : 365)) / 100.0m;
                total += tmp_summ;
                from = to;
                tmp_period -= tmp_period_month;
                if (_inEndPeriod)
                    tmp_amount += tmp_summ;
            }
            while(tmp_period > 0);
            result.Add(new() { Text = "Итого:", Value = total.ToString("N2") });
            return result;
        }
    }
}
