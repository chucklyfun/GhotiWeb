if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt, fun) {
        var len = this.length;

        var from = Number(arguments[1]) || 0;
        from = (from < 0)
             ? Math.ceil(from)
             : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (fun === undefined)
            {
                if (this[from] === elt)
                return from;
            }
            else if (fun(value))
            {
                return from;
            }
        }
        return -1;
    };
}

if (!Array.prototype.replace) {
    Array.prototype.replace = function (value, fun) {
        var index = items.indexOf(value, fun);

        if (~index) {
            items[index] = value;
        }

    };
}
