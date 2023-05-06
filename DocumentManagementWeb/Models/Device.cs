namespace DocumentManagementWeb.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
