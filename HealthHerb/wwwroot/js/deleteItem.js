function deleteItem(config) {
    if (!confirm(config.confirmMessage)) {
        return false;
    }

    let element = $(config.ele);
    let currentHTML = element.html();

    element.attr('disabled', 'disabled').html('<i class="fa fa-fw fa-spinner fa-pulse"></i>');

    $.ajax({
        method: 'POST',
        url: config.url,
        data: config.data,
        success: function () {
            $('#' + config.id).remove();
        },
        error: function () {
            element.removeAttr('disabled').html(currentHTML);
            alert(config.errorMessage)
        }
    });
}