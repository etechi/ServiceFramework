/**
 * React Starter Kit (https://www.reactstarterkit.com/)
 *
 * Copyright © 2014-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

var path =require('path');
var webpack=require('webpack');
var extend =require('extend');
var AssetsPlugin =require( 'assets-webpack-plugin');
var TsConfigPathsPlugin = require('awesome-typescript-loader').TsConfigPathsPlugin;


module.exports = function (cfg) {
    var entities = cfg.entities;
    var DEBUG = !!cfg.debug;
    var VERBOSE = !!cfg.verbose;
    var basePath = cfg.basePath;

    var outputPath = cfg.outputPath || path.resolve(basePath, './js');
    var contextPath = cfg.contextPath || path.resolve(basePath, './client');
    var publicPath = cfg.publicPath || '/scripts/';

	const AUTOPREFIXER_BROWSERS = [
	  'Android 2.3',
	  'Android >= 4',
	  'Chrome >= 35',
	  'Firefox >= 31',
	  'Explorer >= 9',
	  'iOS >= 7',
	  'Opera >= 12',
	  'Safari >= 7.1',
	];
	const GLOBALS = {
		'process.env.NODE_ENV': DEBUG ? '"development"' : '"production"',
		__DEV__: DEBUG,
		'process.env.BROWSER': true
	};

	//
	// Common configuration chunk to be used for both
	// client-side (client.js) and server-side (server.js) bundles
	// -----------------------------------------------------------------------------

	const config = {
        context: contextPath,
		output: {
            path: outputPath,
            publicPath: publicPath,
			sourcePrefix: '  ',
		},

		module: {
			rules: [
			  {
			  	test: /\.css/,
			  	use: [
				  'isomorphic-style-loader',
				  `css-loader?${JSON.stringify({
				  	sourceMap: DEBUG,
				  	// CSS Modules https://github.com/css-modules/css-modules
				  	modules: true,
				  	localIdentName: DEBUG ? '[name]_[local]_[hash:base64:3]' : '[hash:base64:4]',
				  	// CSS Nano http://cssnano.co/options/
				  	minimize: !DEBUG,
				  })}`,
				  'postcss-loader?pack=default',
			  	],
			  },
			  {
			  	test: /\.scss$/,
                    use: [
				  'isomorphic-style-loader',
				  `css-loader?${JSON.stringify({ sourceMap: DEBUG, minimize: !DEBUG })}`,
				  'postcss-loader?pack=sass',
				  'sass-loader',
			  	],
			  },
			  //{
			  //	test: /\.json$/,
     //               use: 'json-loader',
			  //},
			  {
			  	test: /\.html$/,
                    use: 'html-loader',
			  },
			  {
			  	test: /\.md$/,
                    use: 'html!markdown',
			  },
			  {
			  	test: /\.txt$/,
                    use: 'raw-loader',
			  },
			  {
			  	test: /\.(png|jpg|jpeg|gif|svg|woff|woff2)$/,
                    use:[ {
                        loader: 'url-loader',
                        options: {
                            name: DEBUG ? '[path][name].[ext]?[hash]' : '[hash].[ext]',
                            limit: 10000,
                        }
                    }],
			  },
			  {
			  	test: /\.(eot|ttf|wav|mp3)$/,
                    use: [{
                        loader: 'file-loader',
                        options: {
                            name: DEBUG ? '[path][name].[ext]?[hash]' : '[hash].[ext]',
                        }
                    }
                        ],
			  },
			   {
			   	test: /\.tsx?$/,
                      use: [
                          {
                              loader: 'awesome-typescript-loader',
                              options: {
                                  target: 'ES5',
                                  jsx: 'react',
                                  lib: [
                                      "ES5",
                                      "ES2015",
                                      "DOM",
                                      "ScriptHost"
                                  ]
                              }
                          }
                          ]
			   }
            ],
            //preLoaders: [
            //    // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
            //    //{ test: /\.js$/, loader: "source-map-loader" }
            //]
		},
		
        resolve: {
            modules: [
                contextPath,
                'node_modules',
                 path.resolve(__dirname, './fallback'),
                //path.resolve(contextPath, './node_modules'),
                //path.resolve(basePath, './node_modules')
           ],
            plugins: [
                new TsConfigPathsPlugin({
                    configFileName: path.resolve(basePath, './tsconfig.json')
                })
            ],
   //         fallback: [
			//],
			extensions: [ '.webpack.js', '.web.js', '.js', '.jsx', '.json', '.ts', '.tsx'],
			alias: {
			    jquery: "jquery-global",
                  //"react": "preact-compat",
                  //"react-dom": "preact-compat",
                  //'react-addons-css-transition-group': 'rc-css-transition-group'

			},
			//externals: {DedupePlugin
			//	'jquery-global': 'jQuery'
			////	'react': 'React',
			////	'react-dom': 'React.__SECRET_DOM_DO_NOT_USE_OR_YOU_WILL_BE_FIRED'
			//},
		},

		cache: DEBUG,
		//debug: DEBUG,

		stats: {
			colors: true,
			reasons: DEBUG,
			hash: VERBOSE,
			version: VERBOSE,
			timings: true,
			chunks: VERBOSE,
			chunkModules: VERBOSE,
			cached: VERBOSE,
			cachedAssets: VERBOSE,
		}
	};

	//
	// Configuration for the client-side bundle (client.js)
	// -----------------------------------------------------------------------------


	var plugins = [

		// Define free variables
		// https://webpack.github.io/docs/list-of-plugins.html#defineplugin
		new webpack.DefinePlugin(GLOBALS),

		// Emit a file with assets paths
		// https://github.com/sporto/assets-webpack-plugin#options
        new AssetsPlugin({
            path: outputPath,
			filename: 'assets.js',
			processOutput: x => `module.exports = ${JSON.stringify(x)};`,
		}),

		// Assign the module and chunk ids by occurrence count
		// Consistent ordering of modules required if using any hashing ([hash] or [chunkhash])
		// https://webpack.github.io/docs/list-of-plugins.html#occurrenceorderplugin
		//new webpack.optimize.OccurenceOrderPlugin(true),

        
	];
	if (!DEBUG)
		plugins.push(
		  // Minimize all JavaScript output of chunks
		  // https://github.com/mishoo/UglifyJS2#compressor-options
		  new webpack.optimize.UglifyJsPlugin({
		  	compress: {
		  		screw_ie8: true, // jscs:ignore requireCamelCaseOrUpperCaseIdentifiers
		  		warnings: VERBOSE,
		  	},
		  }),

		  // A plugin for a more aggressive chunk merging strategy
		  // https://webpack.github.io/docs/list-of-plugins.html#aggressivemergingplugin
		  new webpack.optimize.AggressiveMergingPlugin()

		);
	
	var devtool = DEBUG ? 'cheap-module-eval-source-map' : false;

	var extcfg={
		entry: entities,

		output: {
			filename: DEBUG ? '[name].js?[chunkhash]' : '[name].[chunkhash].js',
			chunkFilename: DEBUG ? '[name].[id].js?[chunkhash]' : '[name].[id].[chunkhash].js',
		},

		target: 'web',

		// Choose a developer tool to enhance debugging
		// http://webpack.github.io/docs/configuration.html#devtool
		plugins: plugins
	};

	if(!DEBUG)
		extcfg.devtool=devtool;
		

	const clientConfig = extend(true, {}, config,extcfg );
	return clientConfig;
}
