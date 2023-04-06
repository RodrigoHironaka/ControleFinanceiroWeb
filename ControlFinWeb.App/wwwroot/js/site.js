function TipoClick() {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        return 'touchstart';
    } else {
        return 'click';
    }
}

function CheckAll() {
    $('#checkAll').change(function () {
        $('.check').prop('checked', this.checked);
    });
}

function ClickCheckItem() {
    $('.check').change(function () {
        if ($('.check:checked').length == $('.check').length) {
            $('#checkAll').prop('checked', true);
        }
        else {
            $('#checkAll').prop('checked', false);
        }
    });
}

function HabilitarToolTip() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
}
