declare var require: any;

var moment = require("moment");
var moment_locale = require("moment/locale/zh-cn");
moment.locale("zh-cn");
export function format(t: string | Date, fmt?: string): string{
    if (!t) return "";
    var m = moment(t);
    if (!m.hour() && !m.minute() && !m.second())
        return m.format("Y-MM-DD");
    return m.format(fmt || "Y-MM-DD HH:mm:ss");
}

export function formatEndDate(t: string | Date): string {
    if (!t) return "";
    var m = moment(t);
    if (!m.hour() && !m.minute() && !m.second()) {
        m = m.subtract(1, 'days');
        return m.format("Y-MM-DD");
    }
    return m.format("Y-MM-DD HH:mm");
}
export function valueOf(t:string|Date):number{
    return moment(t).valueOf();
}
export function parse(t: string): Date {
    return moment(t);
} 