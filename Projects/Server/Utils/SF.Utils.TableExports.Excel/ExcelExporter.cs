using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;

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
		private static Stylesheet CreateStylesheet()
		{
			Stylesheet ss = new Stylesheet();

			//Fonts fts = new Fonts();
			//DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
			//FontName ftn = new FontName();
			//ftn.Val = "Calibri";
			//FontSize ftsz = new FontSize();
			//ftsz.Val = 11;
			//ft.FontName = ftn;
			//ft.FontSize = ftsz;
			//fts.Append(ft);
			//fts.Count = (uint)fts.ChildElements.Count;

			//Fills fills = new Fills();
			//Fill fill;
			//PatternFill patternFill;
			//fill = new Fill();
			//patternFill = new PatternFill();
			//patternFill.PatternType = PatternValues.None;
			//fill.PatternFill = patternFill;
			//fills.Append(fill);
			//fill = new Fill();
			//patternFill = new PatternFill();
			//patternFill.PatternType = PatternValues.Gray125;
			//fill.PatternFill = patternFill;
			//fills.Append(fill);
			//fills.Count = (uint)fills.ChildElements.Count;

			//Borders borders = new Borders();
			//Border border = new Border();
			//border.LeftBorder = new LeftBorder();
			//border.RightBorder = new RightBorder();
			//border.TopBorder = new TopBorder();
			//border.BottomBorder = new BottomBorder();
			//border.DiagonalBorder = new DiagonalBorder();
			//borders.Append(border);
			//borders.Count = (uint)borders.ChildElements.Count;

			//CellStyleFormats csfs = new CellStyleFormats();
			//CellFormat cf = new CellFormat();
			//cf.NumberFormatId = 0;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//csfs.Append(cf);
			//csfs.Count = (uint)csfs.ChildElements.Count;

			//uint iExcelIndex = 164;
			//NumberingFormats nfs = new NumberingFormats();
			//CellFormats cfs = new CellFormats();

			//cf = new CellFormat();
			//cf.NumberFormatId = 0;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//cf.FormatId = 0;
			//cfs.Append(cf);

			//NumberingFormat nf;
			//nf = new NumberingFormat();
			//nf.NumberFormatId = iExcelIndex++;
			//nf.FormatCode = "dd/mm/yyyy hh:mm:ss";
			//nfs.Append(nf);
			//cf = new CellFormat();
			//cf.NumberFormatId = nf.NumberFormatId;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//cf.FormatId = 0;
			//cf.ApplyNumberFormat = true;
			//cfs.Append(cf);

			//nf = new NumberingFormat();
			//nf.NumberFormatId = iExcelIndex++;
			//nf.FormatCode = "#,##0.0000";
			//nfs.Append(nf);
			//cf = new CellFormat();
			//cf.NumberFormatId = nf.NumberFormatId;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//cf.FormatId = 0;
			//cf.ApplyNumberFormat = true;
			//cfs.Append(cf);

			//// #,##0.00 is also Excel style index 4
			//nf = new NumberingFormat();
			//nf.NumberFormatId = iExcelIndex++;
			//nf.FormatCode = "#,##0.00";
			//nfs.Append(nf);
			//cf = new CellFormat();
			//cf.NumberFormatId = nf.NumberFormatId;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//cf.FormatId = 0;
			//cf.ApplyNumberFormat = true;
			//cfs.Append(cf);

			//// @ is also Excel style index 49
			//nf = new NumberingFormat();
			//nf.NumberFormatId = iExcelIndex++;
			//nf.FormatCode = "@";
			//nfs.Append(nf);
			//cf = new CellFormat();
			//cf.NumberFormatId = nf.NumberFormatId;
			//cf.FontId = 0;
			//cf.FillId = 0;
			//cf.BorderId = 0;
			//cf.FormatId = 0;
			//cf.ApplyNumberFormat = true;
			//cfs.Append(cf);

			//nfs.Count = (uint)nfs.ChildElements.Count;
			//cfs.Count = (uint)cfs.ChildElements.Count;

			//ss.Append(nfs);
			//ss.Append(fts);
			//ss.Append(fills);
			//ss.Append(borders);
			//ss.Append(csfs);
			//ss.Append(cfs);

			//CellStyles css = new CellStyles();
			//CellStyle cs = new CellStyle();
			//cs.Name = "Normal";
			//cs.FormatId = 0;
			//cs.BuiltinId = 0;
			//css.Append(cs);
			//css.Count = (uint)css.ChildElements.Count;
			//ss.Append(css);

			//DifferentialFormats dfs = new DifferentialFormats();
			//dfs.Count = 0;
			//ss.Append(dfs);

			//TableStyles tss = new TableStyles();
			//tss.Count = 0;
			//tss.DefaultTableStyle = "TableStyleMedium9";
			//tss.DefaultPivotStyle = "PivotStyleLight16";
			//ss.Append(tss);

			return ss;
		}
        public ExcelExporter(System.IO.Stream Stream,string Title, Column[] Columns):base(Columns)
        {
            this.Title = string.IsNullOrWhiteSpace(Title) ? "导出数据" : Title;

            Document = SpreadsheetDocument.Create(Stream, SpreadsheetDocumentType.Workbook);

            WorkbookPart = Document.AddWorkbookPart();
            WorkbookPart.Workbook = new Workbook();
            var worksheetPart = WorkbookPart.AddNewPart<WorksheetPart>();

			// Style Part
			var wbsp = WorkbookPart.AddNewPart<WorkbookStylesPart>();
			wbsp.Stylesheet = CreateStylesheet();
			//wbsp.Stylesheet.Save();
			
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
						//cell.Append(new CellValue(Convert.ToDateTime(text).ToOADate().ToString()));
						//cell.StyleIndex = 164u;

						SetText(cell, text);
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