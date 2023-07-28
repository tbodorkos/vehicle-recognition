namespace Core.Entities
{
    public class Status
    {
        public string Id { get; set; } = default!;
        public string StatusQueryGetUri { get; set; } = default!;
        public string SendEventPostUri { get; set; } = default!;
        public string TerminatePostUri { get; set; } = default!;
        public string PurgeHistoryDeleteUri { get; set; } = default!;
        public string ErrorMessage { get; set; } = default!;
    }
}
