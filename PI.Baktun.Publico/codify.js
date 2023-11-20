//uglify-js
//node codify.js
var fs = require("fs");
var UglifyJS = require("uglify-js");

//uglifycss

var code = {
    file1: fs.readFileSync("./Spa/Controllers/HomeCtrl.js", "utf8"),
    file2: fs.readFileSync("./Spa/Controllers/authController.js", "utf8"),
    file3: fs.readFileSync("./Spa/Controllers/econsultaControllers.js", "utf8"),
    file4: fs.readFileSync("./Spa/Factories/authFactory.js", "utf8"),
    file5: fs.readFileSync("./Spa/App/register.js", "utf8"),
    file6: fs.readFileSync("../PI.Baktum.Intranet/Spa/Directives/GrlDirectives.js", "utf8"),
    file7: fs.readFileSync("../PI.Baktum.Intranet/Spa/Directives/feedback.js", "utf8")
};

var options = {
    warnings: true,
    mangle: {
        toplevel: true
    },
    compress: {
        dead_code: true,
        global_defs: {
            DEBUG: false
        }
    },
    output: {
        beautify: false,
        preamble: "/* rpi */"
    }
};
fs.writeFileSync("./Spa/App/publico.js", UglifyJS.minify(code, options).code, "utf8");


var uglifycss = require('uglifycss');

var uglified = uglifycss.processFiles(
    ['./Bootstrap/Custom.css', './Content/Site.css'],
    { maxLineLen: 500, expandVars: true }
);

fs.writeFileSync("./Bootstrap/Custom.min.css", uglified, "utf8");