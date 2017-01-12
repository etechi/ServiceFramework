
export function post(uri: string) {
    var form = document.createElement("form");
    var i = uri.indexOf('?');
    if (i != -1) {
        var args = uri.substring(i + 1);
        uri = uri.substring(0, i);
        args.split('&').map(p => p.split('=')).forEach(p => {
            var input = document.createElement("input");
            input.type = "hidden";
            input.name = decodeURIComponent(p[0]);
            input.value = decodeURIComponent(p[1]);
            form.appendChild(input);
        });
    }
    form.action = uri;
    form.method = "post";
    var s = form.style;
    s.position = "absolute";
    s.top = "-100px";
    s.width = "0";
    s.height = "0";
    document.body.appendChild(form);
    form.submit();    
}