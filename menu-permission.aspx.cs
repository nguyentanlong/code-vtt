/*using System.Collections.Generic;
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
}*/
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
        public static object GetMenuInfo()
        {
            var visibleTrangList = PermissionHelper.GetVisibleTrangList();
            bool isAdmin = PermissionHelper.IsAdminOwner();
            // Có quyền quản lý sơ đồ tổ chức (Sửa Nhân viên) -> Trưởng phòng/CN_Admin/Admin; Phó phòng/Nhân viên = false
            bool canManageOrgChart = PermissionHelper.GetPermissionScope(Trang.NV, ChucNang.U) != null;

            return new
            {
                visibleTrang = visibleTrangList,
                isAdmin = isAdmin,
                canManageOrgChart = canManageOrgChart
            };
        }
    }
}