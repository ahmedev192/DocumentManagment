namespace DocumentManagementWeb.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
    }
}
