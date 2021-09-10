using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private string unixtimePathToDatePath (string unixtimePath)
        {
            string filename = Path.GetFileName(unixtimePath);
            string dir = Path.GetDirectoryName(unixtimePath);

            var segments = new List<string>();
            segments.AddRange(filename.Split('.'));
            string ext = segments[segments.Count - 1];
            segments.RemoveAt(segments.Count - 1);


            string[] new_src;
            new_src = segments.ToArray();
            string basename = String.Join(".", new_src).Substring(0, 10);

            if (long.TryParse(basename, out long result))
            {
                DateTime datetime = DateTimeOffset.FromUnixTimeSeconds(result).LocalDateTime;
                string ymdhis = datetime.ToString(textBoxDateFormat.Text);

                return dir + "\\" + ymdhis + "." + ext;
            }

            return "";
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            //コントロール内にドロップされたとき実行される
            //ドロップされたすべてのファイル名を取得する
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            //ListBoxに追加する
            foreach (string path in paths)
            {
                Boolean isExists = false;

                foreach (ListViewItem item in this.listView1.Items)
                {
                    if (item.Text == path)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (!isExists)
                {
                    string datePath = this.unixtimePathToDatePath(path);

                    if (datePath != "")
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = path;                    // 1列目に表示するテキスト
                        item.ImageIndex = 0;                 // イメージのインデックス番号
                        item.SubItems.Add(datePath);     // 2列目に表示するテキスト

                        this.listView1.Items.Add(item);
                    }
                }
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (!File.Exists(item.SubItems[1].Text))
                {
                    File.Move(item.Text, item.SubItems[1].Text);
                }
            }

            this.listView1.Items.Clear();
        }

        private void textBoxDateFormat_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxDateFormat.Text == "")
            {
                return;
            }

            foreach (ListViewItem item in this.listView1.Items)
            {
                item.SubItems[1].Text = this.unixtimePathToDatePath(item.Text);
            }

        }
    }
}
