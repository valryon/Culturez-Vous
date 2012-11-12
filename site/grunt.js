var exec = require("child_process").exec;

// Gruntfile
// Set a server.
module.exports = function(grunt) {

	grunt.initConfig({
		server: {
			base: './bin'
		},
		
		watch : {
			files: ['./src/index.jade', './src/site.less'],
			tasks: 'build'
		}
	});

	grunt.registerTask('default', 'server watch');
	grunt.registerTask('build', 'build project', function () {
		console.log('Building...')
		exec('./build.sh', function () {
			console.log('\nBuild done.');
		});
	})
};