//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ITAXI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class information_Dr_Order
    {
        public int Order_Num { get; set; }
        public string Dr_ID { get; set; }
        public string Cus_ID { get; set; }
        public string Ord_Status { get; set; }
        public string Ord_Type { get; set; }
        public string Ord_date { get; set; }
        public string Ord_Numpeople { get; set; }
        public Nullable<byte> Ord_Money { get; set; }
        public Nullable<System.TimeSpan> Boarding_Time { get; set; }
        public string Boarding_Location { get; set; }
        public Nullable<System.TimeSpan> Drop_off_Time { get; set; }
        public string Drop_off_location { get; set; }
        public Nullable<System.TimeSpan> Journey_Time { get; set; }
    }
}
