var fs = require("fs")
var superagent = require("superagent");
module.exports=function build_api_types(uri, file,cb) {
    superagent(uri).end(function (e, res){
       if (e) {
            cb(e);
            return;
        }
        fs.writeFileSync(file,res.text);
        cb(e);        
    });
}
