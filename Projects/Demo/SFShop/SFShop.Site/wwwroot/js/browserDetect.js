function init(){
    var ss= {};
    var ua = window.navigator.userAgent;
    var i = ua.indexOf("MSIE");
    if (i == -1) 
        return ss;
    i+=4;
    var e = ua.indexOf(";",i);
    var v = parseInt(ua.substring(i, e));
    ss.isIE = true;
    ss.version = v;
    return ss;
}

function showAlert(uv) {
    var ba = document.getElementById("browser-alert");
    if (!ba) return;
    //" + uv.version + " 
    ba.innerHTML = "您的浏览器较旧,为了快速夺宝," +
                "建议使用<a target='_blank' href='https://www.baidu.com/s?wd=%E8%B0%B7%E6%AD%8C%E6%B5%8F%E8%A7%88%E5%99%A8'>谷歌浏览器</a>。"+
                "<a href='javascript:;' id='browser-express-mode'>若您正在使用360等浏览器，请改用 “急速模式”。" +
                "<img src='/content/desktop/images/express-mode.png'/></a>";
     
    ba.style.display = "block";
    
}

var uv = init();
if(uv.isIE && uv.version<10){
    showAlert(uv);
}

