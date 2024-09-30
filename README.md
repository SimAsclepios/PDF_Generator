# PDF_Generator
PDF Generator .NET Program from txt File

This project is a C# program that reads data from a txt file, 
processes it, and generates a PDF file with the data formatted in a table. 

There is a mapping file with values of country code and the associated country name.
We can add new [Country code ; Country name] in the file, and the program will manage it.
We can add Country Code in the input file, and in the pdf we will see the associated country name if it exist in the mapping file.

The program handles up to 3 columns and 20 rows for the table.
Each row can't exceed 64 caracters.

It also includes error handling for file input/output operations and data validation.

The input file is pre-filled with data, containing 3 columns and 5 rows of data.


# Requirements
- Visual Studio Community 2022
- .NET 6.0 or later

# Installation
Clone this repository
