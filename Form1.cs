using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PI_Project
{
    public partial class ImageEffects : Form
    {
        public ImageEffects()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e){
            OpenFileDialog Opfile = new OpenFileDialog();
            Opfile.Filter = "Image File (*.bmp,*.png)| *.bmp;*.png";
            if (DialogResult.OK == Opfile.ShowDialog()) {
                this.pictureBox1.Image = new Bitmap(Opfile.FileName);
                button2.Enabled = true;
            }
                
        }

        private void button2_Click(object sender, EventArgs e){
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG files(*.jpeg)|*.jpeg";
            if (DialogResult.OK == sfd.ShowDialog())
                this.pictureBox1.Image.Save(sfd.FileName, ImageFormat.Jpeg);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
