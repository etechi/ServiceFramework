using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;

namespace SF.Utils.TableExports.Excel
{
	public class ExcelExporterFactory : ITableExporterFactory
    {
        public ITableExporter Create(System.IO.Stream Stream,string Title, Column[] Columns)
        {
            return new ExcelExporter(Stream, Title, Columns);
        }
    }
    class ExcelExporter  : BaseTableExporter,ITableExporter
    {
        public string ContentType { get; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string FileExtension { get; } = "xlsx";
        public string Title { get; }

        SheetData SheetData { get; }
        WorkbookPart WorkbookPart { get; }
        SpreadsheetDocument Document { get; }
        UInt32Value _rowIndex = new UInt32Value(0u);

        Stylesheet CreateStylesheet()
        {
			Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac x16r2" } };
			stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
			stylesheet1.AddNamespaceDeclaration("x16r2", "http://schemas.microsoft.com/office/spreadsheetml/2015/02/main");

			NumberingFormats numberingFormats1 = new NumberingFormats() { Count = (UInt32Value)1U };
			NumberingFormat numberingFormat1 = new NumberingFormat() { NumberFormatId = (UInt32Value)178U, FormatCode = "yyyy\\-mm\\-dd\\ hh:mm" };

			numberingFormats1.Append(numberingFormat1);

			Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

			Font font1 = new Font();
			FontSize fontSize1 = new FontSize() { Val = 11D };
			Color color1 = new Color() { Theme = (UInt32Value)1U };
			FontName fontName1 = new FontName() { Val = "等线" };
			FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
			FontCharSet fontCharSet1 = new FontCharSet() { Val = 134 };
			FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

			font1.Append(fontSize1);
			font1.Append(color1);
			font1.Append(fontName1);
			font1.Append(fontFamilyNumbering1);
			font1.Append(fontCharSet1);
			font1.Append(fontScheme1);

			Font font2 = new Font();
			FontSize fontSize2 = new FontSize() { Val = 9D };
			FontName fontName2 = new FontName() { Val = "等线" };
			FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
			FontCharSet fontCharSet2 = new FontCharSet() { Val = 134 };
			FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

			font2.Append(fontSize2);
			font2.Append(fontName2);
			font2.Append(fontFamilyNumbering2);
			font2.Append(fontCharSet2);
			font2.Append(fontScheme2);

			fonts1.Append(font1);
			fonts1.Append(font2);

			Fills fills1 = new Fills() { Count = (UInt32Value)2U };

			Fill fill1 = new Fill();
			PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			Fill fill2 = new Fill();
			PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			fills1.Append(fill1);
			fills1.Append(fill2);

			Borders borders1 = new Borders() { Count = (UInt32Value)1U };

			Border border1 = new Border();
			LeftBorder leftBorder1 = new LeftBorder();
			RightBorder rightBorder1 = new RightBorder();
			TopBorder topBorder1 = new TopBorder();
			BottomBorder bottomBorder1 = new BottomBorder();
			DiagonalBorder diagonalBorder1 = new DiagonalBorder();

			border1.Append(leftBorder1);
			border1.Append(rightBorder1);
			border1.Append(topBorder1);
			border1.Append(bottomBorder1);
			border1.Append(diagonalBorder1);

			borders1.Append(border1);

			CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };

			CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
			Alignment alignment1 = new Alignment() { Vertical = VerticalAlignmentValues.Center };

			cellFormat1.Append(alignment1);

			cellStyleFormats1.Append(cellFormat1);

			CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)2U };

			CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
			Alignment alignment2 = new Alignment() { Vertical = VerticalAlignmentValues.Center };

			cellFormat2.Append(alignment2);

			CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)178U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true };
			Alignment alignment3 = new Alignment() { Vertical = VerticalAlignmentValues.Center };

			cellFormat3.Append(alignment3);

			cellFormats1.Append(cellFormat2);
			cellFormats1.Append(cellFormat3);

			CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
			CellStyle cellStyle1 = new CellStyle() { Name = "常规", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

			cellStyles1.Append(cellStyle1);
			DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
			TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

			StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

			StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
			stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
			X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

			stylesheetExtension1.Append(slicerStyles1);

			StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
			stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

			stylesheetExtension2.Append(timelineStyles1);

			stylesheetExtensionList1.Append(stylesheetExtension1);
			stylesheetExtensionList1.Append(stylesheetExtension2);

			stylesheet1.Append(numberingFormats1);
			stylesheet1.Append(fonts1);
			stylesheet1.Append(fills1);
			stylesheet1.Append(borders1);
			stylesheet1.Append(cellStyleFormats1);
			stylesheet1.Append(cellFormats1);
			stylesheet1.Append(cellStyles1);
			stylesheet1.Append(differentialFormats1);
			stylesheet1.Append(tableStyles1);
			stylesheet1.Append(stylesheetExtensionList1);

			return stylesheet1;
		}
		public ExcelExporter(System.IO.Stream Stream,string Title, Column[] Columns):base(Columns)
        {
            this.Title = string.IsNullOrWhiteSpace(Title) ? "导出数据" : Title;

            Document = SpreadsheetDocument.Create(Stream, SpreadsheetDocumentType.Workbook);

            WorkbookPart = Document.AddWorkbookPart();
            WorkbookPart.Workbook = new Workbook();
            var worksheetPart = WorkbookPart.AddNewPart<WorksheetPart>();

			// Style Part
			WorkbookStylesPart wbsp = WorkbookPart.AddNewPart<WorkbookStylesPart>();
			wbsp.Stylesheet = CreateStylesheet();
			wbsp.Stylesheet.Save();

			this.SheetData = new SheetData();

            worksheetPart.Worksheet = new Worksheet(this.SheetData);

			var sheets = Document.WorkbookPart.Workbook.AppendChild(new Sheets());

			var sheet = new Sheet()
			{
				Id = Document.WorkbookPart.GetIdOfPart(worksheetPart),
				SheetId = 1,
				Name = this.Title
			};
			sheets.AppendChild(sheet);

			var row = new Row { RowIndex = ++_rowIndex };
			SheetData.AppendChild(row);
			var cellIdex = 0;
			
			foreach (var header in Columns)
			{
				row.AppendChild(
					CreateCell(
						ColumnLetter(cellIdex++),
						(int)_rowIndex.Value,
						header.Name ?? string.Empty
					)
				 );
			}
			//if (data.Headers.Count > 0)
			//{
			//    // Add the column configuration if available
			//    if (data.ColumnConfigurations != null)
			//    {
			//        var columns = (Columns)data.ColumnConfigurations.Clone();
			//        worksheetPart.Worksheet
			//            .InsertAfter(columns, worksheetPart
			//            .Worksheet.SheetFormatProperties);
			//    }
			//}
		}
		Cell GetFirstCell(Row row)
		{
			return null;
			//foreach (Cell cell in row.Elements<Cell>())
			//	if (string.Compare(cell.CellReference.Value, "A1", true) > 0)
			//		return cell;
			//throw new NotSupportedException("找不到A1单元格");
		}
		public void AddRow(object[] Cells)
        {
			var cellIdex = 0;
			var row = new Row { RowIndex = ++_rowIndex };
			SheetData.AppendChild(row);
			
			foreach (var cell in Cells)
			{
				var c = CreateCell(
					ColumnLetter(cellIdex),
					(int)_rowIndex.Value,
					Format(cellIdex, cell)
					);
				row.AppendChild(c);
				cellIdex++;
			}
		}


        private string ColumnLetter(int intCol)
        {
            var intFirstLetter = ((intCol) / 676) + 64;
            var intSecondLetter = ((intCol % 676) / 26) + 64;
            var intThirdLetter = (intCol % 26) + 65;

            var firstLetter = (intFirstLetter > 64)
                ? (char)intFirstLetter : ' ';
            var secondLetter = (intSecondLetter > 64)
                ? (char)intSecondLetter : ' ';
            var thirdLetter = (char)intThirdLetter;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }

		void SetText(Cell cell,object str)
		{
			cell.DataType = new EnumValue<CellValues>(CellValues.String);
			cell.CellValue = new CellValue(str == null ? "" : Convert.ToString(str));
			//var istring = new InlineString();
			//var t = new Text { Text = str==null?"":Convert.ToString(str) };
			//istring.AppendChild(t);
			//cell.AppendChild(istring);
		}
        private Cell CreateCell(string header, int index,
            object text)
        {
            var cell = new Cell
            {
                CellReference = header + index
            };
            if (text == null)
            {
				SetText(cell, text);
				return cell;
            }
            switch (Type.GetTypeCode(text.GetType()))
            {
                case TypeCode.Boolean:
                    cell.DataType = CellValues.Boolean;
                    cell.Append(new CellValue(Convert.ToString(text)));
                    break;
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Object:
                    {
						SetText(cell, text);
						break;
                    }
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    cell.DataType = CellValues.Number;
                    cell.Append(new CellValue(Convert.ToString(text)));
                    break;
                case TypeCode.DateTime:
					{
						//cell.DataType = CellValues.Date;
						cell.Append(new CellValue(Convert.ToDateTime(text).ToOADate().ToString()));
						cell.StyleIndex = 1u;

						//SetText(cell, text);
                    }
                    break;
                case TypeCode.DBNull:
                case TypeCode.Empty:
					SetText(cell, null);
					break;

            }
            return cell;
        }

        public void Dispose()
        {
            //WorkbookPart.Workbook.Save();
            Document.Close();
			Document.Dispose();
        }
    }
}