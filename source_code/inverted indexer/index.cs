using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace inverted_indexer
{
    public partial class index : Form
    {
        FolderBrowserDialog fld = new FolderBrowserDialog();
        ArrayList load = new ArrayList();
        ArrayList terms = new ArrayList();
        public index()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = @"C:\Users\HP\Desktop\index";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.Visible == true)
            {
                listBox1.Hide();
                progressBar1.Location = new Point(12, 92);
                button4.Text = "Show detail";
            }
            else if(listBox1.Visible == false)
            {
                listBox1.Show();
                progressBar1.Location = new Point(12, 271);
                button4.Text = "Hide detail";
            }
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
            fld.ShowDialog();
            if (fld.SelectedPath != "")
            {
                textBox1.Text = fld.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = 0;
            load.Clear();
            if (Directory.Exists(textBox1.Text))
            {
                string[] files = Directory.GetFiles(textBox1.Text);
                Directory.CreateDirectory(textBox1.Text + @"\index");
                StreamWriter wrt = new StreamWriter(textBox1.Text + @"\index\vocabulary.txt", false);
                StreamWriter wrt2 = new StreamWriter(textBox1.Text + @"\index\posting.txt", false);
                listBox1.Items.Clear();
                progressBar1.Value = 0;
                foreach (string str in Directory.EnumerateFiles(textBox1.Text))
                {
                    i++;
                    load.Add(File.ReadAllText(str));
                    listBox1.Items.Add(i + " : " + str + " ... Done");
                }
                terms.Clear();
                //tokenization
                for (int lp = 0; lp < i; lp++)
                {
                    foreach (Match res in Regex.Matches(load[lp].ToString(), @"\w*\s"))
                    {
                        terms.Add(res.ToString());
                        wrt2.Write(res + "\t" + files[lp]);
                        wrt2.WriteLine();
                    }
                }
                //sorting
                terms.Sort();
                //stopword removal
                string tm = "";
                for (int lp3 = 0; lp3 < terms.Count; lp3++)
                {
                    tm = terms[lp3].ToString();
                    if (Regex.IsMatch(tm, @"be") || Regex.IsMatch(tm, @"it") ||
                       Regex.IsMatch(tm, @"did") || Regex.IsMatch(tm, @"let") ||
                       Regex.IsMatch(tm, @"has") || Regex.IsMatch(tm, @"me") ||
                       Regex.IsMatch(tm, @"(i|I)") || Regex.IsMatch(tm, @"so") ||
                       Regex.IsMatch(tm, @"the") || Regex.IsMatch(tm, @"told") ||
                       Regex.IsMatch(tm, @"you") || Regex.IsMatch(tm, @"was") ||
                       Regex.IsMatch(tm, @"with") || Regex.IsMatch(tm, @"are") ||
                       Regex.IsMatch(tm, @"can") || Regex.IsMatch(tm, @"by") ||
                       Regex.IsMatch(tm, @"and") || Regex.IsMatch(tm, @"a") || Regex.IsMatch(tm, @"\s"))
                    {
                        terms.RemoveAt(lp3);
                    }
                }
                //stemming
                string stm = "";
                for (int lp4 = 0; lp4 < terms.Count; lp4++)
                {
                    stm = terms[lp4].ToString();
                    if (Regex.IsMatch(terms[lp4].ToString(), @"\w*ous") ||
                        Regex.IsMatch(terms[lp4].ToString(), @"\w*ing"))
                    {
                        stm = stm.Remove(stm.Length - 4);
                        terms[lp4] = stm;
                    }
                    else if (Regex.IsMatch(terms[lp4].ToString(), @"\w*ed") ||
                             Regex.IsMatch(terms[lp4].ToString(), @"\w*er"))
                    {
                        stm = stm.Remove(stm.Length - 3);
                        terms[lp4] = stm;
                    }
                    else if (Regex.Matches(terms[lp4].ToString(), @"\w*n") != null)
                    {
                        stm = stm.Remove(stm.Length - 1);
                        terms[lp4] = stm;
                    }
                }
                string trm = "";
                int tf = 0;
                foreach (string str in terms)
                {
                    trm = str + "\n";
                }
                //compute tf
                for (int lp2 = 0; lp2 < terms.Count; lp2++)
                {
                    trm = terms[lp2].ToString();
                    foreach (Match res in Regex.Matches(trm, terms[lp2].ToString()))
                    {
                        tf++;
                        terms.Remove(res.ToString());
                    }
                    terms.Insert(lp2, trm + "," + tf);
                    wrt.Write(terms[lp2]);
                    wrt.WriteLine();
                    tf = 0;
                }
                //split
                button2.Text = "Refresh";
                button4.Show();
                listBox1.Show();
                progressBar1.Show();
                progressBar1.Value = 100;
                wrt.Close();
                wrt2.Close();
            }
            else
            {
                MessageBox.Show("folder doesn't exist choose existing folder", "warning");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Text = "Start";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (Int32.TryParse(textBox2.Text,out i))
            {
                if (i <= load.Count&& i>0)
                {
                    richTextBox1.Text = load[i-1].ToString();
                }
                else
                {
                    label2.Text = "index out of bound";
                }
            }
            else
            {
                label2.Text = "invalid index value";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Visible == false)
            {
                richTextBox1.Show();
                textBox2.Show();
                label3.Show();
                this.Size = new Size(779, 394);
                button5.Location = new Point(734, 136);
                button5.Text = "<";
            }
            else
            {
                richTextBox1.Hide();
                textBox2.Hide();
                label3.Hide();
                this.Size = new Size(434, 394);
                button5.Location = new Point(398, 136);
                button5.Text = ">";
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Product name: inverted indexer\n" +
                            "Version:v0.1\n" +
                            "Copyright:2018 nanosoft all rigths reserved\n" +
                            "Description:inverterd index file gen\n"+
                            "group member:\n" +
                            "\t1,naol areaga\n" +
                            "\t2,tebibu solomon\n" +
                            "\t3,simon mulugeta\n" +
                            "\t4,yonathan hilezghi\n" +
                            "\t5,lidya \n" +
                            "\t6,yeabsira getachew\n" +
                            "\t7,nahom nayzgi\n" +
                            "\t8,omer mohamed\n");
        }
    }
}