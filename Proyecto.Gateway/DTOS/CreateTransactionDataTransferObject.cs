namespace Proyecto.Gateway.DTOS
{
    public class CreateTransactionDataTransferObject
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
}

//public enum Status
//{
//    InProgress,
//    Done,
//    Errored
//}
