using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace zVirtualScenes_WPF
{
    public class DragableListViewItem : ListViewItem
    {

        //This prevent unwanted behavior #1

        protected override void OnMouseEnter(MouseEventArgs e)
        {

            //base.OnMouseEnter(e);

        }



        //This prevent unwanted behavior #2 (part 1 of 2)

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            if (!IsSelected

            || Keyboard.IsKeyDown(Key.LeftShift)

            || Keyboard.IsKeyDown(Key.LeftCtrl)

            || Keyboard.IsKeyDown(Key.RightShift)

            || Keyboard.IsKeyDown(Key.RightCtrl))
            {

                base.OnMouseLeftButtonDown(e);

            }

        }

        //This prevent unwanted behavior #2 (part 2 of 2)

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            if (!IsSelected

            || Keyboard.IsKeyDown(Key.LeftShift)

            || Keyboard.IsKeyDown(Key.LeftCtrl)

            || Keyboard.IsKeyDown(Key.RightShift)

            || Keyboard.IsKeyDown(Key.RightCtrl))
            {

                base.OnMouseLeftButtonUp(e);

            }

            else
            {

                base.OnMouseLeftButtonDown(e);

            }

        }

    }
}

