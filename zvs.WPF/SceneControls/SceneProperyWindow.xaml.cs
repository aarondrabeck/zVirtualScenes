using System.Diagnostics;
using System.Windows;
using zvs.DataModel;
using System.Data.Entity;
using zvs.Processor;

namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for ScenePropertiesWindow.xaml
    /// </summary>
    public partial class ScenePropertiesWindow
    {
        private int SceneId { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;

        public ScenePropertiesWindow(int sceneId)
        {
            SceneId = sceneId;
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Scene Properties" };
            InitializeComponent();
        }
#if DEBUG
        ~ScenePropertiesWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("ScenePropertiesWindow Deconstructed.");
        }
#endif

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            SceneControl.SceneId = SceneId;
        }

        private async void ResetBtn_Click_1(object sender, RoutedEventArgs e)
        {
            using (var context = new ZvsContext(_app.EntityContextConnection))
            {
                var scene = await context.Scenes.FirstOrDefaultAsync(sc => sc.Id == SceneId);
                if (scene == null) return;
                scene.IsRunning = false;

                var result = await context.TrySaveChangesAsync(_app.Cts.Token);
                if (result.HasError)
                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error resetting scene. {0}", result.Message);
				
            }
        }
    }
}
