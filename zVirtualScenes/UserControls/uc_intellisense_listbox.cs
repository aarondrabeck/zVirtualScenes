using System.Drawing;
using System.Windows.Forms;

namespace zVirtualScenesApplication.UserControls
{
    public class IntellisenseListBoxItem
    {
        private string _myText;
        private int _myImageIndex;

        public string Text
        {
            get { return _myText; }
            set { _myText = value; }
        }
        public int ImageIndex
        {
            get { return _myImageIndex; }
            set { _myImageIndex = value; }
        }

        public IntellisenseListBoxItem(string text, int index)
        {
            _myText = text;
            _myImageIndex = index;
        }
        public IntellisenseListBoxItem(string text) : this(text, -1) { }
        public IntellisenseListBoxItem() : this("") { }
        public override string ToString()
        {
            return _myText;
        }
    }

    public class uc_intellisense_listbox : ListBox
    {
        private ImageList _myImageList;
        public ImageList ImageList
        {
            get { return _myImageList; }
            set { _myImageList = value; }
        }
        public uc_intellisense_listbox()
        {
            // Set owner draw mode
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            IntellisenseListBoxItem item;
            Rectangle bounds = e.Bounds;
            if (_myImageList != null)
            {
                Size imageSize = _myImageList.ImageSize;
                try
                {
                    item = (IntellisenseListBoxItem) Items[e.Index];
                    if (item.ImageIndex != -1)
                    {
                        _myImageList.Draw(e.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                                              bounds.Left + imageSize.Width, bounds.Top);
                    }
                    else
                    {
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                                              bounds.Left, bounds.Top);
                    }
                }
                catch
                {
                    if (e.Index != -1)
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), e.Font,
                                              new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                    }
                    else
                    {
                        e.Graphics.DrawString(Text, e.Font, new SolidBrush(e.ForeColor),
                                              bounds.Left, bounds.Top);
                    }
                }
                base.OnDrawItem(e);
            }
        }
    }
}
