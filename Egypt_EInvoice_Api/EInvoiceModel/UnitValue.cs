using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class UnitValue
    {
        public string currencySold { get; set; } //Currency code used from ISO 4217. example EGP كود العملة
        public decimal amountEGP { get; set; } //السعر بالجنيه المصري
        public decimal? amountSold { get; set; } //السعر بالعملة الاجنبيه - يكون هذا الحقل اجباري اذا كانت العملة المستخدة هي عملة اجنبية ولا ياخذ اي قيمة اذا كانت العملة المستخدمة هي الجنية
        public decimal currencyExchangeRate { get; set; }
        [JsonIgnore] // <---- ضيف السطر ده
        public int? currencyNumber { get; set; }

    }
}
