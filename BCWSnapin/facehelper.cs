using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCWSnapin
{
    internal class facehelper
    {
        private readonly IFaceServiceClient faceServiceClient = null;
        Face[] faces;                   // The list of detected faces.
        String[] faceDescriptions;      // The list of descriptions for the detected faces.
        double resizeFactor;
        internal facehelper(string apikey)
        {
            faceServiceClient = new FaceServiceClient(apikey, "https://westeurope.api.cognitive.microsoft.com/face/v1.0");
            
        }
           
        internal async Task<FaceDO> ProcessFile(string filename)
        {
            var fc = new FaceDO();
            fc.FileName = filename;
            fc.faces = await UploadAndDetectFaces(fc.FileName);


            return fc;
        }
        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath)
        {
            // The list of Face attributes to return.
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.FacialHair, FaceAttributeType.Hair };

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    Face[] faces = await faceServiceClient.DetectAsync(imageFileStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    return faces;
                }
            }
            // Catch and display Face API errors.
            catch (FaceAPIException f)
            {
                //MessageBox.Show(f.ErrorMessage, f.ErrorCode);
                return new Face[0];
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error");
                return new Face[0];
            }
        }
    }
    internal class FaceDO
    {
        public string FileName { get; set; }
        public Bitmap bitmap { get; set; }
        public Face[] faces { get; set; }

        public Face GetFace()
        {
            if (faces.Length > 0)
                return faces[0];
            else
                return null;
        }

        public Image GetImage()
        {
            Image f = null;
            Image img = null;
            try {
                img = Image.FromFile(this.FileName);
                f =(Image)img.Clone();
                
            }
            finally
            {
                img.Dispose();
            }
            return f;
        }
        public Rectangle GetFaceRect()
        {
            var rz = Convert.ToInt32(96 / GetImage().HorizontalResolution);

            return new System.Drawing.Rectangle(
                GetFace().FaceRectangle.Left * rz, 
                GetFace().FaceRectangle.Top *rz, 
                GetFace().FaceRectangle.Width*rz, 
                GetFace().FaceRectangle.Height*rz);
        }
    }
}
