using System;
using System.Windows.Forms;

namespace ColorJump
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = "";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Controls.Add(new MainMenuScreen(this));
        }

        public void ChangeScreen(BaseScreen screen)
        {
            Controls.Clear();
            Controls.Add(screen);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            (Controls[0] as BaseScreen)?.Stop();
        }
    }
}
