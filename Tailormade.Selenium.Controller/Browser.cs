using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tailormade.Selenium.Controller
{
    public partial class Browser : IDisposable
    {
        public OpenQA.Selenium.Remote.RemoteWebDriver Driver { get; set; }
        public int DefaultSleep { get; set; }
        public int NavigateCount { get; set; }
        public int MaxRenderWait { get; set; }
        public Browser()
        {
            this.DefaultSleep = 2000;
            this.MaxRenderWait = 5000;
            RecycleDriver();
        }

        public bool Navigate(string url, int sleep = 0, string checkString = "||||||||||||||||")
        {
            Driver.ExecuteScript("window.focus();");
            Driver.Navigate().GoToUrl(url);
            IsBrowserReady(sleep: sleep, checkString: checkString);
            NavigateCount++;
            return (Driver.Url.IndexOf(url) > -1);
        }
        public bool IsBrowserReady(int sleep = 0, string checkString = "||||||||||||||||")
        {
            var dt = DateTime.Now.AddMilliseconds(sleep == 0 ? DefaultSleep : sleep);
            var checkStringFound = false;
            while (!checkStringFound && DateTime.Now < dt)
            {
                checkStringFound = checkString == "||||||||||||||||" ? false : (Driver.PageSource.IndexOf(checkString, StringComparison.InvariantCultureIgnoreCase) > -1);
                if (!checkStringFound)
                {
                    Task.Delay(100).Wait();
                    //System.Threading.Thread.Sleep(10);
                }
            }
            //System.Threading.Thread.Sleep(sleep==0?DefaultSleep:sleep);
            return true;
        }

        

        public bool RecycleDriver(int recycleNavigateCount = 100)
        {
            if (this.Driver != null && this.NavigateCount < recycleNavigateCount)
            { return false; }
            if (this.Driver != null)
            {
                this.Driver.Quit();
            }
            var options = new OpenQA.Selenium.Chrome.ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            options.AddArguments("--start-maximized");
            this.Driver = new OpenQA.Selenium.Chrome.ChromeDriver(options);
            //this.RemoteWebDriver = new OpenQA.Selenium.Firefox.FirefoxDriver();
            NavigateCount = 0;
            return true;
        }
        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
