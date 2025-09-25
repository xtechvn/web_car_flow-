$(document).ready(function () {
    _cartofactory.init();
    var input_chua_xu_ly = document.getElementById("input_chua_xu_ly");
    input_chua_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _cartofactory.ListCartoFactory();
        }
    });
    var input_da_xu_ly = document.getElementById("input_da_xu_ly");
    input_da_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _cartofactory.ListCartoFactory_Da_SL();
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
                .attr('data-value',opt.value) // Corrected from opt.valuse
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


                $currentBtn
                    .text(text)
                    .removeClass(function (_, old) {
                        return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                    }) // xoá các class status- cũ
                    .addClass(cls); // gắn class mới (status-arrived, status-blank…)

                _cartofactory.UpdateStatus(id_row, val_TT, 1);
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
  
});
var _cartofactory = {

init: function () {
    _cartofactory.ListCartoFactory();
    _cartofactory.ListCartoFactory_Da_SL();
},
ListCartoFactory: function () {
    var model = {
        VehicleNumber: $('#input_chua_xu_ly').val(),
        PhoneNumber: $('#input_chua_xu_ly').val(),
        VehicleStatus: null,
        LoadType: null,
        VehicleWeighingType: null,
        VehicleTroughStatus: null,
        TroughType: null,
        VehicleWeighingStatus: null,
    }
    $.ajax({
        url: "/Car/ListCartoFactory",
        type: "post",
        data: { SearchModel: model },
        success: function (result) {
            $('#imgLoading').hide();
            $('#data_chua_xu_ly').html(result);
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
        VehicleWeighingType: null,
        VehicleTroughStatus: null,
        TroughType: null,
        VehicleWeighingStatus: null,
    }
    $.ajax({
        url: "/Car/ListCartoFactory",
        type: "post",
        data: { SearchModel: model },
        success: function (result) {
            $('#imgLoading').hide();
            $('#data_da_xu_ly').html(result);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("Status: " + textStatus);
        }
    });
},
OpenPopup: function (id) {
    let title = 'Cập nhật trạng thái';
    let url = '/Car/OpenPopup';
    let param = {
        id: id,
        type: 1
    };

    _magnific.OpenSmallPopup(title, url, param);

    },
    UpdateStatus: function (id, status,type) {
        $.ajax({
            url: "/Car/UpdateStatus",
            type: "post",
            data: { id: id, status: status, type: type },
            success: function (result) {
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
    }
}