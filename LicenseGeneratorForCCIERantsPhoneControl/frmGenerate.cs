using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ccierants.baseclasses;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace LicenseGeneratorForCCIERantsPhoneControl
{
    public partial class frmGenerate : Form
    {
        public frmGenerate()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
           Console.WriteLine(dsa.ToXmlString(true));

           Console.WriteLine();
           Console.WriteLine();
           Console.WriteLine();
           Console.WriteLine();
           Console.WriteLine();

           DSACryptoServiceProvider dsaPub = new DSACryptoServiceProvider();
           dsaPub.FromXmlString(dsa.ToXmlString(true));

           Console.WriteLine(dsaPub.ToXmlString(false));


        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            
            // create the crypto-service provider:
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();

            // setup the dsa from the private key:
            dsa.FromXmlString(Properties.Resources.privateKey);

            // get the byte-array of the licence terms:
            byte[] contentToSign = Encoding.ASCII.GetBytes(textBox1.Text);


            // get the signature:
            byte[] signature = dsa.SignData(contentToSign);

            textBox2.Text = Encoding.ASCII.GetString(signature);
            

            

        }

        private bool isValidLicense(string LicenseContent, string Signature)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();

            // get the key
            dsa.FromXmlString(Properties.Resources.xmlMethod);
            // get the license terms data:
            byte[] terms = Encoding.ASCII.GetBytes(textBox1.Text);

            // get the signature data:
            byte[] signature = Encoding.ASCII.GetBytes(textBox2.Text);

            // verify that the license-terms match t
            return dsa.VerifyData(terms,signature);

            
                return false;
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(isValidLicense(textBox1.Text,textBox2.Text).ToString());
        }
        
    }
    
  

}
