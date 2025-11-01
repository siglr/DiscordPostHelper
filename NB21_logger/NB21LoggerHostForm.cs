using System.Drawing;
using System.Windows.Forms;

namespace NB21_logger
{
    public class NB21LoggerHostForm : Form
    {
        private readonly NB21Logger loggerControl;

        public NB21LoggerHostForm(bool launchedViaStartup)
        {
            this.loggerControl = new NB21Logger(launchedViaStartup)
            {
                Dock = DockStyle.Fill
            };

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = this.loggerControl.Size;
            Controls.Add(this.loggerControl);
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }
    }
}
