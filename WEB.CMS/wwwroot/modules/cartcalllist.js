$(document).ready(function () {
    _cartcalllist.init();
    var input_chua_xu_ly = document.getElementById("input_chua_xu_ly");
    input_chua_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _cartcalllist.ListCartoFactory();
        }
    });
    var input_da_xu_ly = document.getElementById("input_da_xu_ly");
    input_da_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _cartcalllist.ListCartoFactory_Da_SL();
        }
    });
    const container = $('<div id="dropdown-container"></div>').appendTo('body');
    let $menu = null;
    let $currentBtn = null;
    $(document).on('click', '.status-dropdown .dropdown-toggle', function (e) {
        e.stopPropagation();
        const $btn = $(this);
        // 🚫 Nếu button đã disabled thì thoát luôn
        if ($btn.hasClass("disabled") || $btn.is(":disabled")) {
            return;
        }
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

        // --- 🔧 Tính toán vị trí dropdown (dùng viewport coords) ---
        const rect = $btn[0].getBoundingClientRect(); // viewport coordinates
        const btnHeight = rect.height;
        const winWidth = $(window).width();
        const winHeight = $(window).height();
        const paddingScreen = 15; // chừa khoảng 15px mỗi bên
        $menu.css({
            position: 'absolute',
            left: 0,
            top: 0,
            display: 'block',
            visibility: 'hidden'
        });

        const menuWidth = $menu.outerWidth();
        const menuHeight = $menu.outerHeight();

        // Vị trí mặc định: bên dưới button (viewport coords)
        let left = rect.left;
        let top = rect.top + btnHeight;

        // Nếu dropdown tràn phải -> dịch sang trái
        if (left + menuWidth + paddingScreen > winWidth) {
            left = winWidth - menuWidth - paddingScreen;
        }

        // Nếu tràn trái -> giữ cách paddingScreen
        if (left < paddingScreen) {
            left = paddingScreen;
        }

        // Nếu tràn dưới -> bật drop-up (hiển thị phía trên button)
        if (top + menuHeight > winHeight) {
            top = rect.top - menuHeight;
            $menu.addClass('drop-up');
        } else {
            $menu.removeClass('drop-up');
        }

        // Áp vị trí cuối cùng và hiển thị menu
        $menu.css({
            left: left,
            top: top,
            visibility: 'visible' // hiện lên
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
    // Khi chọn máng xuất trong dropdown (type=1)
    $(document).on('click', '#dropdown-container .actions .confirm', async function (e) {
        debugger
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

                // cập nhật giao diện dropdown
            

                var type = $currentBtn.attr('data-type');
                if (type == '1') {
                    // update máng xuất
                    var status_type = await _cartcalllist.UpdateStatus(id_row, val_TT, 4);
                    if (status_type == 0) {
                        $currentBtn
                            .text(text)
                            .removeClass(function (_, old) {
                                return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                            })
                            .addClass($active.attr('class').split(/\s+/).filter(c => c !== 'active')[0] || '');
                    }
                    // gọi SignalR thông báo cho tất cả client
                    connection.invoke("BroadcastUpdateMang", val_TT, "Đang xử lý")
                        .catch(err => console.error(err.toString()));
                } else {
                    var weight = $row.find('input.weight').val() || 0;
                    var status_type=await _cartcalllist.UpdateStatus(id_row, val_TT, 6, weight);
                    
                    if (val_TT != 0) {
                        $('#dataBody-0').find('.CartoFactory_' + id_row).remove();
                    }
                    if (status_type == 0) {
                        $currentBtn
                            .text(text)
                            .removeClass(function (_, old) {
                                return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                            })
                            .addClass($active.attr('class').split(/\s+/).filter(c => c !== 'active')[0] || '');
                    }

                    // nếu trạng thái kết thúc → giải phóng máng
                    if (val_TT == 0) {
                        let mangName = $row.find('button[data-type="1"]').text().trim();
                        let match = mangName.match(/\d+/);
                        if (match) {
                            let mangIndex = parseInt(match[0]);

                            // 🔎 Check còn xe nào khác trong cùng máng này không
                            let stillHasCar = $("#dataBody-0 tr, #dataBody-1 tr").toArray().some(tr => {
                                let btnText = $(tr).find("button[data-type='1']").text().trim();
                                let trangThai = $(tr).find("td:last .dropdown-toggle").text().trim();
                                return btnText === mangName && trangThai !== "Hoàn thành";
                            });

                            if (stillHasCar) {
                                // ✅ Nếu còn xe khác chưa hoàn thành → máng vẫn đang xử lý
                                $("#input" + mangIndex).val("Đang xử lý")
                                    .removeClass("empty").addClass("processing");
                            } else {
                                // ✅ Nếu không còn xe nào → máng trống
                                $("#input" + mangIndex).val("Trống")
                                    .removeClass("processing").addClass("empty");
                            }
                        }
                    } else {
                        // ✅ nếu không phải hoàn thành => máng đó đang xử lý
                        let mangName = $row.find('button[data-type="1"]').text().trim();
                        let match = mangName.match(/\d+/);
                        if (match) {
                            let mangIndex = parseInt(match[0]);
                            $("#input" + mangIndex).val("Đang xử lý")
                                .removeClass("empty").addClass("processing");
                        }
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
        { Description: "Máng 1", CodeValue: "0" },
        { Description: "Máng 2", CodeValue: "1" },
        { Description: "Máng 3", CodeValue: "2" },
        { Description: "Máng 4", CodeValue: "3" },
        { Description: "Máng 5", CodeValue: "4" },
        // Add more objects as needed
    ];
    const AllCode2 = [
        { Description: "Blank", CodeValue: "3" },
        { Description: "Đang xếp hàng", CodeValue: "2" },
        { Description: "Đã gọi", CodeValue: "1" },
        { Description: "Hoàn thành", CodeValue: "0" },
        // Add more objects as needed
    ];
    // Create a new array of objects in the desired format
    const options = AllCode.map(allcode => ({
        text: allcode.Description,
        value: allcode.CodeValue
    }));
    const options2 = AllCode2.map(allcode2 => ({
        text: allcode2.Description,
        value: allcode2.CodeValue
    }));
    const jsonString = JSON.stringify(options);
    const jsonString2 = JSON.stringify(options2);
    // Hàm render row
    function renderRow(item, isProcessed) {
        return `
    <tr class="CartoFactory_${item.id}" data-queue="${item.recordNumber}">
        <td>${item.recordNumber}</td>
        <td>${item.customerName}</td>
        <td>${item.driverName}</td>
        <td>${item.vehicleNumber}</td>
        <td>${item.vehicleWeighingTimeComplete || ""}</td>
        <td>
            <div class="status-dropdown">
                <button class="dropdown-toggle ${isProcessed ? "disabled" : ""}"
                        data-type="1"
                        data-options='${jsonString}'
                        ${isProcessed ? "disabled" : ""}>
                    ${item.troughTypeName || ""}
                </button>
            </div>
        </td>
      <td>
        <input type="text"
               class="input-form weight"
               value="${item.vehicleTroughWeight > 0 ? item.vehicleTroughWeight : ""}"
               placeholder="Vui lòng nhập"
               ${isProcessed ? "disabled" : ""} />
    </td>
        <td>
            <div class="status-dropdown">
                <button class="dropdown-toggle"
                        data-options='${jsonString2}'>
                    ${item.vehicleTroughStatusName || ""}
                </button>
            </div>
        </td>
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
    // Hàm cập nhật trạng thái máng (client-side)


    connection.start()
        .then(() => console.log("✅ Kết nối SignalR thành công"))
        .catch(err => console.error("❌ Lỗi kết nối:", err));
    // Nhận data mới từ server
    connection.on("ListCarCall_Da_SL", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-1");
        tbody.insertAdjacentHTML("beforeend", renderRow(item, true));
        sortTable_Da_SL(); // sắp xếp lại ngay khi thêm
    });
    // Nhận data từ server (SignalR)
    connection.on("UpdateMangStatus", function (oldMangId, newMangId, carId) {
        // ✅ Update máng mới thành "Đang xử lý"
        if (newMangId !== null && newMangId !== undefined) {
            $("#input" + (parseInt(newMangId) + 1)).val("Đang xử lý")
                .removeClass("empty").addClass("processing");
        }

        // ✅ Kiểm tra máng cũ: nếu không còn xe nào ở máng đó thì reset về "Trống"
        if (oldMangId !== null && oldMangId !== undefined && oldMangId != newMangId) {
            const hasOtherCars = $("#dataBody-0 tr, #dataBody-1 tr").toArray().some(tr => {
                return $(tr).find("button[data-type='1']").text().trim() === "Máng " + (parseInt(oldMangId) + 1);
            });

            if (!hasOtherCars) {
                $("#input" + (parseInt(oldMangId) + 1)).val("Trống")
                    .removeClass("processing").addClass("empty");
            }
        }

        // ✅ Update luôn dropdown text trong bảng cho xe đó
        const $row = $(".CartoFactory_" + carId);
        if ($row.length) {
            $row.find(".dropdown-toggle[data-type='1']").text("Máng " + (parseInt(newMangId) + 1));
        }
    });




    connection.on("ListCarCall", function (item) {
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item, false));
        sortTable(); // sắp xếp lại ngay khi thêm
    });

    // Nhận data mới từ gọi xe cân đầu vào
    connection.on("ListWeighedInput_Da_SL", function (item) {
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item, false));
        sortTable();
    });
    connection.on("ListWeighedInput", function (item) {
        $('#dataBody-0').find('.CartoFactory_' + item.id).remove();

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
var _cartcalllist = {
    // ✅ Hàm đồng bộ máng khi vừa load trang hoặc reload data
    // ✅ Đồng bộ trạng thái máng khi load trang hoặc reload data
    initMangStatus: function () {
        // Giả sử có 5 máng, bạn thay bằng số máng thực tế
        for (let mangIndex = 1; mangIndex <= 5; mangIndex++) {
            let mangName = "Máng " + mangIndex;

            // 🔎 Kiểm tra xem có xe nào trong máng này chưa hoàn thành không
            let stillHasCar = $("#dataBody-0 tr, #dataBody-1 tr").toArray().some(tr => {
                let btnText = $(tr).find("button[data-type='1']").text().trim();
                let trangThai = $(tr).find("td:last .dropdown-toggle").text().trim();
                return btnText === mangName && trangThai !== "Hoàn thành";
            });

            if (stillHasCar) {
                _cartcalllist.updateMangStatus(mangIndex, "Đang xử lý");
            } else {
                _cartcalllist.updateMangStatus(mangIndex, "Trống");
            }
        }
    },

    // ✅ Hàm cập nhật input trạng thái máng
    updateMangStatus: function (mangIndex, statusText) {
        const $input = $("#input" + mangIndex);

        if ($input.length) {
            $input.val(statusText);

            if (statusText === "Trống") {
                $input.removeClass("processing").addClass("empty");
            } else {
                $input.removeClass("empty").addClass("processing");
            }
        }
    },
    init: function () {
        _cartcalllist.ListCartoFactory();
        _cartcalllist.ListCartoFactory_Da_SL();
    },
    ListCartoFactory: function () {
        var model = {
            VehicleNumber: $('#input_chua_xu_ly').val(),
            PhoneNumber: $('#input_chua_xu_ly').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            VehicleWeighedstatus: 0,
            type: 0,
        }
        $.ajax({
            url: "/ListCar/ListCarCallView",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#data_chua_xu_ly').html(result);
                _cartcalllist.initMangStatus();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListCartoFactory_Da_SL: function () {

        var model = {
            VehicleNumber: $('#input_da_xu_ly').val(),
            PhoneNumber: $('#input_da_xu_ly').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: 0,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            VehicleWeighedstatus: 0,
            type: 1,
        }
        $.ajax({
            url: "/ListCar/ListCarCallView",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#data_da_xu_ly').html(result);
                _cartcalllist.initMangStatus();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    UpdateStatus: async function (id, status, type, weight) {
        var status_type = 1
        $.ajax({
            url: "/Car/UpdateStatus",
            type: "post",
            data: { id: id, status: status, type: type, weight: weight },
            success: function (result) {
                status_type = result.status;
                if (result.status == 0) {
                    _msgalert.success(result.msg);

                    // ✅ chỉ remove row nếu cập nhật thành công
                    if (type == 6) {
                        if (parseInt(status) == 0) {
                            $('#dataBody-0').find('.CartoFactory_' + id).remove();
                        } else {
                            $('#dataBody-1').find('.CartoFactory_' + id).remove();
                        }
                    }

                    // 🔥 Sau khi update → reload lại dữ liệu cả 2 bảng
                    _cartcalllist.ListCartoFactory();
                    _cartcalllist.ListCartoFactory_Da_SL();

                } else {
                    _msgalert.error(result.msg);
                }
               
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
              
        });
        return await status_type;
    }

}