using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ColorJump
{
    public class BaseScreen: Control
    {
        private bool _isWorking;
        public BaseScreen(Control parent)
        {
            Parent = parent;
            Location = new Point(0, 0);
            Size = new Size(800, 600);
            DoubleBuffered = true;
            if (parent is Form form)
            {
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ControlBox = false;
                form.BackColor = Color.MidnightBlue;
            }
        }

        public void Start()
        {
            _isWorking = true;
            new Thread(Run).Start();
        }
        
        public void Stop()
        {
            _isWorking = false;
        }

        private const int SleepTime = 40;

        private void Run() 
        {
            while (_isWorking)
            {
                Update(SleepTime);
                Invalidate();
                Thread.Sleep(SleepTime);
            }
        }

        public virtual void Update(long tick)
        {

        }
    }
}
