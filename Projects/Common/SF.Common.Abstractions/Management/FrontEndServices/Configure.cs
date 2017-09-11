using ServiceProtocol.ObjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.DI;
using ServiceProtocol.Auth.Permissions;
namespace ServiceProtocol.Biz.UIManager
{
    public static class Configure
    {
        public static IDIRegister UseUIManager(this IDIRegister Register )
        {
            Register.AddAuthResource("界面内容", "用户界面", "界面内容", null, "查看", "管理");
            Register.AddAuthResource("界面站点", "用户界面", "界面站点", null, "查看", "管理");
            Register.AddAuthResource("界面站点模板", "用户界面", "界面站点模板", null, "查看", "管理");
            return Register;
        }
    }

}
