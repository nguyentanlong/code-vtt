using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using VTT.libs;

namespace VTT
{
    [ScriptService]
    public partial class MenuPermissionApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
        }

        [WebMethod(EnableSession = true)]
        public static List<string> GetVisibleTrangList()
        {
            return PermissionHelper.GetVisibleTrangList();
        }
    }
}