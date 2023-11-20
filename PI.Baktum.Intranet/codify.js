//npm install
//uglify-js
//node codify.js
var fs = require("fs");
var UglifyJS = require("uglify-js");

//uglifycss
var base_code = {
    file1: fs.readFileSync("./Scripts/jrzStorage.js", "utf8"),
    file2: fs.readFileSync("./Spa/Factories/authFactory.js", "utf8"),
    file3: fs.readFileSync("./Spa/Controllers/appCtrlr.js", "utf8"),
    file4: fs.readFileSync("./Spa/Controllers/HomeCtrl.js", "utf8"),
    file5: fs.readFileSync("./Spa/Controllers/authController.js", "utf8"),

    file6: fs.readFileSync("./Spa/Directives/GrlDirectives.js", "utf8"),
    file7: fs.readFileSync("./Spa/Directives/Feedback.js", "utf8"),
    file8: fs.readFileSync("./Spa/App/appBase.js", "utf8"),
    file9: fs.readFileSync("./Spa/App/Temp.js", "utf8")
};

var admin_code = {
    file1: fs.readFileSync("./Scripts/jrzStorage.js", "utf8"),
    file2: fs.readFileSync("./Spa/Controllers/appCtrlr.js", "utf8"),
    file3: fs.readFileSync("./Spa/Controllers/AgentesCtrlr.js", "utf8"),
    file4: fs.readFileSync("./Spa/Controllers/EstatusCtrlr.js", "utf8"),
    file5: fs.readFileSync("./Spa/Controllers/EventosCtrlr.js", "utf8"),
    file6: fs.readFileSync("./Spa/Controllers/OpsXEventoCtrlr.js", "utf8"),
    file7: fs.readFileSync("./Spa/Controllers/PaisesCtrlr.js", "utf8"),
    file8: fs.readFileSync("./Spa/Controllers/LeyesCtrlr.js", "utf8"),
    file9: fs.readFileSync("./Spa/Controllers/NizaCtrlr.js", "utf8"),
    file10: fs.readFileSync("./Spa/Controllers/ViennaCtrlr.js", "utf8"),
    file11: fs.readFileSync("./Spa/Controllers/RolesCtrlr.js", "utf8"),
    file12: fs.readFileSync("./Spa/Controllers/UsuariosCtrlr.js", "utf8"),
    file13: fs.readFileSync("./Spa/Controllers/UsuariosExternosCtrlr.js", "utf8"),

    file14: fs.readFileSync("./Spa/Factories/authFactory.js", "utf8"),
    file15: fs.readFileSync("./Spa/Factories/UsersFactory.js", "utf8"),
    file16: fs.readFileSync("./Spa/Factories/pageFactory.js", "utf8"),
    file17: fs.readFileSync("./Spa/Directives/GrlDirectives.js", "utf8"),
    file18: fs.readFileSync("./Spa/Directives/Feedback.js", "utf8"),

    file19: fs.readFileSync("./Spa/App/appAdmin.js", "utf8"),
    file20: fs.readFileSync("./Spa/App/Temp.js", "utf8")
};

var ges_code = {
    file1: fs.readFileSync("./Scripts/jrzStorage.js", "utf8"),
    file2: fs.readFileSync("./Scripts/ui-utils.js", "utf8"),

    file3: fs.readFileSync("./Spa/Factories/authFactory.js", "utf8"),
    file4: fs.readFileSync("./Spa/Factories/formUtils.js", "utf8"),

    file5: fs.readFileSync("./Spa/Controllers/appCtrlr.js", "utf8"),
    file6: fs.readFileSync("./Spa/Controllers/MenueRPICtrl.js", "utf8"),
    file7: fs.readFileSync("./Spa/Controllers/eRPICtrl.js", "utf8"),
    file8: fs.readFileSync("./Spa/Controllers/CronCtrlr.js", "utf8"),
    file9: fs.readFileSync("./Spa/Controllers/MarcasCtrl.js", "utf8"),
    file10: fs.readFileSync("./Spa/Controllers/AnotacionesCtrlr.js", "utf8"),
    file11: fs.readFileSync("./Spa/Controllers/RenovacionesCtrlr.js", "utf8"),
    file12: fs.readFileSync("./Spa/Controllers/marcas-busquedas-ctrlr.js", "utf8"),
    file13: fs.readFileSync("./Spa/Controllers/DACtrl.js", "utf8"),
    file14: fs.readFileSync("./Spa/Controllers/PatenteCtrl.js", "utf8"),
    file15: fs.readFileSync("./Spa/Controllers/authController.js", "utf8"),

    file16: fs.readFileSync("./Spa/Factories/UsersFactory.js", "utf8"),
    file17: fs.readFileSync("./Spa/Directives/GrlDirectives.js", "utf8"),
    file18: fs.readFileSync("./Spa/Directives/Feedback.js", "utf8"),

    file19: fs.readFileSync("./Spa/App/appeRPI.js", "utf8"),
    file20: fs.readFileSync("./Spa/App/Temp.js", "utf8")
};

var recep_code = {
    file1: fs.readFileSync("./Scripts/jrzStorage.js", "utf8"),
    file2: fs.readFileSync("./Scripts/ui-utils.js", "utf8"),

    file3: fs.readFileSync("./Spa/Factories/authFactory.js", "utf8"),

    file4: fs.readFileSync("./Spa/Controllers/appCtrlr.js", "utf8"),
    file5: fs.readFileSync("./Spa/Controllers/Recepcion/AvisosCtrlr.js", "utf8"),

    file6: fs.readFileSync("./Spa/Factories/UsersFactory.js", "utf8"),
    file7: fs.readFileSync("./Spa/Directives/GrlDirectives.js", "utf8"),
    file8: fs.readFileSync("./Spa/Directives/Feedback.js", "utf8"),

    file9: fs.readFileSync("./Spa/App/appRecepcion.js", "utf8"),
    file10: fs.readFileSync("./Spa/App/Temp.js", "utf8")
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
        preamble: "/* rpi GES*/"
    }
};


fs.writeFileSync("./Spa/App/rpi_base.js", UglifyJS.minify(base_code, options).code, "utf8");
fs.writeFileSync("./Spa/App/rpi_admin.js", UglifyJS.minify(admin_code, options).code, "utf8");
fs.writeFileSync("./Spa/App/rpi_ges.js", UglifyJS.minify(ges_code, options).code, "utf8");
fs.writeFileSync("./Spa/App/rpi_recep.js", UglifyJS.minify(recep_code, options).code, "utf8");


//var uglifycss = require('uglifycss');

//var uglified = uglifycss.processFiles(
//    ['./Bootstrap/Custom.css', './Content/Site.css'],
//    { maxLineLen: 500, expandVars: true }
//);

//fs.writeFileSync("./Bootstrap/Custom.min.css", uglified, "utf8");