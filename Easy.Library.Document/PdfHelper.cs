using System;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Cells;
using Aspose.Words;
using Aspose.Words.Drawing;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PageSetup = Aspose.Words.PageSetup;
using Path = System.IO.Path;
using SaveFormat = Aspose.Words.SaveFormat;

namespace Easy.Library.Document
{
    public class PdfHelper
    {
        public static string ConvertToPdf(string fullPath,string id)
        {
            string filename = Path.GetFileName(fullPath);
            string extension = Path.GetExtension(fullPath);
            string pdfName = filename.Replace(extension, "") +id+ ".pdf";
            string[] excel = new string[] { "xls", "xlsx" };
            string[] word = new string[] { "doc", "docx" };
            string[] img = new string[] { "jpg", "png", "gif", "jpeg", "bmp" };
            string extensionType = extension.ToLower().Replace(".", "");
            string path = "/Attachment/PreViewPdf/";
            string w_pdfName = AppDomain.CurrentDomain.BaseDirectory + path.Replace("/", "\\") + pdfName;
            if (File.Exists(w_pdfName))
            {
                return path + pdfName;
            }
            if(!Directory.Exists(Path.GetDirectoryName(w_pdfName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(w_pdfName));
            }
            try
            {
                if (excel.Contains(extensionType))
                {
                    ExcelToPdf(fullPath, w_pdfName);
                }
                else if (img.Contains(extensionType))
                {
                    ImageToPdf(fullPath, w_pdfName);
                }
                else if (extensionType.Equals("pdf"))
                {
                    File.Copy(fullPath, w_pdfName);
                }
                else if (extensionType.Equals("txt"))
                {
                    TxtToPdf(fullPath, w_pdfName);
                }
                else if (word.Contains(extensionType))
                {
                    WordToPdf(fullPath, w_pdfName);
                }
                else
                {
                    return null;
                }
                return path + pdfName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private static void WordToPdf(string fullPath, string pdfName)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(fullPath);
            doc.Save(pdfName, SaveFormat.Pdf);

        }

        private static void TxtToPdf(string fullPath, string pdfName)
        {
            string str = System.IO.File.ReadAllText(fullPath, System.Text.Encoding.Default);
            System.IO.File.WriteAllText(fullPath, str, System.Text.Encoding.UTF8);
            Aspose.Words.Document doc = new Aspose.Words.Document(fullPath);
            doc.Save(pdfName, SaveFormat.Pdf);

        }

        private static void ExcelToPdf(string fullPath, string pdfName)
        {
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(fullPath);
            wb.Save(pdfName, Aspose.Cells.SaveFormat.Pdf);

        }

        private static void ImageToPdf(string fullPath, string pdfName)
        {
            ConvertImageToPdf(fullPath, pdfName);
        }
        private static void ConvertImageToPdf(string inputFileName, string outputFileName)
        {
            // Create Aspose.Words.Document and DocumentBuilder.
            // The builder makes it simple to add content to the document.
            Aspose.Words.Document doc = new Aspose.Words.Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Read the image from file, ensure it is disposed.
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(inputFileName))
            {
                // Find which dimension the frames in this image represent. For example
                // the frames of a BMP or TIFF are "page dimension" whereas frames of a GIF image are "time dimension".
                FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);

                int framesCount = image.GetFrameCount(dimension);
                // Get the number of frames in the image.
                framesCount = image.GetFrameCount(FrameDimension.Page);


                // Loop through all frames.
                for (int frameIdx = 0; frameIdx < framesCount; frameIdx++)
                {
                    // Insert a section break before each new page, in case of a multi-frame TIFF.
                    if (frameIdx != 0)
                        builder.InsertBreak(BreakType.SectionBreakNewPage);

                    // Select active frame.
                    image.SelectActiveFrame(dimension, frameIdx);

                    // We want the size of the page to be the same as the size of the image.
                    // Convert pixels to points to size the page to the actual image size.
                    PageSetup ps = builder.PageSetup;
                    ps.PageWidth = ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
                    ps.PageHeight = ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);

                    // Insert the image into the document and position it at the top left corner of the page.
                    builder.InsertImage(
                        image,
                        RelativeHorizontalPosition.Page,
                        0,
                        RelativeVerticalPosition.Page,
                        0,
                        ps.PageWidth > 950 ? 950 : ps.PageWidth,
                        ps.PageHeight > 950 ? 950 : ps.PageHeight,
                        WrapType.None);
                }
            }

            // Save the document to PDF.
            doc.Save(outputFileName);
        }
        public static string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
    }
}
