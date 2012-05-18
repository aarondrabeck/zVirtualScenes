using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenes_WPF.CustomControls
{
    public class DragandDropDataGrid : DataGrid
    {
        public DragandDropDataGrid()
        {
            AllowDrop = true;
            this.MouseMove += new System.Windows.Input.MouseEventHandler(DragandDropDataGrid_MouseMove);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DragandDropDataGrid_PreviewMouseLeftButtonUp);
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragandDropDataGrid_PreviewMouseLeftButtonDown);
        }

        private void DragandDropDataGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !ProcessDrag)
            {
                return;
            }

            if (IsMouseOverScrollbar(sender, e.GetPosition(sender as IInputElement)))
            {
                ProcessDrag = false;
                return;
            }

            if (this.SelectedItems.Count == 0)
                return;

            
                

            DataObject dataObject = new DataObject("objects", this.SelectedItems);

            //CUSTOMIZE TYPES HERE
            var devices = this.SelectedItems.OfType<device>().ToList();
            if (devices.Count > 0)
                dataObject.SetData("deviceList", devices);

            var scenes = this.SelectedItems.OfType<scene>().ToList();
            if (scenes.Count > 0)
                dataObject.SetData("sceneList", scenes);

            DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
        }

        private static bool IsMouseOverScrollbar(object sender, Point mousePosition)
        {
            if (sender is Visual)
            {
                HitTestResult hit = VisualTreeHelper.HitTest(sender as Visual, mousePosition);

                if (hit == null) return false;

                DependencyObject dObj = hit.VisualHit;
                while (dObj != null)
                {
                    if (dObj is ScrollBar) return true;

                    if ((dObj is Visual) || (dObj is Visual3D)) dObj = VisualTreeHelper.GetParent(dObj);
                    else dObj = LogicalTreeHelper.GetParent(dObj);
                }
            }

            return false;
        }

        bool ProcessDrag = true;
        DataGridRow DragStartRow = null;
        private void DragandDropDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProcessDrag = true;

            if ((Keyboard.Modifiers == ModifierKeys.Control) || (Keyboard.Modifiers == ModifierKeys.Shift))
                return;

            DataGrid daatgrid = sender as DataGrid;
            Point pt = e.GetPosition(daatgrid);

            VisualTreeHelper.HitTest(daatgrid, null, (result) =>
            {
                DataGridRow row = FindVisualParent<DataGridRow>(result.VisualHit);
                if (row != null)
                {
                    DragStartRow = row;
                    return HitTestResultBehavior.Stop;
                }
                else
                    return HitTestResultBehavior.Continue;
            },
            new PointHitTestParameters(pt));

            if (DragStartRow != null)
            {
                //if the row is already selected, ignore this event so multi-row selections 
                //can be dragged without losing selection the other rows.
                if (DragStartRow.IsSelected)
                    e.Handled = true;
            }
        }

        private void DragandDropDataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) || (Keyboard.Modifiers == ModifierKeys.Shift))
                return;

            if (DragStartRow != null)
            {
                DataGridRow RowMouseIsOver = null;
                DataGrid daatgrid = sender as DataGrid;
                Point pt = e.GetPosition(daatgrid);

                VisualTreeHelper.HitTest(daatgrid, null, (result) =>
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(result.VisualHit);
                    if (row != null)
                    {
                        RowMouseIsOver = row;
                        return HitTestResultBehavior.Stop;
                    }
                    else
                        return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(pt));

                if (RowMouseIsOver == DragStartRow)
                {
                    this.SelectedItems.Clear();
                    DragStartRow.IsSelected = true;
                }
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
    }
}
