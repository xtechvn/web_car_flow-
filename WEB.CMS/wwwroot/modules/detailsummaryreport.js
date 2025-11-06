$(document).ready(function () {

    _detail_summary_report.init();
});
let revenuChartInstance = null;
var _detail_summary_report = {
    init: function () {
        var datetime = null;
        _detail_summary_report.GetDailyStatistics(datetime)

    },
    Seach: function () {
        var text = $('#date_time_Car').val();
        parse_value = text.split(' ')[0].split('-')
        var datetime = parse_value[2] + '/' + parse_value[1] + '/' + parse_value[0];
        _summary_report.GetDailyStatistics(datetime)
        _summary_report.GetTotalWeightByHour(datetime);
        _summary_report.GetProductivityStatistics(datetime);
    },

    GetDailyStatistics: function (datetime) {
        $.ajax({
            url: "/SummaryReport/DailyStatistics",
            type: "post",
            data: { date: datetime },
            success: function (result) {
                $('#Grid-DailyStatistics').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },

}