using Moq;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests;

public class ReportServiceTests
{
    private static readonly DateTime November = new(2026, 11, 13);

    private static Task<TransportReport> Build(params TripSummary[] trips)
    {
        var source = new Mock<IReportSource>();
        source.Setup(s => s.ReadAsync()).ReturnsAsync(trips.ToList());

        var service = new ReportService(source.Object);
        return service.BuildAsync();
    }

    [Fact]
    public async Task EmptyScheduleHasEmptyReport()
    {
        var report = await Build();

        Assert.Equal(0, report.Trips);
        Assert.Equal(0, report.Requests);
        Assert.Empty(report.Months);
        Assert.Empty(report.Lines);
    }

    [Fact]
    public async Task EachDepartureCountsAsOneTrip()
    {
        var report = await Build(
            new TripSummary("Б1", November, 5),
            new TripSummary("Б1", November, 0));

        Assert.Equal(2, report.Trips);
        Assert.Equal(5, report.Requests);
    }

    [Fact]
    public async Task DeparturesInSameMonthAreCombined()
    {
        var report = await Build(
            new TripSummary("Б1", November, 2),
            new TripSummary("Б2", November.AddDays(1), 1));

        Assert.Equal(
            new ReportRow("2026-11", 2, 3),
            Assert.Single(report.Months));
    }

    [Fact]
    public async Task SameMonthInDifferentYearsStaysSeparate()
    {
        var report = await Build(
            new TripSummary("Б1", November, 1),
            new TripSummary("Б1", November.AddYears(1), 3));

        Assert.Equal(2, report.Months.Count);
        Assert.Equal(3, report.Months[1].Requests);
    }

    [Fact]
    public async Task MonthsAreSortedChronologically()
    {
        var report = await Build(
            new TripSummary("Б1", November, 0),
            new TripSummary("Б1", November.AddMonths(-1), 0));

        Assert.Equal(
            new[] { "2026-10", "2026-11" },
            report.Months.Select(row => row.Name));
    }

    [Fact]
    public async Task BusiestLineRanksFirstAcrossMultipleTrips()
    {
        var report = await Build(
            new TripSummary("Б1", November, 3),
            new TripSummary("Б2", November, 2),
            new TripSummary("Б2", November.AddDays(1), 2));

        Assert.Equal(new ReportRow("Б2", 2, 4), report.Lines[0]);
    }

    [Fact]
    public async Task EqualDemandIsSortedByLineName()
    {
        var report = await Build(
            new TripSummary("Б2", November, 1),
            new TripSummary("Б1", November, 1));

        Assert.Equal("Б1", report.Lines[0].Name);
    }

    [Fact]
    public async Task LineWithNoRequestsStillAppears()
    {
        var report = await Build(new TripSummary("Б1", November, 0));

        Assert.Equal(new ReportRow("Б1", 1, 0), Assert.Single(report.Lines));
    }
}