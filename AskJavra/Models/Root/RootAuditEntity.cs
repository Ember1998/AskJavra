namespace AskJavra.Models.Root
{
    public class RootAuditEntity
    {
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public required string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
