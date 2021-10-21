using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WorkingWithFiles.eventPercentAndSpeedOnChange += setPercentAndSpeed;
            LauncherUpdater launcher = new LauncherUpdater();
            Task.Run(() => launcher.UpdaterLauncher());
        }

        public void setPercentAndSpeed(double percent)
        {
            this.Dispatcher.Invoke(() => percent_text.Content = ((int)percent).ToString() + "%");
            this.Dispatcher.Invoke(() => progressbar.Value = percent);
        }
    }
}
