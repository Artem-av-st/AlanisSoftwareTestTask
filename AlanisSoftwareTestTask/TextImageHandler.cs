
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AlanisSoftwareTestTask
{
    public class TextImageHandler
    {
        private readonly string _inputImagePath;
        public TextImageHandler(string inputImagePath)
        {
            if(!File.Exists(inputImagePath))
            {
                throw new FileNotFoundException("Input image does not exists", inputImagePath);
            }

            _inputImagePath = inputImagePath;
        }
        public void DrawWordsContours(string outputImagePath)
        {
            var wordsContours = FindWordsRetangles(_inputImagePath);

            DrawRectangles(wordsContours, _inputImagePath, outputImagePath);
        }

        public void DrawTextRegionsContours(string outputImagePath)
        {
            var textContours = FindTextRegions(_inputImagePath);

            DrawRectangles(textContours, _inputImagePath, outputImagePath);
        }

        #region private methods
        private void DrawRectangles(IEnumerable<Rect> rectangles, string inputImagePath, string outputImagePath, int lineThickness = 1)
        {
            using (Mat sourceImage = new Mat(inputImagePath))
            {
                foreach (var rect in rectangles)
                {                    
                    Cv2.Rectangle(sourceImage, rect, new Scalar(0, 0, 255), lineThickness);
                }
                sourceImage.SaveImage(outputImagePath);               
            }
        }

        private IEnumerable<Rect> FindWordsRetangles(string inputImagePath)
        {            
            using (Mat sourceImage = new Mat(inputImagePath))
            using (Mat formattedImage = new Mat())           
            {                
                // Make image greyscale
                Cv2.CvtColor(sourceImage, formattedImage, ColorConversionCodes.BGR2GRAY);                
                
                // Сonstruct a rect
                using (Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(15, 3)))
                {
                    // And apply it to the image
                    Cv2.MorphologyEx(formattedImage, formattedImage, MorphTypes.Gradient, element);
                    
                    // Look for countours
                    var countoursArray = formattedImage.Clone().FindContoursAsArray(RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    return CreateRectsFromCountours(countoursArray);                   
                }                
            }
        }

        private IEnumerable<Rect> CreateRectsFromCountours(IEnumerable<IEnumerable<Point>> countours)
        {
            foreach (var edges in countours)
            {
                yield return Cv2.BoundingRect(edges);
            }
        }

        private IEnumerable<Rect> FindTextRegions(string inputImagePath)
        {
            var wordsRectangles = FindWordsRetangles(inputImagePath).ToList();            
            
            var outputRects = new List<Rect>();

            // Merge intersected or close rectangles
            while (wordsRectangles.Any())
            {
                var current = wordsRectangles[0];
                wordsRectangles.Remove(current);
                AggregateRectangles(current, wordsRectangles, ref outputRects);
            }           

            return outputRects;
        }

        private  void AggregateRectangles(Rect rect, List<Rect> inputRects, ref List<Rect> outputRects)
        {
            var intersectedRect = inputRects.FirstOrDefault(r =>
            {   
                // Inflate to merge close rectangles
                r.Inflate(10, 12);
                return r.IntersectsWith(rect);
            });
            
            if (intersectedRect.Height != 0 && intersectedRect.Width != 0)
            {                
                inputRects.Remove(rect);
                inputRects.Remove(intersectedRect);
                AggregateRectangles(rect.Union(intersectedRect), inputRects, ref outputRects);                
            }
            else
            {
                outputRects.Add(rect);                
            }
            
        }
        #endregion private methods
    }
}
