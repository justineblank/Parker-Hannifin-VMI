// Write your Javascript code.
Date.prototype.mmddyyyyhhmmtt = function (dateSplitter, timeSplitter) {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();
    var hour = this.getHours();
    var min = this.getMinutes();
    var tt = (hour > 11 ? 'PM' : 'AM');
    hour = (hour > 12 ? hour - 12 : hour);
    return [
        (mm > 9 ? '' : '0') + mm, dateSplitter,
        (dd > 9 ? '' : '0') + dd, dateSplitter,
        this.getFullYear(), ' ',
        (hour > 9 ? '' : '0') + hour, timeSplitter,
        (min > 9 ? '' : '0') + min, ' ',
        tt

    ].join('');
};