using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Services;

// Одна строка исходных данных — один курс.
public record TripSummary(string Line, DateTime Departure, int Requests);

// Одна строка готовой справки — месяц или линия.
public record ReportRow(string Name, int Trips, int Requests);

public record TransportReport(
    int Trips,
    int Requests,
    List<ReportRow> Months,
    List<ReportRow> Lines);

public interface IReportSource
{
    Task<List<TripSummary>> ReadAsync();
}

public class DatabaseReportSource(TransportContext db) : IReportSource
{
    public Task<List<TripSummary>> ReadAsync()
    {
        return db.Schedules
            .Select(s => new TripSummary(
                s.Route!.RouteName,
                s.DepartureTime,
                db.TravelRequests.Count(r => r.ScheduleID == s.ScheduleID)))
            .ToListAsync();
    }
}

public class ReportService(IReportSource source)
{
    public async Task<TransportReport> BuildAsync()
    {
        var trips = await source.ReadAsync();

        var months = trips
            .GroupBy(t => t.Departure.ToString(
                "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture))
            .OrderBy(group => group.Key)
            .Select(group => new ReportRow(
                group.Key,
                group.Count(),
                group.Sum(t => t.Requests)))
            .ToList();

        var lines = trips
            .GroupBy(t => t.Line)
            .Select(group => new ReportRow(
                group.Key,
                group.Count(),
                group.Sum(t => t.Requests)))
            .OrderByDescending(row => row.Requests)
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();

        return new TransportReport(
            trips.Count,
            trips.Sum(t => t.Requests),
            months,
            lines);
    }
}