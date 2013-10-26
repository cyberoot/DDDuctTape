using System.Windows.Forms;
using System.Threading;

namespace FolderSync.Library.Common.SplashScreen
{
    public class SplashScreen
    {
        protected bool _Done = false;
        Thread _Thread = null;
        Form _Form = null;

        public SplashScreen(Form _frm)
        {
            _Form = _frm;
        }

        public void Show()
        {
            ThreadStart ts = new ThreadStart(ShowForm);
            _Thread = new Thread(ts);
            _Thread.Start();
        }

        protected void ShowForm()
        {
            _Form.Show();
            _Form.Update();
            while(!_Done)
            {
                Application.DoEvents();
            }
            _Form.Close();
            _Form.Dispose();
        }

        public void Close()
        {
            _Done = true;
        }
    }
}
