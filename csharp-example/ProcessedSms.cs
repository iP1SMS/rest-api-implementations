using System;

namespace csharp_example
{
    internal class ProcessedSms
    {
        public int ID { get; set; }
        public Nullable<int> BundleID { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public string To { get; set; }
        public object CountryCode { get; set; }
        public string Currency { get; set; }
        public double TotalPrice { get; set; }
        public double Price { get; set; }
        public string Encoding { get; set; }
        public int Segments { get; set; }
        public int Prio { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
    }
}