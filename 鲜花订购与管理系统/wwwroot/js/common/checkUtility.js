; (function ($) {
    $.checkPhone = function (val) {
        return /^1[34578]\d{9}$/.test(val)
    }
})(jQuery);