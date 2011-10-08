using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Objects;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_script_editor : UserControl
    {
        private Regex keyWords = new Regex("abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|" +
            "extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|" +
            "public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|volatile|void|while|");

        private string typed = "";
        private bool wordMatched;
        private bool foundNode;
        private string currentPath;
        private Hashtable namespaces;
        private TreeNode nameSpaceNode;
        private TreeNode findNodeResult;

        private ObjectQuery<device> deviceListQuery;

        public uc_script_editor()
        {
            InitializeComponent();
        }

        private void uc_script_editor_Load(object sender, EventArgs e)
        {
            // Load up all the items and their actions/methods

            deviceListQuery = zvsEntityControl.zvsContext.devices;
            deviceListQuery.MergeOption = MergeOption.AppendOnly;

            foreach (device d in ((IListSource)deviceListQuery).GetList())
            {
                TreeNode newNode = new TreeNode(d.friendly_name.Replace(" ", "_"));

                foreach (device_commands dc in d.device_commands)
                {
                    TreeNode subNode = new TreeNode(dc.friendly_name.Replace(" ", "_"));

                    subNode.Tag = "method";

                    newNode.Nodes.Add(subNode);
                }

                treeViewItems.Nodes.Add(newNode);
            }
        }

        public void SetScript(string script)
        {
            txtScript.Text = script;
        }

        public string GetScript()
        {
            return txtScript.Text;
        }

        private void txtScript_TextChanged(object sender, EventArgs e)
        {
            // Test to remove color from words that no longer match
            txtScript.SelectAll();
            txtScript.SelectionColor = Color.Black;
            txtScript.Select(txtScript.Text.Length, 0);

            int selPos = txtScript.SelectionStart;

            //For each match from the regex, highlight the word.
            foreach (Match keyWordMatch in keyWords.Matches(txtScript.Text))
            {
                txtScript.Select(keyWordMatch.Index, keyWordMatch.Length);
                txtScript.SelectionColor = Color.Blue;
                txtScript.SelectionStart = selPos;
                txtScript.SelectionColor = Color.Black;
            }
        }

        private void txtScript_KeyDown(object sender, KeyEventArgs e)
        {
            // Keep track of the current character, used
            // for tracking whether to hide the list of members,
            // when the delete button is pressed
            int i = txtScript.SelectionStart;
            string currentChar = "";

            if (i > 0)
            {
                currentChar = txtScript.Text.Substring(i - 1, 1);
            }

            if (e.KeyData == Keys.OemPeriod)
            {
                // The amazing dot key

                if (!listBoxAutoComplete.Visible)
                {
                    // Display the member listview if there are
                    // items in it
                    if (populateListBox())
                    {
                        //this.listBoxAutoComplete.SelectedIndex = 0;

                        // Find the position of the caret
                        Point point = txtScript.GetPositionFromCharIndex(txtScript.SelectionStart);
                        point.Y += (int)Math.Ceiling(txtScript.Font.GetHeight()) + 2;
                        point.X += 2; // for Courier, may need a better method

                        listBoxAutoComplete.Location = point;
                        listBoxAutoComplete.BringToFront();
                        listBoxAutoComplete.Show();
                    }
                }
                else
                {
                    listBoxAutoComplete.Hide();
                    typed = "";
                }

            }
            else if (e.KeyCode == Keys.Back)
            {
                // Delete key - hides the member list if the character
                // being deleted is a dot

                textBoxTooltip.Hide();
                if (typed.Length > 0)
                {
                    typed = typed.Substring(0, typed.Length - 1);
                }
                if (currentChar == ".")
                {
                    listBoxAutoComplete.Hide();
                }

            }
            else if (e.KeyCode == Keys.Up)
            {
                // The up key moves up our member list, if
                // the list is visible

                textBoxTooltip.Hide();

                if (listBoxAutoComplete.Visible)
                {
                    wordMatched = true;
                    if (listBoxAutoComplete.SelectedIndex > 0)
                        listBoxAutoComplete.SelectedIndex--;

                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                // The up key moves down our member list, if
                // the list is visible

                textBoxTooltip.Hide();

                if (listBoxAutoComplete.Visible)
                {
                    wordMatched = true;
                    if (listBoxAutoComplete.SelectedIndex < listBoxAutoComplete.Items.Count - 1)
                        listBoxAutoComplete.SelectedIndex++;

                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.D9)
            {
                // Trap the open bracket key, displaying a cheap and
                // cheerful tooltip if the word just typed is in our tree
                // (the parameters are stored in the tag property of the node)

                string word = getLastWord();
                foundNode = false;
                nameSpaceNode = null;

                currentPath = "";
                searchTree(treeViewItems.Nodes, word, true);

                if (nameSpaceNode != null)
                {
                    if (nameSpaceNode.Tag is string)
                    {
                        textBoxTooltip.Text = (string)nameSpaceNode.Tag;

                        Point point = txtScript.GetPositionFromCharIndex(txtScript.SelectionStart);
                        point.Y += (int)Math.Ceiling(txtScript.Font.GetHeight()) + 2;
                        point.X -= 10;
                        textBoxTooltip.Location = point;
                        textBoxTooltip.Width = textBoxTooltip.Text.Length * 6;

                        textBoxTooltip.Size = new Size(textBoxTooltip.Text.Length * 6, textBoxTooltip.Height);

                        // Resize tooltip for long parameters
                        // (doesn't wrap text nicely)
                        if (textBoxTooltip.Width > 300)
                        {
                            textBoxTooltip.Width = 300;
                            int height = 0;
                            height = textBoxTooltip.Text.Length / 50;
                            textBoxTooltip.Height = height * 15;
                        }
                        textBoxTooltip.Show();
                    }
                }
            }
            else if (e.KeyCode == Keys.D8)
            {
                // Close bracket key, hide the tooltip textbox

                textBoxTooltip.Hide();
            }
            else if (e.KeyValue < 48 || (e.KeyValue >= 58 && e.KeyValue <= 64) || (e.KeyValue >= 91 && e.KeyValue <= 96) || e.KeyValue > 122)
            {
                // Check for any non alphanumerical key, hiding
                // member list box if it's visible.

                if (listBoxAutoComplete.Visible)
                {
                    // Check for keys for autofilling (return,tab,space)
                    // and autocomplete the richtextbox when they're pressed.
                    if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Space)
                    {
                        textBoxTooltip.Hide();

                        // Autocomplete
                        selectItem();

                        typed = "";
                        wordMatched = false;
                        e.Handled = true;
                    }

                    // Hide the member list view
                    listBoxAutoComplete.Hide();
                }
            }
            else
            {
                // Letter or number typed, search for it in the listview
                if (listBoxAutoComplete.Visible)
                {
                    char val = (char)e.KeyValue;
                    typed += val;

                    wordMatched = false;

                    // Loop through all the items in the listview, looking
                    // for one that starts with the letters typed
                    for (i = 0; i < listBoxAutoComplete.Items.Count; i++)
                    {
                        if (listBoxAutoComplete.Items[i].ToString().ToLower().StartsWith(typed.ToLower()))
                        {
                            wordMatched = true;
                            listBoxAutoComplete.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    typed = "";
                }
            }
        }
        
        private bool populateListBox()
        {
            bool result = false;
            string word = getLastWord();

            //System.Diagnostics.Debug.WriteLine(" - Path: " +word);

            if (word != "")
            {
                findNodeResult = null;
                findNode(word, treeViewItems.Nodes);

                if (findNodeResult != null)
                {
                    listBoxAutoComplete.Items.Clear();

                    if (findNodeResult.Nodes.Count > 0)
                    {
                        result = true;

                        // Sort alphabetically (this could be replaced with
                        // a sortable treeview)
                        MemberItem[] items = new MemberItem[findNodeResult.Nodes.Count];
                        for (int n = 0; n < findNodeResult.Nodes.Count; n++)
                        {
                            MemberItem memberItem = new MemberItem();
                            memberItem.DisplayText = findNodeResult.Nodes[n].Text;
                            memberItem.Tag = findNodeResult.Nodes[n].Tag;

                            if (findNodeResult.Nodes[n].Tag != null)
                            {
                                System.Diagnostics.Debug.WriteLine(findNodeResult.Nodes[n].Tag.GetType().ToString());
                            }

                            items[n] = memberItem;
                        }
                        Array.Sort(items);

                        for (int n = 0; n < items.Length; n++)
                        {
                            int imageindex = 0;

                            if (items[n].Tag != null)
                            {
                                // Default to method (contains text for parameters)
                                imageindex = 2;
                                if (items[n].Tag is MemberTypes)
                                {
                                    MemberTypes memberType = (MemberTypes)items[n].Tag;

                                    switch (memberType)
                                    {
                                        case MemberTypes.Custom:
                                            imageindex = 1;
                                            break;
                                        case MemberTypes.Property:
                                            imageindex = 3;
                                            break;
                                        case MemberTypes.Event:
                                            imageindex = 4;
                                            break;
                                    }
                                }
                            }

                            listBoxAutoComplete.Items.Add(new IntellisenseListBoxItem(items[n].DisplayText, imageindex));
                        }
                    }
                }
            }

            return result;
        }

        private string getLastWord()
        {
            string word = "";

            int pos = txtScript.SelectionStart;
            if (pos > 1)
            {

                string tmp = "";
                char f = new char();
                while (f != ' ' && f != 10 && pos > 0)
                {
                    pos--;
                    tmp = txtScript.Text.Substring(pos, 1);
                    f = tmp[0];
                    word += f;
                }

                char[] ca = word.ToCharArray();
                Array.Reverse(ca);
                word = new String(ca);

            }
            return word.Trim();
        }

        private void searchTree(TreeNodeCollection treeNodes, string path, bool continueUntilFind)
        {
            if (foundNode)
            {
                return;
            }

            string p = "";
            int n = 0;
            n = path.IndexOf(".");

            if (n != -1)
            {
                p = path.Substring(0, n);

                if (currentPath != "")
                {
                    currentPath += "." + p;
                }
                else
                {
                    currentPath = p;
                }

                // Knock off the first part
                path = path.Remove(0, n + 1);
            }
            else
            {
                currentPath += "." + path;
            }

            for (int i = 0; i < treeNodes.Count; i++)
            {
                if (treeNodes[i].FullPath == currentPath)
                {
                    if (continueUntilFind)
                    {
                        nameSpaceNode = treeNodes[i];
                    }

                    nameSpaceNode = treeNodes[i];

                    // got a dot, continue, or return
                    this.searchTree(treeNodes[i].Nodes, path, continueUntilFind);

                }
                else if (!continueUntilFind)
                {
                    foundNode = true;
                    return;
                }
            }
        }

        private void findNode(string path, TreeNodeCollection treeNodes)
        {
            for (int i = 0; i < treeNodes.Count; i++)
            {
                if (treeNodes[i].FullPath == path)
                {
                    findNodeResult = treeNodes[i];
                    break;
                }
                else if (treeNodes[i].Nodes.Count > 0)
                {
                    findNode(path, treeNodes[i].Nodes);
                }
            }
        }

        private void selectItem()
        {
            if (wordMatched)
            {
                int selstart = txtScript.SelectionStart;
                int prefixend = txtScript.SelectionStart - typed.Length;
                int suffixstart = txtScript.SelectionStart + typed.Length;

                if (suffixstart >= txtScript.Text.Length)
                {
                    suffixstart = txtScript.Text.Length;
                }

                string prefix = txtScript.Text.Substring(0, prefixend);
                string fill = listBoxAutoComplete.SelectedItem.ToString();
                string suffix = txtScript.Text.Substring(suffixstart, txtScript.Text.Length - suffixstart);

                txtScript.Text = prefix + fill + suffix;
                txtScript.SelectionStart = prefix.Length + fill.Length;
            }
        }
    }

    public class MemberItem : IComparable
    {
        public string DisplayText;
        public object Tag;

        public int CompareTo(object obj)
        {
            int result = 1;
            if (obj != null)
            {
                if (obj is MemberItem)
                {
                    MemberItem memberItem = (MemberItem)obj;
                    return (this.DisplayText.CompareTo(memberItem.DisplayText));
                }
                else
                {
                    throw new ArgumentException();
                }
            }


            return result;
        }
    }
}
