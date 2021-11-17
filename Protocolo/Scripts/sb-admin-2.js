function clearFormElements(className) {
    $("." + className).find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'text':
            case 'textarea':
            case 'file':
            case 'select-one':
            case 'select-multiple':
                $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }
    });
}

function applyMaxLength() {
    $("input[data-val-length-max]").each(function () {
        var length = parseInt($(this).attr("data-val-length-max"));
        $(this).prop("maxlength", length);
    });
}

function hideButtons() {
    $('button.toggle-enable').each(function () {
        $(this).css('visibility', 'hidden');
    });
}

function initDatepicker() {
    $('.date').datetimepicker({
        locale: 'pt-br',
        format: 'L'
    });
}

function clearSelection(selectId) {
    $(selectId + ' option').removeAttr('selected');
}

function append(selectables, selectedIds) {
    $(selectables + ' option:selected').removeAttr('selected').appendTo(selectedIds);
}

function appendAll(selectables, selectedIds) {
    $(selectables + ' option').removeAttr('selected').appendTo(selectedIds);
}

function remover(selectables, selectedIds) {
    $(selectedIds + ' option:selected').removeAttr('selected').appendTo(selectables);
}

function removeAll(selectables, selectedIds) {
    $(selectedIds + ' option').removeAttr('selected').appendTo(selectables);
}

function hideModal() {
    $('#modal-container').modal('hide');
}

function confirmAction(elem) {
    var $form = elem.closest('form');

    if ($form.valid()) {
        var confirmationDialog = $('#confirmationDialog');

        confirmationDialog.attr('data-id', $form.attr('id'));

        confirmationDialog.modal('show');
    } else {
        var validator = $form.validate();

        validator.errorList[0].element.focus();
    }
}

function writeView(title, data) {
    var mywindow = window.open('', title, '');

    mywindow.document.write('<html><head><title>' + title + '</title>');
    mywindow.document.write('<link href="' + ROOT + 'Content/bootstrap.css" rel="stylesheet">');
    mywindow.document.write('<link href="' + ROOT + 'Content/font-awesome.css" rel="stylesheet">');
    mywindow.document.write('<link href="' + ROOT + 'Content/sb-admin-2.css" rel="stylesheet">');
    mywindow.document.write('<link href="' + ROOT + 'Content/print.css" rel="stylesheet" media="print">');
    mywindow.document.write('</head><body>');
    mywindow.document.write(data);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10
}

$(function () {
    $('#side-menu').metisMenu();

    applyMaxLength();

    hideButtons()

    $('a.collapsable').click(function () {
        $(this).find('i').toggleClass('fa-plus-circle fa-minus-circle')
    });

    $(document).on('change', '.selectAll', function () {
        var form = $(this).closest('form');

        form.find('.checkbox_class:checkbox').prop('checked', $(this).prop('checked'));

        form.find('button.toggle-enable').css('visibility', $('.checkbox_class:checked').length == 0 ? 'hidden' : 'visible');
    });

    $(document).on('click', '.checkbox_class', function () {
        var checks_length = $('.checkbox_class').length;
        var checked_length = $('.checkbox_class:checked').length;

        var form = $(this).closest('form');
        
        form.find('.selectAll').prop('checked', checks_length == checked_length);

        form.find('button.toggle-enable').css('visibility', checked_length == 0 ? 'hidden' : 'visible');
    });

    if ($.fn.dataTable) {
        $.extend($.fn.dataTable.defaults, {
            'aoColumnDefs': [{
                'bSortable': false,
                'aTargets': ['no-sort']
            }]
        });

        var _re_new_lines = /[\r\n]/g;

        $.extend($.fn.dataTable.ext.type.search, {
            html: function (data) {
                return _empty(data) ? data : typeof data === 'string' ? data.replace(_re_new_lines, ' ') : '';
            }
        })

        var _empty = function (d) {
            return !d || d === true || d === '-' ? true : false;
        };
    }

    (function ($) {
        $.fn.cascade = function (options) {
            var defaults = {};
            var opts = $.extend(defaults, options);

            return this.each(function () {
                $(this).change(function () {
                    var selectedValue = $(this).val();
                    var params = {};
                    params[opts.paramName] = selectedValue;

                    $.getJSON(opts.url, params, function (items) {
                        opts.childSelect.empty();

                        opts.childSelect.append('<option value>--Selecione--</option>');

                        $.each(items, function (index, item) {
                            opts.childSelect.append(
                                $('<option/>')
                                    .attr('value', item.Id)
                                    .text(item.Nome)
                            );
                        });
                    });
                });
            });
        };
    })(jQuery);

    function centerModal() {
        $(this).css('display', 'block');

        var $dialog = $(this).find(".modal-dialog"),

        offset = ($(window).height() - $dialog.height()) / 2,
        bottomMargin = parseInt($dialog.css('marginBottom'), 10);

        if (offset < bottomMargin) {
            offset = bottomMargin;
        }

        $dialog.css("margin-top", offset);
    }

    $(document).on('show.bs.modal', '.modal', centerModal);

    $(window).bind("load resize", function() {
        topOffset = 50;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;

        if (height < 1) {
            height = 1;
        }
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }

        $('.modal:visible').each(centerModal);
    });

    var url = window.location;

    var element = $('ul.nav a').filter(function() {
        return this.href == url || url.href.indexOf(this.href) == 0;
    }).addClass('active').parent().parent().addClass('in').parent();

    if (element.is('li')) {
        element.addClass('active');
    }
});