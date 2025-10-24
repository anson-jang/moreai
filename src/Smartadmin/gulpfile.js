const { series, src, dest, parallel, watch } = require("gulp");
const fs = require('fs');
const autoprefixer = require("gulp-autoprefixer");
const browsersync = require("browser-sync");
const concat = require("gulp-concat");
const CleanCSS = require("gulp-clean-css");
const del = require("del");
const path = require('path');
const fileinclude = require("gulp-file-include");
const newer = require("gulp-newer");
const rename = require("gulp-rename");
const rtlcss = require("gulp-rtlcss");
const sourcemaps = require("gulp-sourcemaps");
const sass = require("gulp-sass")(require("sass"));
const uglify = require("gulp-uglify");
const replace = require('gulp-replace');
const gulpIf = require('gulp-if');
const cheerio = require('cheerio');
const svgo = require('svgo');

const pluginFile = require("./plugins.config"); // Import the plugins list

const paths = {
    baseSrc: "wwwroot/",                // source directory
    baseDist: "wwwroot/",              // build directory
};

const clean = function (done) {
    del.sync(paths.baseDist, done());
};

// Warning not show
process.removeAllListeners('warning');

// Copying Third Party Plugins Assets
const plugins = function () {
    const out = paths.baseDist + "plugins/";

    pluginFile.forEach(({ name, vendorsJS, vendorCSS, vendorFonts, assets, fonts, font, media, img, webfonts }) => {

        const handleError = (label, files) => (err) => {
            const shortMsg = err.message.split('\n')[0];
            console.error(`\n${label} - ${shortMsg}`);
            throw new Error(`${label} failed`);
        };

        if (vendorsJS) {
            src(vendorsJS)
                .on('error', handleError('vendorsJS'))
                .pipe(concat("vendors.min.js"))
                .pipe(dest(paths.baseDist + "scripts/"));
        }

        if (vendorCSS) {
            src(vendorCSS)
                .pipe(concat("vendors.min.css"))
                .on('error', handleError('vendorCSS'))
                .pipe(dest(paths.baseDist + "css/"));
        }

        if (vendorFonts) {
            src(vendorFonts)
                .on('error', handleError('vendorFonts'))
                .pipe(dest(paths.baseDist + "css/fonts/"));
        }

        if (assets) {
            src(assets)
                .on('error', handleError('assets'))
                .pipe(dest(`${out}${name}/`));
        }

        if (img) {
            src(img)
                .on('error', handleError('img'))
                .pipe(dest(`${out}${name}/images/`));
        }

        if (media) {
            src(media)
                .on('error', handleError('media'))
                .pipe(dest(`${out}${name}/`));
        }


        if (fonts) {
            src(fonts)
                .on('error', handleError('fonts'))
                .pipe(dest(`${out}${name}/fonts/`));
        }

        if (font) {
            src(font)
                .on('error', handleError('font'))
                .pipe(dest(`${out}${name}/font/`));
        }

        if (webfonts) {
            src(webfonts)
                .on('error', handleError('webfonts'))
                .pipe(dest(`${out}${name}/webfonts/`));
        }
    });

    return Promise.resolve();
};

const json = function () {
    const out = paths.baseDist + "json/";
    return src([paths.baseSrc + "json/**/*"])
        .pipe(dest(out));
};

const icons = function () {
    const out = paths.baseDist + "icons/";
    return src([paths.baseSrc + "icons/**/*"])
        .pipe(dest(out));
};

const webfonts = function () {
    const out = paths.baseDist + "webfonts/";

    src(paths.baseSrc + "webfonts/**/*.scss")
        .pipe(sourcemaps.init())
        .pipe(sass.sync().on('error', sass.logError)) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(CleanCSS())
        .pipe(dest(out));

    return src([paths.baseSrc + "webfonts/**/*", "!" + paths.baseSrc + "webfonts/**/*.scss"])
        .pipe(newer(out))
        .pipe(dest(out));
};

const images = function () {
    const out = paths.baseDist + "img";
    return src(paths.baseSrc + "img/**/*")
        .pipe(dest(out));
};

