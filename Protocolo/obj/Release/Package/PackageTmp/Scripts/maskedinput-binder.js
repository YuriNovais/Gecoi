$(function () {
    applyMask();
});

function applyMask() {
    $('[mask]').each(function (e) {
        $(this).mask($(this).attr('mask'));
    });
}