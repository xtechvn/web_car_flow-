$(document).ready(function () {
    _summary_report.init();
});
var _summary_report = {
    init: function () {
        var datetime = null;
        _summary_report.GetDailyStatistics(datetime)
    },
    GetDailyStatistics: function (datetime) {
        $.ajax({
            url: "/SummaryReport/DailyStatistics",
            type: "post",
            data: { datetime: datetime },
            success: function (result) {
                $('#Grid-DailyStatistics').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
}