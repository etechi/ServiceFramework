/// <binding />
var gulp = require("gulp");
var webpack = require("webpack");
var webpackConfig = require('./client/node_modules/SF/build-tools/webpack2.config');
var fs = require("fs");
var build_webapi = require("./client/node_modules/SF/build-tools/build_api_types.js");
var path = require("path");

function call_webpack(entrys, release, verbose, cb) {
    var cfg = webpackConfig({
        entities:entrys, 
        debug: !release,
        verbose: verbose,
        basePath: __dirname,
        outputPath: path.resolve(__dirname,"./wwwroot/js/admin") 
    });
    webpack(cfg).run((err, stats) => {
        if (err) {
            cb(err)
            return;
        }
        console.log(stats.toString(cfg.stats));
        cb();
    });
}
gulp.task("build", (cb) => {
    call_webpack({
        admin: './SFAdminSite/index.tsx',
    }, false, false,  cb);
});

gulp.task("build-release", (cb) => {
    call_webpack({
        admin: './SFAdminSite/index.tsx',
    }, true, false, cb);
});
gulp.task("build-webapi-all-interface", (cb) => {
    build_webapi(
        "http://localhost:52706/api/servicemetadata/typescript?all=true",
        path.join(__dirname, "./client/SFAdminSite/webapi-all.ts"),
        cb
    );
});
gulp.task("build-webapi-user-interface", (cb) => {
    build_webapi(
        "http://localhost:52706/api/servicemetadata/typescript?all=false",
        path.join(__dirname, "./client/SFAdminSite/webapi.ts"),
        cb
    );
});
gulp.task("build-default-style", () => {
    var ds = fs.readFileSync("./content/defaultStyle.scss", "utf8");
    var code = "export = {" +
        ds.split('\n').map(s => s.trim()).filter(s => s[0] === '$').map(s => {
            var p = s.split(':');
            return p[0].substring(1) + ":\"" + p[1].substring(0, p[1].length - 1) + "\""
        }).join(",\n")
        + "\n};";
    fs.writeFileSync("./client/defaultStyle.ts", code);
});