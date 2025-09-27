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
    const container = $('<div id="dropdown-container"></div>').appendTo('body');
    let $menu = null;
    let $currentBtn = null;

    $(document).on('click', '.status-dropdown .dropdown-toggle', function (e) {
        e.stopPropagation();
        const $btn = $(this);
        const optsData = $btn.data('options'); // mảng [{text, class}]
        const options = Array.isArray(optsData) ? optsData : JSON.parse(optsData);
        const currentText = $.trim($btn.text());

        // Đóng menu cũ (nếu có)
        if ($menu) {
            $menu.remove();
            $menu = null;
        }

        $currentBtn = $btn;

        // Tạo menu + danh sách li
        $menu = $('<div class="dropdown-menu"><ul></ul></div>');
        const $ul = $menu.find('ul');

        options.forEach(opt => {
            $('<li>')
                .text(opt.text)
                .addClass('status-option')
                .attr('data-value', opt.value) // Corrected from opt.valuse
                .toggleClass('active', opt.text === currentText)
                .appendTo($ul);
        });

        const $actions = $('<div class="actions"></div>');
        $('<button class="cancel">Bỏ qua</button>').appendTo($actions);
        $('<button class="confirm">Xác nhận</button>').appendTo($actions);
        $menu.append($actions);
        container.append($menu);

        // Tính toán vị trí
        const btnOffset = $btn.offset();
        const btnHeight = $btn.outerHeight();
        const menuHeight = $menu.outerHeight();
        const winHeight = $(window).height();
        let top = btnOffset.top + btnHeight;
        let dropUp = false;

        if (top + menuHeight > winHeight) {
            top = btnOffset.top - menuHeight;
            dropUp = true;
            $menu.addClass('drop-up');
        } else {
            $menu.removeClass('drop-up');
        }

        $menu.css({
            left: btnOffset.left,
            top: top,
            display: 'block'
        });
    });

    // Click chọn item
    $(document).on('click', '#dropdown-container .dropdown-menu li', function (e) {
        e.stopPropagation();
        $('#dropdown-container .dropdown-menu li').removeClass('active');
        $(this).addClass('active');
    });

    // Bỏ qua
    $(document).on('click', '#dropdown-container .actions .cancel', function (e) {
        e.stopPropagation();
        closeMenu();
    });

    // ✅ Xác nhận – đổi text + class cho button
    $(document).on('click', '#dropdown-container .actions .confirm', function (e) {
        e.stopPropagation();
        if ($menu && $currentBtn) {
            const $active = $menu.find('li.active');
            if ($active.length) {
                const text = $active.text();
                const val_TT = $active.attr('data-value');
                const $row = $currentBtn.closest('tr');
                let id_row = 0;
                if ($row.length) {
                    const classAttr = $row.attr('class');
                    const match = classAttr.match(/CartoFactory_(\d+)/);
                    if (match && match[1]) {
                        id_row = match[1];
                    }
                }

                const cls = $active.attr('class').split(/\s+/)
                    .filter(c => c !== 'active')[0] || '';

                var Status_type = _Call_The_Scale.UpdateStatus(id_row, val_TT, 3);
                if (Status_type == 0) {
                    $currentBtn
                        .text(text)
                        .removeClass(function (_, old) {
                            return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                        }) // xoá các class status- cũ
                        .addClass(cls); // gắn class mới (status-arrived, status-blank…)


                    if (val_TT == 1) {
                        $('#dataBody-1').find('.CartoFactory_' + id_row).remove();

                    } else {
                        $('#dataBody-0-0').find('.CartoFactory_' + id_row).remove();
                        $('#dataBody-0-1').find('.CartoFactory_' + id_row).remove();

                    }
                }
                
            }
        }
        closeMenu();
    });

    // Đóng menu khi click ra ngoài
    $(document).on('click', function () {
        closeMenu();
    });

    function closeMenu() {
        if ($menu) {
            $menu.remove();
            $menu = null;
            $currentBtn = null;
        }
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/CarHub")
        .withAutomaticReconnect([0, 2000, 10000, 30000]) // retry sau 0s, 2s, 10s, 30s
        .build();
    const AllCode = [
        { Description: "Blank", CodeValue: "1" },
        { Description: "Đã vào cân", CodeValue: "0" },
        // Add more objects as needed
    ];
    // Create a new array of objects in the desired format
    const options = AllCode.map(allcode => ({
        text: allcode.Description,
        value: allcode.CodeValue
    }));
    const jsonString = JSON.stringify(options);
    // Hàm render row
    function renderRow_Da_SL(item) {

        return `
        <tr class="CartoFactory_${item.id}" data-queue="${item.recordNumber}" >
            <td>${item.recordNumber}</td>
            <td>${item.registerDateOnline}</td>
            <td>${item.customerName}</td>
            <td>${item.driverName}</td>
            <td>${item.phoneNumber}</td>
            <td>${item.vehicleNumber}</td>
            <td>${item.loadTypeName}</td>
            <td>
                <div class="status-dropdown">
                    <button class="dropdown-toggle status-perfect" data-options='${jsonString}'>
                        ${item.vehicleWeighingTypeName}
                    </button>
                </div>

            </td>
            <td><span class="icon"><img src="/images/graphics/SpeakerHigh.png" height="25" alt=""></span></td>
        </tr>`;
    }
    function renderRow(item) {

        return `
        <tr class="CartoFactory_${item.id}" data-queue="${item.recordNumber}" >
            <td>${item.recordNumber}</td>
            <td>${item.phoneNumber}</td>
            <td>${item.vehicleNumber}</td>
            <td>${item.loadTypeName}</td>
            <td>
                <div class="status-dropdown">
                    <button class="dropdown-toggle status-perfect" data-options='${jsonString}'>
                        ${item.vehicleWeighingTypeName}
                    </button>
                </div>

            </td>
            <td><span class="icon"><img src="/images/graphics/SpeakerHigh.png" height="25" alt=""></span></td>
        </tr>`;
    }

    // Hàm sắp xếp lại tbody theo QueueNumber tăng dần
    function sortTable_Da_SL() {
        const tbody = document.getElementById("dataBody-1");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const qa = parseInt(a.getAttribute("data-queue") || 0);
            const qb = parseInt(b.getAttribute("data-queue") || 0);
            return qa - qb;
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
    function sortTable() {
        const tbody = document.getElementById("dataBody-0");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const qa = parseInt(a.getAttribute("data-queue") || 0);
            const qb = parseInt(b.getAttribute("data-queue") || 0);
            return qa - qb;
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
    connection.start()
        .then(() => console.log("✅ Kết nối SignalR thành công"))
        .catch(err => console.error("❌ Lỗi kết nối:", err));
    // Nhận data mới từ server
    connection.on("ListCallTheScale_Da_SL", function (item) {
        const tbody = document.getElementById("dataBody-1");
        tbody.insertAdjacentHTML("beforeend", renderRow_Da_SL(item));
        sortTable_Da_SL(); // sắp xếp lại ngay khi thêm
    });

    connection.on("ListCallTheScale_0", function (item) {
        const tbody = document.getElementById("dataBody-0-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable(); // sắp xếp lại ngay khi thêm
    });
    connection.on("ListCallTheScale_1", function (item) {
        const tbody = document.getElementById("dataBody-0-1");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable(); // sắp xếp lại ngay khi thêm
    });
    //sử lý đăng tải
    connection.on("ListProcessingIsLoading_Da_SL", function (item) {
        if (item.loadType == 0) {
            const tbody = document.getElementById("dataBody-0-0");
            tbody.insertAdjacentHTML("beforeend", renderRow(item));
            sortTable(); 
        } else {
            const tbody = document.getElementById("dataBody-0-1");
            tbody.insertAdjacentHTML("beforeend", renderRow(item));
            sortTable(); 
        }     
    });
    connection.on("ListProcessingIsLoading", function (item) {
        $('#dataBody-0-0').find('.CartoFactory_' + item.id).remove();
        $('#dataBody-0-1').find('.CartoFactory_' + item.id).remove();
    });
    connection.onreconnecting(error => {
        console.warn("🔄 Đang reconnect...", error);
    });

    connection.onreconnected(connectionId => {
        console.log("✅ Đã reconnect. Connection ID:", connectionId);
    });

    connection.onclose(error => {
        console.error("❌ Kết nối bị đóng.", error);
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
            LoadType: 0,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
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
            LoadType: 1,
            VehicleWeighingType: null,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
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
    },
    UpdateStatus: function (id, status, type) {
        var status_type = 0
        $.ajax({
            url: "/Car/UpdateStatus",
            type: "post",
            data: { id: id, status: status, type: type },
            success: function (result) {
                status_type = result.status;
                if (result.status == 0) {
                    _msgalert.success(result.msg)
                    $.magnificPopup.close();
                } else {
                    _msgalert.error(result.msg)
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
        return status_type;
    },
}