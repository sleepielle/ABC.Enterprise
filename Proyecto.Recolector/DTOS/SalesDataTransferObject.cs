namespace Proyecto.Recolector.DTOS
{
    public class completeDTOS
    {

        public TransactionDataTransferObject transaction;
        public List<SalesDataTransferObject> sales;
    }

    public class TransactionDataTransferObject
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public List<string> Errors { get; set; }
    }
    public static class TransactionStatus
    {
        public const string InProcess = "En Proceso";
        public const string Charged = "Cobrado";
        public const string Completed = "Completed";
    }

    public class SalesDataTransferObject
    {

        public string username { get; set; }
        public string car_id { get; set; }
        public string price { get; set; }
        public string vin { get; set; }
        public string buyer_first_name { get; set; }
        public string buyer_last_name { get; set; }
        public string buyer_id { get; set; }
        public string branch_id { get; set; }
    }
}
