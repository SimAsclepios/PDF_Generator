using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Colors;
using PDF_Generator_Program.Utils;

Tool.LogInfo("Start : PDF Generator Program");

const int ci_columnCount_max = 3;   // Maximum number of columns that the txt file can contain
const int ci_lineCount_max = 20;    // Maximum number of rows    that the txt file can contain
const int ci_lineLength_max = 64;   // Maximum length in characters that a line in the file can have

string s_logoPath = Tool.GetFullPath("Resources\\PDF_Generator_Program_icon.png");  // Path of the logo used in the PDF
string s_inputFilePath = Tool.GetFullPath("Input\\Customer_List.txt");              // Path of the file that contains the data for the PDF table
string s_outputPdfFilePath = Tool.GetFullPath($"Output\\CustomerList_ {DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss")} .pdf"); // Path of the generated pdf file

Color color_green = new DeviceRgb(144, 238, 144);   // Background color of the PDF table

List<string> ls_fileLines;  // Contain all lines of the input file
string[] ts_columnNames;    // Contain Column names only 
List<string[]> lts_dataRow; // Contain data row without columns names


// ----------------------------- 1 - File ---------------------------- //
Tool.LogInfo("Step 1 - Read Data File");

if (!File.Exists(s_inputFilePath))
    Tool.LogError(ErrorType.FileNotFoundException, $"ERROR : The File '{s_inputFilePath}' is not found.");

ls_fileLines = File.ReadAllLines(s_inputFilePath).ToList();

// File Format Exception
if (ls_fileLines.Count == 0)
    Tool.LogError(ErrorType.FormatException, "The file is empty.");
else if (ls_fileLines.Count == 1)
    Tool.LogError(ErrorType.FormatException, $"In the input file : '{s_inputFilePath}', There are only column names (1st line) without data (line 2+).");
if (ls_fileLines.Count > ci_lineCount_max + 1)
    Tool.LogError(ErrorType.FormatException, $"The file contains too many lines : {ls_fileLines.Count}. It doesn't exceed {ci_lineCount_max + 1} lines");

ts_columnNames = ls_fileLines[0].Split(';');

if (ts_columnNames.Length > ci_columnCount_max)
{
    Tool.LogInfo($"The number of columns is {ts_columnNames.Length} and exceeds the limit of {ci_columnCount_max}.");
    return;
}

lts_dataRow = new List<string[]>();

for (int i = 1; i < ls_fileLines.Count; i++)
{
    if (ls_fileLines[i].Length > ci_lineLength_max)
        Tool.LogError(ErrorType.FormatException, $"The line {i} contains {ls_fileLines[i].Length} characters and exceeds the maximum allowed length of {ci_lineLength_max} caractères.");
 
    string[] values = ls_fileLines[i].Split(';');

    if (values.Length > ts_columnNames.Length)
        Tool.LogError(ErrorType.FormatException,$"The line {i} contains more columns than expected : {values.Length} instead of {ts_columnNames.Length}.");

    lts_dataRow.Add(values);
}

// ------------------------- 2 - PDF GENERATION ----------------------------- //
Tool.LogInfo("Step 2 - Generate PDF");
Tool.Generate_PDF_with_DataTable(ts_columnNames, lts_dataRow, color_green, s_logoPath, s_outputPdfFilePath);
Tool.LogInfo("DONE");