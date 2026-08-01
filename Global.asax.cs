using System;
using System.Web;

namespace VTT
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch
            {
            }
        }
    }
}