const media = function () {
    const out = paths.baseDist + "media";
    return src(paths.baseSrc + "media/**/*")
        .pipe(dest(out));
};

const javascript = function () {
    const out = paths.baseDist + "scripts/";

    // copying and minifying all other scripts
    return src(paths.baseSrc + "scripts/**/*.js")
        // .pipe(uglify())
        .pipe(dest(out));
};

function notMap(file) {
    // returns false if the file is anywhere under an "icon" folder
    const parts = file.path.split(path.sep);
    return !(parts.includes('themes'));
}

const css = function () {
    const out = paths.baseDist + "css/";

    return src(paths.baseSrc + "sass/**/*.scss")
        .pipe(sourcemaps.init())
        .pipe(sass.sync().on('error', sass.logError)) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(dest(out))
        .pipe(CleanCSS())
        .pipe(gulpIf(notMap, rename({ suffix: '.min' })))
        .pipe(gulpIf(notMap, sourcemaps.write('./')))
        .pipe(dest(out));
};

const htmlRTL = function () {
    const srcPath = paths.baseSrc + "/";
    const out = paths.baseDist;
    return src([
        srcPath + "*.html",
        srcPath + "*.ico", // favicon
        srcPath + "*.png",
    ])
        .pipe(
            fileinclude({
                prefix: "@@",
                basepath: "@file",
                indent: true,
            })
        )

        // Add dir="rtl" to <html> tag
        .pipe(replace(
            /<html(.*?)>/,
            '<html$1 dir="rtl">'
        ))

        // Replace smartapp.css with smartapp-rtl.css
        .pipe(replace(
            /href="css\/(smartapp)(\.min)?\.css"/g,
            'href="css/$1-rtl$2.css"'
        ))

        .pipe(dest(out));
};


const rtlCSS = function () {

    const out = paths.baseDist + "css/";

    return src(paths.baseSrc + "sass/**/*.scss")
        .pipe(sourcemaps.init())
        .pipe(sass.sync().on('error', sass.logError)) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(rtlcss())
        .pipe(gulpIf(notMap, rename({ suffix: '-rtl' })))
        .pipe(dest(out))
        .pipe(CleanCSS())
        .pipe(gulpIf(notMap, rename({ suffix: '.min' })))
        .pipe(gulpIf(notMap, sourcemaps.write('./')))
        .pipe(dest(out));
}


const reloadBrowserSync = function (done) {
    browsersync.reload();
    done();
}

function watchFiles() {
    watch(paths.baseSrc + "icons/**/*", series(icons, reloadBrowserSync));
    watch(paths.baseSrc + "webfonts/**/*", series(webfonts, reloadBrowserSync));
    watch(paths.baseSrc + "json/**/*", series(json, reloadBrowserSync));
    watch(paths.baseSrc + "img/**/*", series(images, reloadBrowserSync));
    watch(paths.baseSrc + "scripts/**/*.js", series(javascript, reloadBrowserSync));
    watch(paths.baseSrc + "sass/**/*.scss", series(css, reloadBrowserSync));
}


function watchFilesRTL() {
    watch(paths.baseSrc + "icons/**/*", series(icons, reloadBrowserSync));
    watch(paths.baseSrc + "webfonts/**/*", series(webfonts, reloadBrowserSync));
    watch(paths.baseSrc + "json/**/*", series(json, reloadBrowserSync));
    watch(paths.baseSrc + "img/**/*", series(images, reloadBrowserSync));
    watch(paths.baseSrc + "scripts/**/*.js", series(javascript, reloadBrowserSync));
    watch(paths.baseSrc + "sass/**/*.scss", series(rtlCSS, reloadBrowserSync));
}

// Production Tasks
exports.default = series(
    plugins,
    parallel( webfonts, css),
    parallel(watchFiles)
);

// Build Tasks
exports.build = series(
    plugins,
    parallel( webfonts, css)
);

// RTL Tasks
exports.rtl = series(
    htmlRTL,
    plugins,
    parallel(rtlCSS),
    parallel(watchFilesRTL)
);

// RTL Build Tasks
exports.rtlbuild = series(
    htmlRTL,
    plugins,
    parallel(rtlCSS),
);