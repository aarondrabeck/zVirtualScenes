using System.Windows;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for ScenePropertiesWindow.xaml
    /// </summary>
    public partial class ScenePropertiesWindow : Window
    {
        private int SceneID = 0;

        public ScenePropertiesWindow(int SceneID)
        {
            this.SceneID = SceneID;
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
            this.Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            SceneControl.SceneID = SceneID;
        }

        private async void ResetBtn_Click_1(object sender, RoutedEventArgs e)
        {
            using (ZvsContext context = new ZvsContext())
            {
                var scene = await context.Scenes.FirstOrDefaultAsync(sc => sc.Id == SceneID);
                if (scene != null)
                {
                    scene.IsRunning = false;

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).ZvsEngine.log.Error(result.Message);
                }
            }
        }
    }
}
