using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace Antykutasator.FaceDetection
{
    public class OpenCvFaceClassifier
    {
        #region Variables

        //Eigen
        //EigenObjectRecognizer recognizer;
        FaceRecognizer recognizer;

        //training variables
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> _namesList = new List<string>(); //labels
        int _contTrain, _numLabels;
        int _eigenThreshold = 3000;

        //Class Variables

        private RecognizerType _recognizerType = RecognizerType.EigenFaceRecognizer;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor, Looks in (Application.StartupPath + "\\TrainedFaces") for traing data.
        /// </summary>
        public OpenCvFaceClassifier()
        {
            IsTrained = LoadTrainingData();
        }

        /// <summary>
        /// Takes String input to a different location for training data
        /// </summary>
        /// <param name="trainingFolder"></param>
        public OpenCvFaceClassifier(string trainingFolder)
        {
            IsTrained = LoadTrainingData();
        }
        #endregion

        #region Public
        /// <summary>
        /// Retrains the recognizer witout resetting variables like recognizer type.
        /// </summary>
        /// <returns></returns>
        public bool Retrain()
        {
            return IsTrained = LoadTrainingData();
        }
        /// <summary>
        /// Retrains the recognizer witout resetting variables like recognizer type.
        /// Takes String input to a different location for training data.
        /// </summary>
        /// <returns></returns>
        public bool Retrain(string trainingFolder)
        {
            return IsTrained = LoadTrainingData();
        }

        /// <summary>
        /// <para>Return(True): If Training data has been located and Eigen Recogniser has been trained</para>
        /// <para>Return(False): If NO Training data has been located of error in training has occured</para>
        /// </summary>
        public bool IsTrained { get; private set; } = false;

        /// <summary>
        /// Recognise a Grayscale Image using the trained Eigen Recogniser
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="eigenThresh"></param>
        /// <returns></returns>
        public bool Recognise(Image<Gray, byte> inputImage, int eigenThresh = -1)
        {
            if (!IsTrained) return false;

            try
            {
                var er = recognizer.Predict(inputImage);

                if (er.Label == -1)
                {
                    GetEigenLabel = "Unknown";
                    GetEigenDistance = 0;
                    return false;
                }
                GetEigenLabel = _namesList[er.Label];
                GetEigenDistance = (float)er.Distance;
                if (eigenThresh > -1) _eigenThreshold = eigenThresh;

                //Only use the post threshold rule if we are using an Eigen Recognizer 
                //since Fisher and LBHP threshold set during the constructor will work correctly 
                switch (_recognizerType)
                {
                    case RecognizerType.EigenFaceRecognizer:
                        return GetEigenDistance > _eigenThreshold;
                    default:
                        return true; //the threshold set in training controls unknowns
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the threshold confidence value for string Recognise(Image<Gray, byte> Input_image) to be used.
        /// </summary>
        public int SetEigenThreshold
        {
            set
            {
                //NOTE: This is still not working correctley 
                //recognizer.EigenDistanceThreshold = value;
                _eigenThreshold = value;
            }
        }

        /// <summary>
        /// Returns a string containg the recognised persons name
        /// </summary>
        public string GetEigenLabel { get; private set; }

        /// <summary>
        /// Returns a float confidence value for potential false clasifications
        /// </summary>
        public float GetEigenDistance { get; private set; } = 0;

        /// <summary>
        /// Returns a string contatining any error that has occured
        /// </summary>
        public string GetError { get; private set; }

        /// <summary>
        /// Loads the trained Eigen Recogniser from specified location
        /// </summary>
        /// <param name="filename"></param>
        public void LoadEigenRecogniser(string filename)
        {
            //Lets get the recogniser type from the file extension
            string ext = Path.GetExtension(filename);
            switch (ext)
            {
                case (".LBPH"):
                    _recognizerType = RecognizerType.LBPHFaceRecognizer;
                    recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);//50
                    break;
                case (".FFR"):
                    _recognizerType = RecognizerType.FisherFaceRecognizer;
                    recognizer = new FisherFaceRecognizer(0, 3500);//4000
                    break;
                case (".EFR"):
                    _recognizerType = RecognizerType.EigenFaceRecognizer;
                    recognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
                    break;
            }

            //introduce error checking
            recognizer.Load(filename);

            //Now load the labels
            string direct = Path.GetDirectoryName(filename);
            _namesList.Clear();
            if (File.Exists(direct + "/Labels.xml"))
            {
                FileStream filestream = File.OpenRead(direct + "/Labels.xml");
                long filelength = filestream.Length;
                byte[] xmlBytes = new byte[filelength];
                filestream.Read(xmlBytes, 0, (int)filelength);
                filestream.Close();

                MemoryStream xmlStream = new MemoryStream(xmlBytes);

                using (XmlReader xmlreader = XmlTextReader.Create(xmlStream))
                {
                    while (xmlreader.Read())
                    {
                        if (xmlreader.IsStartElement())
                        {
                            switch (xmlreader.Name)
                            {
                                case "NAME":
                                    if (xmlreader.Read())
                                    {
                                        _namesList.Add(xmlreader.Value.Trim());
                                    }
                                    break;
                            }
                        }
                    }
                }
                _contTrain = _numLabels;
            }
            IsTrained = true;

        }

        #endregion

        #region Private

        /// <summary>
        /// Loads the traing data given a (string) folder location
        /// </summary>
        /// <returns></returns>
        private bool LoadTrainingData()
        {
            try
            {
                //Load of previus trainned faces and labels for each image
                var labelsinfo = File.ReadAllText(Application.StartupPath + "\\Resources\\TrainedFaces\\TrainedLabels.txt");
                var labels = labelsinfo.Split('%');
                _numLabels = Convert.ToInt16(labels[0]);
                _contTrain = _numLabels;

                for (var tf = 1; tf < _numLabels + 1; tf++)
                {
                    var loadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "\\Resources\\TrainedFaces\\" + loadFaces));
                    _namesList.Add(labels[tf]);
                }

                if (trainingImages.ToArray().Length == 0) return false;

                switch (_recognizerType)
                {
                    case RecognizerType.LBPHFaceRecognizer:
                        recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);//50
                        break;
                    case RecognizerType.FisherFaceRecognizer:
                        recognizer = new FisherFaceRecognizer(0, 3500);//4000
                        break;
                    case RecognizerType.EigenFaceRecognizer:
                        recognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
                        break;
                    default:
                        recognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
                        break;
                }

                recognizer.Train(trainingImages.ToArray(), Enumerable.Range(0, _namesList.Count).ToArray());
                return true;
            }
            catch (Exception ex)
            {
                GetError = ex.ToString();
                return false;
            }
        }

        #endregion
    }
}
