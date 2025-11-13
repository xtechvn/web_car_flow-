$(document).ready(function () {

    _detail_summary_report.init();
});
let revenuChartInstance = null;
var _detail_summary_report = {
    init: function () {
        var model = {
            RegistrationTime: null,
            LoadType: $('#loadType').val(),
        }
        _detail_summary_report.GetDailyStatistics(model)

    },
    Seach: function () {
      
        var text = $('#date_time_Car').val();
        parse_value = text.split(' ')[0].split('-')
        var datetime = parse_value[2] + '/' + parse_value[1] + '/' + parse_value[0];
        var model = {
            RegistrationTime: datetime,
            LoadType: $('#loadType').val(),
        }
        _detail_summary_report.GetDailyStatistics(model)

    },

    GetDailyStatistics: function (model) {
        $.ajax({
            url: "/SummaryReport/DailyStatistics",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#Grid-DailyStatistics').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },

}