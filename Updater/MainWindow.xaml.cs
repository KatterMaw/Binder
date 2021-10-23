using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WorkingWithFiles.eventPercentAndSpeedOnChange += setPercentAndSpeed;
            LauncherUpdater launcher = new LauncherUpdater();
            Task.Run(() => launcher.UpdaterLauncher(this));
        }

        public void setPercentAndSpeed(double percent)
        {
            Dispatcher.Invoke(() => percent_text.Content = ((int)percent).ToString() + "%");
            Dispatcher.Invoke(() => progressbar.Value = percent);
        }
    }
}
