$(document).ready(function () {
    
    _summary_report.init();
});
let revenuChartInstance = null;
var _summary_report = {
    init: function () {
        var datetime = null;
        _summary_report.GetDailyStatistics(datetime)
        _summary_report.GetTotalWeightByHour(datetime);
        _summary_report.GetProductivityStatistics(datetime);
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
    GetProductivityStatistics: function (datetime) {
        $.ajax({
            url: "/SummaryReport/GetProductivityStatistics",
            type: "post",
            data: { date: datetime },
            success: function (result) {
                $('#Grid-ProductivityStatistics').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    GetTotalWeightByHour: function (datetime) {
        $.ajax({
            url: "/SummaryReport/GetTotalWeightByHour",
            type: "post",
            data: { date: datetime },
            success: function (result) {
                if (result.isSuccess != false) {
                    
                    result.data.totalWeightInHour
                    _summary_report.Bieudo(result.data.completionHour, result.data.totalWeightInHour, result.data.totalWeightInHour);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    Bieudo: function (listLabel, volXuatData, nangSuatLoadingData) {
        //const listLabel = ['1', '2', '3', '4', '5', '7', '8', '9', '14', '15', '16', '17', '18', '19', '21', '22', '23', '24'];
        //const volXuatData = [159, 71, 74, 146, 151, 185, 110, 163, 159, 179, 90, 141, 230, 132, 175, 50, 137, 50];
        //const nangSuatLoadingData = [159, 71, 74, 146, 151, 185, 110, 163, 159, 179, 90, 141, 230, 132, 175, 50, 137, 50];

        if (window.revenuChartInstance) {
            window.revenuChartInstance.destroy();
        }

        var canvas = document.getElementById("revenuChart");
        var ctx = canvas.getContext('2d');

        var barChartData = {
            labels: listLabel,
            datasets: [
                {
                    label: 'Vol. xuất (tấn)',
                    data: volXuatData,
                    backgroundColor: 'rgba(10, 140, 50, 0.8)',
                    borderColor: 'rgba(10, 140, 50, 1)',
              
                    yAxisID: 'y-axis-0',
                   
                },
                {
                    label: 'Năng suất loading (tấn/h)',
                    data: nangSuatLoadingData,
                    type: 'line',
                    borderColor: '#2f3db5',
                    backgroundColor: 'rgba(54, 162, 235, 0.1)',
                    yAxisID: 'y-axis-1',
                 
                  
                }
            ]
        };

        var ObjectChart = {
            type: 'bar',
            data: barChartData,
            options: {
                responsive: true,
                maintainAspectRatio: false,

                // ---- CHỐT: TẮT HOÀN TOÀN TOOLTIP & HOVER ----
                // 1) tắt tooltip built-in
                tooltips: { enabled: false },
                // 2) tắt hover mode
                hover: { mode: null, onHover: null },
                // 3) KHÓA hoàn toàn event chuột (tắt mọi event mouse)
                events: [],

                // nếu bạn dùng legend click để show/hide, có thể disable onClick:
                legend: {
                    onClick: function () { /* no-op */ }
                },

                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Mốc thời gian'
                        },
                        gridLines: { display: false }
                    }],
                    yAxes: [{
                        id: 'y-axis-0',
                        type: 'linear',
                        position: 'left',
                        stacked: true,
                        ticks: {
                            beginAtZero: true,
                            stepSize: 20,
                            callback: function (value) { return value + ' Tấn'; }
                        },
                        gridLines: { color: '#f1f1f1' }
                    }, {
                        id: 'y-axis-1',
                        type: 'linear',
                        position: 'right',
                        gridLines: { drawOnChartArea: false },
                        ticks: { display: false, beginAtZero: true, stepSize: 20, },
                      
                    }]
                },

                animation: {
                    duration: 0,
                    onComplete: function () {
                        var chartInstance = this.chart;
                        var ctx = chartInstance.ctx;
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.font = 'bold 11px Arial';

                        this.data.datasets.forEach(function (dataset, i) {
                            var meta = chartInstance.controller.getDatasetMeta(i);
                            meta.data.forEach(function (bar, index) {
                                var value = dataset.data[index];

                                if (dataset.type === 'line') {
                                    ctx.fillStyle = 'rgba(54, 162, 235, 1)';
                                    ctx.fillText(value, bar._model.x, bar._model.y - 10);
                                } else {
                                    const barHeight = Math.abs(bar._model.y - bar._yScale.bottom);
                                    const textY = bar._model.y + barHeight / 2;
                                    ctx.fillStyle = (barHeight > 40) ? '#ffffff' : '#000000';
                                    ctx.fillText(value, bar._model.x, textY);
                                }
                            });
                        });
                    }
                },

                title: {
                    display: true,
                    text: 'Xu hướng khối lượng xuất và năng suất loading'
                }
            }
        };

        // create chart
        window.revenuChartInstance = new Chart(ctx, ObjectChart);

        // đảm bảo cập nhật
        if (window.revenuChartInstance) window.revenuChartInstance.update();
    }

}