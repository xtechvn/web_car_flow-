$(document).ready(function () {
    _Call_The_Scale.init();
    var input_Call_The_Scale_Chua_SL = document.getElementById("input_Call_The_Scale_Chua_SL");
    input_Call_The_Scale_Chua_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Call_The_Scale.ListCallTheScale();
            _Call_The_Scale.ListCallTheScale_2();
        }
    });
    var input_Call_The_Scale_Da_SL = document.getElementById("input_Call_The_Scale_Da_SL");
    input_Call_The_Scale_Da_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Call_The_Scale.ListCallTheScale_Da_SL();
        }
    });
});
var _Call_The_Scale = {
    init: function () {
        _Call_The_Scale.ListCallTheScale();
        _Call_The_Scale.ListCallTheScale_2();
        _Call_The_Scale.ListCallTheScale_Da_SL();
    },
    ListCallTheScale: function () {
        var model = {
            VehicleNumber: $('#input_Call_The_Scale_Chua_SL').val(),
            PhoneNumber: $('#input_Call_The_Scale_Chua_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: null,
            type: 0,
        }
        $.ajax({
            url: "/Car/ListCallTheScale",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Call_The_Scale_Chua_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListCallTheScale_2: function () {
        var model = {
            VehicleNumber: $('#input_Call_The_Scale_Chua_SL').val(),
            PhoneNumber: $('#input_Call_The_Scale_Chua_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: null,
            type: 0,
        }
        $.ajax({
            url: "/Car/ListCallTheScale",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Call_The_Scale_Chua_SL_2').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListCallTheScale_Da_SL: function () {
        var model = {
            VehicleNumber: $('#input_Call_The_Scale_Da_SL').val(),
            PhoneNumber: $('#input_Call_The_Scale_Da_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            type:1,
        }
        $.ajax({
            url: "/Car/ListCallTheScale",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Call_The_Scale_Da_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    }
}