using DocumentManagementWeb.Models;

namespace DocumentManagementWeb.VM
{
    public class MainVM
    {
      public  List<FileModel> files;
        public IQueryable<Notification> notifications;
    }
}
