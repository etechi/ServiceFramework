/// <binding />
var gulp = require("gulp");
var webpack = require("webpack");
var webpackConfig = require('./client/node_modules/SF/build-tools/webpack.config');
var fs = require("fs");
var build_webapi = require("./client/node_modules/SF/build-tools/build_api_types.js");
var path = require("path");

function call_webpack(entrys, release, verbose, cb) {
    var cfg = webpackConfig({
        entities:entrys, 
        debug: !release,
        verbose: verbose,
        basePath: __dirname,
        outputPath: path.resolve(__dirname,"./wwwroot/js") 
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
        admin: './admin/index.tsx',
        desktop: './desktop/index.ts',
        mobile: './mobile/index.tsx',
    }, false, false, path. cb);
});
gulp.task("build-desktop", (cb) => {
    call_webpack({
        desktop: './desktop/index.ts',
    }, false, false, cb);
});
gulp.task("build-desktop-release", (cb) => {
    call_webpack({
        desktop: './desktop/index.ts',
    }, true, false, cb);
});
gulp.task("build-mobile", (cb) => {
    call_webpack({
        mobile: './mobile/index.tsx',
    }, false, false, cb);
});
gulp.task("build-mobile-release", (cb) => {
    call_webpack({
        mobile: './mobile/index.tsx',
    }, true, false, cb);
});
gulp.task("build-admin", (cb) => {
    call_webpack({
        admin: './admin/index.tsx',
    }, false, false, cb);
});
gulp.task("build-admin-release", (cb) => {
    call_webpack({
        admin: './admin/index.tsx',
    }, true, false, cb);
});
gulp.task("build-verbose", (cb) => {
    call_webpack(false, true, cb);
});
gulp.task("build-release", (cb) => {
    call_webpack({
        admin: './admin/index.tsx',
        mobile: './mobile/index.tsx',
        desktop: './desktop/index.ts',
    }, true, false, cb);
});
gulp.task("build-release-verbose", (cb) => {
    call_webpack(true, true, cb);
});
gulp.task("build-webapi-all-interface", (cb) => {
    build_webapi(
        "http://localhost:56622/api/servicemetadata/typescript?all=true",
        path.join(__dirname, "client/admin/webapi-all.ts"),
        cb
    );
});
gulp.task("build-webapi-user-interface", (cb) => {
    build_webapi(
        "http://localhost:56622/api/servicemetadata/typescript?all=false",
        path.join(__dirname, "client/admin/webapi.ts"),
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