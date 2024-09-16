using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Colors;

namespace PDF_Generator_Program.Utils
{
    public static class Tool
    {
        /// <summary>
        /// "MM-dd-yyyy_HH-mm-ss"
        /// </summary>
        public static string s_StandardDateFormat = "MM/dd/yyyy HH:mm:ss";

        /// <summary>
        /// Write in Console + Throw Exception
        /// </summary>
        /// <param name="msgError"></param>
        public static void LogError(ErrorType errorType, string msgError)
        {
            string s_date = DateTime.Now.ToString(s_StandardDateFormat);
            string s_finalMessage = $"[{s_date}] ERROR - {errorType.ToString()} : {msgError} \n\r";
            Console.WriteLine(s_finalMessage);

            switch (errorType)
            {
                case ErrorType.ArgumentException:
                    throw new ArgumentException(s_finalMessage);
                case ErrorType.Exception:
                    throw new Exception(s_finalMessage);
                case ErrorType.FileNotFoundException:
                    throw new FileNotFoundException(s_finalMessage);
                case ErrorType.FormatException:
                    throw new FormatException(s_finalMessage);
                case ErrorType.IndexOutOfRangeException:
                    throw new IndexOutOfRangeException(s_finalMessage);
                case ErrorType.KeyNotFoundException:
                    throw new KeyNotFoundException(s_finalMessage);
                case ErrorType.NullReferenceException:
                    throw new NullReferenceException(s_finalMessage);
                case ErrorType.TimeoutException:
                    throw new TimeoutException(s_finalMessage);
                default:
                    throw new Exception(s_finalMessage);
            }
        }
        /// <summary>
        /// Write in Console
        /// </summary>
        public static void LogInfo(string infoMsg)
        {
            string s_date = DateTime.Now.ToString(s_StandardDateFormat);
            string s_finalMessage = $"[{s_date}] INFO : {infoMsg} \n\r";
            Console.WriteLine(s_finalMessage);
        }

        /// <summary>
        /// Get the path from the path of the project : "PDF_Generator_Program" (and not from the executable directory !)
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string GetFullPath(string relativePath)
        {
            const string programName = "PDF_Generator";
            string currentDirectory = Directory.GetCurrentDirectory();
            string baseDirectory = currentDirectory.Substring(0, currentDirectory.IndexOf(programName) + programName.Length);
            return Path.Combine(baseDirectory, relativePath);
        }

        /// <summary>
        /// Generate a pdf with a logo, title and data table
        /// </summary>
        /// <param name="ts_columnNames"></param>
        /// <param name="lts_dataRow"></param>
        /// <param name="tableColor">Background color of the table</param>
        /// <param name="s_logoPath"></param>
        /// <param name="s_outputPathfile"></param>
        public static void Generate_PDF_with_DataTable(string[] ts_columnNames, List<string[]> lts_dataRow, Color tableColor, string s_logoPath, string s_outputPathfile)
        {
            int CountryColPosition = -1;
            string s_countryMap_FilePath;
            string[] mapping_data;

            for (int i = 0; i < ts_columnNames.Length; i++)
            {
                if (ts_columnNames[i].Equals("CountryCo"))
                {
                    CountryColPosition = i;
                    ts_columnNames[i] = "Country";
                }
            }

            s_countryMap_FilePath = GetFullPath("Param\\Country_Mapping.txt");
            mapping_data = File.ReadAllLines(s_countryMap_FilePath);
            Dictionary<string, string> countryMap = new Dictionary<string, string>();
            foreach (var line in mapping_data)
            {
                string[] parts = line.Split(';');
                countryMap[parts[0]] = parts[1];
            }

            using (PdfWriter writer = new PdfWriter(s_outputPathfile))
            {
                // Ouvre un document PDF avec ce writer
                using (PdfDocument pdfDoc = new PdfDocument(writer))
                {
                    Document document = new Document(pdfDoc);

                    // Charger l'image (logo PNG)
                    ImageData logoData = ImageDataFactory.Create(s_logoPath);
                    Image logo = new Image(logoData);

                    // Positionner le logo en haut à gauche (ajustez la taille si nécessaire)
                    logo.SetFixedPosition(50, pdfDoc.GetDefaultPageSize().GetHeight() - 150); // Position en haut à gauche
                    logo.SetHeight(100); // Taille du logo (ajustable)
                    document.Add(logo); // Ajouter le logo au document

                    document.Add(new Paragraph("\n\r \n\r \n\r \n\r"));
                    Paragraph Title = new Paragraph("Customer Data List")
                                            .SetTextAlignment(TextAlignment.CENTER)
                                            .SetItalic()
                                            .SetFontSize(24);

                    document.Add(Title);
                    document.Add(new Paragraph("\n\r"));

                    // Crée un tableau 
                    Table table = new Table(ts_columnNames.Length);
                    table.SetWidth(UnitValue.CreatePercentValue(80))
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                    // Ajout des en-têtes (première ligne de la liste)
                    for (int i = 0; i < ts_columnNames.Length; i++)
                    {
                        if (ts_columnNames[i].Equals("CountryCo"))
                        {
                            CountryColPosition = i;
                            ts_columnNames[i] = "Country";
                        }
                        table.AddHeaderCell(new Cell().Add(new Paragraph(ts_columnNames[i])
                                                        .SetBold()
                                                        .SetTextAlignment(TextAlignment.CENTER)));
                    }

                    foreach (string[] row in lts_dataRow)
                    {
                        for(int i = 0; i < row.Length; i++)
                        {
                            Cell cell = new Cell();
                            if (i == CountryColPosition)
                                cell.Add(new Paragraph(GetValueFromKey(row[i], countryMap)));
                            else
                                cell.Add(new Paragraph(row[i]));

                            cell.SetBackgroundColor(tableColor); // Définit la couleur de fond
                            table.AddCell(cell);
                        }
                    }

                    // Ajoute le tableau au document
                    document.Add(table);

                    // Ferme le document
                    document.Close();
                }
            }
            LogInfo($"PDF généré avec succès dans : {s_outputPathfile}");
        }

        /// <summary>
        /// Get the value associated to the key in a dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static string GetValueFromKey(string key, Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return key;
        }

    }

}
