
$(function () {
    $('[data-utcdate]').each(function () {
        var d = moment($(this).attr('data-utcdate'));
        $(this).html(d.fromNow());
    });

    //Widgets count plugin
    function initCounters() {
        $('.count-to').countTo();
    }

    function formatMoney(n, c, d, t) {
        var c = isNaN(c = Math.abs(c)) ? 2 : c,
            d = d == undefined ? "." : d,
            t = t == undefined ? "," : t,
            s = n < 0 ? "-" : "",
            i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
            j = (j = i.length) > 3 ? j % 3 : 0;

        return s +
            (j ? i.substr(0, j) + t : "") +
            i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) +
            (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
    };
    $(".moneyValue").each(function () {
        var $this = $(this);
        var $money = $this.text();
        $money = formatMoney($money);
        $this.text($money);
    });
})