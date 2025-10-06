namespace MauiAppTest.Model
{
    internal class CreditCalculation : ICalculation
    {
        private decimal _amount;
        private decimal _percent;
        private uint _period;
        public CreditCalculation(decimal amount, decimal percent, uint period)
        {
            _amount = amount;
            _percent = percent;
            _period = period;
        }
        public List<OutputItem> Clalculation()
        {
            int period = (int)_period;
            decimal tmp_amount = _amount;
            DateTime now = DateTime.Now;
            List<OutputItem> result = new List<OutputItem>();

            if(_period < 1)
                result.Add(new() { Text = "Ошибка", Value = "Невозможно взять кредит на 0 или отрицательное кол-во месяцев месецев" });
            else
                while (tmp_amount > 0 && period > 0)
                {
                    decimal payed = (_amount * ((_percent / 100.0m) / 12.0m)) / (1 - (decimal)Math.Pow((double)(1 + ((_percent / 100.0m) / 12.0m)), (-1) * _period));
                    decimal dolg = tmp_amount > payed ? payed : tmp_amount;
                    now = now.AddMonths(1);
                    result.Add(new() { Text = $"{now: dd.MM.yyyy}", Value = dolg.ToString("N2") });
                    tmp_amount -= dolg;
                    period--;
                }
            return result;
        }
    }
}
