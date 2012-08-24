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


namespace zVirtualScenesGUI.SceneControls
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

        private void ResetBtn_Click_1(object sender, RoutedEventArgs e)
        {

            using (zvsContext context = new zvsContext())
            {
                Scene s = context.Scenes.FirstOrDefault(sc => sc.SceneId == SceneID);
                if (s != null)
                {
                    s.isRunning = false;
                    context.SaveChanges();
                }
            }
        }
    }
}
