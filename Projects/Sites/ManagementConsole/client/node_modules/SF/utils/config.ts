export const res_base = "/r/";
export function res(url: string, format?: string) {
    var re = res_base + url;
    if (format)
        re += "?format=" + format;
    return re;
}
 
