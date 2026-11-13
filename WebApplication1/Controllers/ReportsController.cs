using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController(ReportService reports) : Controller
{
    public async Task<IActionResult> Index()
    {
        var report = await reports.BuildAsync();
        return View(report);
    }

    public async Task<IActionResult> Excel()
    {
        var report = await reports.BuildAsync();

        using var book = new XLWorkbook();
        WriteSheet(book, "По месеци", "Месец на заминаване", report.Months, report);
        WriteSheet(book, "По линии", "Линия", report.Lines, report);

        using var stream = new MemoryStream();
        book.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "REPPO-spravka.xlsx");
    }

    private static void WriteSheet(
        XLWorkbook book,
        string sheetName,
        string firstColumn,
        List<ReportRow> rows,
        TransportReport report)
    {
        var sheet = book.AddWorksheet(sheetName);

        sheet.Cell(1, 1).Value = firstColumn;
        sheet.Cell(1, 2).Value = "Курсове";
        sheet.Cell(1, 3).Value = "Заявки";

        int rowNumber = 2;

        foreach (var row in rows)
        {
            sheet.Cell(rowNumber, 1).Value = row.Name;
            sheet.Cell(rowNumber, 2).Value = row.Trips;
            sheet.Cell(rowNumber, 3).Value = row.Requests;
            rowNumber++;
        }

        sheet.Cell(rowNumber, 1).Value = "Общо";
        sheet.Cell(rowNumber, 2).Value = report.Trips;
        sheet.Cell(rowNumber, 3).Value = report.Requests;

        sheet.Range("A1:C1").Style.Font.Bold = true;
        sheet.Range(rowNumber, 1, rowNumber, 3).Style.Font.Bold = true;
        sheet.Column(1).Width = 35;
        sheet.Columns(2, 3).Width = 15;
        sheet.SheetView.FreezeRows(1);
    }
}