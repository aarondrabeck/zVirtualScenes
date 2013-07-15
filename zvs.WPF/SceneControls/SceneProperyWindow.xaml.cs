using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zvs.Entities;
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

        ~ScenePropertiesWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("ScenePropertiesWindow Deconstructed.");
        }

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
            using (zvsContext context = new zvsContext())
            {
                var scene = await context.Scenes.FirstOrDefaultAsync(sc => sc.Id == SceneID);
                if (scene != null)
                {
                    scene.isRunning = false;

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
                }
            }
        }
    }
}
