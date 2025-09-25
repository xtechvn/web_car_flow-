$(document).ready(function () {
    _Weighed_Input.init();
    var input_Weighed_Input_Chua_SL = document.getElementById("input_Weighed_Input_Chua_SL");
    input_Weighed_Input_Chua_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Weighed_Input.ListWeighedInput();
        }
    });
    var input_Weighed_Input_Da_SL = document.getElementById("input_Weighed_Input_Da_SL");
    input_Weighed_Input_Da_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Weighed_Input.ListWeighedInput_Da_SL();
        }
    });
});
var _Weighed_Input = {
    init: function () {
        _Weighed_Input.ListWeighedInput();
        _Weighed_Input.ListWeighedInput_Da_SL();
    },
    ListWeighedInput: function () {
        var model = {
            VehicleNumber: $('#input_Weighed_Input_Chua_SL').val(),
            PhoneNumber: $('#input_Weighed_Input_Chua_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: null,
        }
        $.ajax({
            url: "/Car/ListWeighedInput",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Weighed_Input_Chua_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListWeighedInput_Da_SL: function () {
        var model = {
            VehicleNumber: $('#input_Weighed_Input_Da_SL').val(),
            PhoneNumber: $('#input_Weighed_Input_Da_SL').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 1,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
        }
        $.ajax({
            url: "/Car/ListWeighedInput",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Weighed_Input_Da_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    }
}