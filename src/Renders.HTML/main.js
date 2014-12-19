// Avoid `console` errors in browsers that lack a console.
(function () {
    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

// Place any jQuery/helper plugins in here.
// http://stackoverflow.com/a/9795091
$.fn.wrapInTag = function (opts) {
    // http://stackoverflow.com/a/1646618
    function getText(obj) {
        return obj.textContent ? obj.textContent : obj.innerText;
    }

    var tag = opts.tag || 'strong',
        words = opts.words || [],
        regex = RegExp(words.join('|'), 'gi'),
        replacement = '<' + tag + '>$&</' + tag + '>';

    // http://stackoverflow.com/a/298758
    $(this).contents().each(function () {
        if (this.nodeType === 3) //Node.TEXT_NODE
        {
            // http://stackoverflow.com/a/7698745
            $(this).replaceWith(getText(this).replace(regex, replacement));
        }
        else if (!opts.ignoreChildNodes) {
            $(this).wrapInTag(opts);
        }
    });
};
