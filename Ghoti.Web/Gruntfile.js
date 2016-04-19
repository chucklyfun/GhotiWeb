module.exports = function(grunt) 
{

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        watch: {
            less: {
                files: ['bower.json'],
                tasks: ['exec:bower_install']
            },
        },
        exec: {
            bower_install: {
                cmd: 'bower install'
            },
        },


        bower_concat: {
            all: {
                dest: 
                {
                    js: 'content/js/app/bower.js',
                    css: 'content/public/css/bower.css',
                },
            },
        },

    });

    grunt.loadNpmTasks('grunt-bower-concat');


}

