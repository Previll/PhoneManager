using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
namespace CCIERantsPhoneControl
{
    public class ScreenshotHolderClass : INotifyPropertyChanged
    {
        public Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            System.Drawing.Imaging.ColorMatrix colormatrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = opacityvalue };
            System.Drawing.Imaging.ImageAttributes imgAttribute = new System.Drawing.Imaging.ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics 
            return bmp;
        }

        private Image curScreenshot;
        public Image CurrentScreenshot
        {
            get
            {
                if (curScreenshot == null)
                {
                    return DefaultScreenshot;
                }
                else
                {
                 
                        return curScreenshot;
                    
                }
            }
            set
            {
                if (value == null)
                {

                    curScreenshot = DefaultScreenshot;
                    OnPropertyChanged("CurrentScreenshot");

                }
                else
                {
                    curScreenshot = value;
                    OnPropertyChanged("CurrentScreenshot");

                }
            }
        }
        public Image DefaultScreenshot
        {
            get { return Properties.Resources.PhoneScreenshot; }

        }

        public Image InitialSetupScreenshot
        {
            get { return Properties.Resources.PhoneScreenshotInitialSetup; }

        }
        public Image BlankImage
        {
            get
            {
                return new Bitmap(1, 1);

            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
