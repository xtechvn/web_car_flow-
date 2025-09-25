$(document).ready(function () {
    _processing_is_loading.init();
    var input_Processing_Is_Loading_Chua_SL = document.getElementById("input_Processing_Is_Loading_Chua_SL");
    input_Processing_Is_Loading_Chua_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _processing_is_loading.ListProcessingIsLoading();
        }
    });
    var input_Processing_Is_Loading_Da_SL = document.getElementById("input_Processing_Is_Loading_Da_SL");
    input_Processing_Is_Loading_Da_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _processing_is_loading.ListProcessingIsLoading_Da_SL();
        }
    });
});
var _processing_is_loading = {
    init: function () {
        _processing_is_loading.ListProcessingIsLoading();
        _processing_is_loading.ListProcessingIsLoading_Da_SL();
    },
    ListProcessingIsLoading: function () {
        var model = {
            VehicleNumber: $('#input_Processing_Is_Loading_Chua_SL').val(),
            PhoneNumber: $('#input_Processing_Is_Loading_Chua_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: null,
        }
        $.ajax({
            url: "/Car/ListProcessingIsLoading",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Processing_Is_Loading_Chua_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListProcessingIsLoading_Da_SL: function () {
        var model = {
            VehicleNumber: $('#input_Processing_Is_Loading_Da_SL').val(),
            PhoneNumber: $('#input_Processing_Is_Loading_Da_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
        }
        $.ajax({
            url: "/Car/ListProcessingIsLoading",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Processing_Is_Loading_Da_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    }
}