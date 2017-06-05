using Microsoft.AspNet.Identity;

namespace CRM.Entities
{
    public partial class User : IUser { }
    public partial class Role : IRole<string> { }
    public partial class CRMContext
    {
        public static CRMContext Create()
        {
            return new CRMContext();
        }
    }
}
