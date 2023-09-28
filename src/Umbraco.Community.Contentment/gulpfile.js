const gulp = require("gulp"),
    concat = require("gulp-concat"),
    cleanCSS = require("gulp-clean-css"),
    rename = require("gulp-rename"),
    strip = require("gulp-strip-comments"),
    uglify = require("gulp-uglify");

function cshtml() {
    return gulp
        .src(["./DataEditors/**/*.cshtml"])
        .pipe(rename({ dirname: "" }))
        .pipe(gulp.dest("./wwwroot/App_Plugins/Contentment/render"));
};

function css() {
    return gulp
        .src(["./DataEditors/**/*.css", "./Web/UI/*.css"])
        .pipe(concat('contentment.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest("./wwwroot/App_Plugins/Contentment"));
};

function html() {
    return gulp
        .src(["./DataEditors/**/*.html"])
        .pipe(rename({ dirname: "" }))
        .pipe(strip())
        .pipe(gulp.dest("./wwwroot/App_Plugins/Contentment/editors"));
};

function js() {
    return gulp
        .src(["./DataEditors/**/*.js", "./Web/UI/*.js"])
        .pipe(concat('contentment.js'))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/App_Plugins/Contentment"));
};

gulp.task("build", gulp.series(cshtml, css, html, js));

gulp.task("watch", function () {
    gulp.watch(["./DataEditors/**/*.css", "./Web/UI/*.css"], gulp.series(css));
    gulp.watch(["./DataEditors/**/*.html"], gulp.series(html));
    gulp.watch(["./DataEditors/**/*.js", "./Web/UI/*.js"], gulp.series(js));
});
