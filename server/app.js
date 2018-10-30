var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
var Server = require('../server/controllers/ws.controller.js');

var server = new Server("/user", 8001);
server.start();

//var apiRouter = require('./routes/api.route');

var app = express();

app.use(express.static(path.join(__dirname, 'public')));
app.listen(3000);
/*app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());

app.use('/api', apiRouter);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};
  
  // render the error page
  res.status(err.status || 500);
  res.render('error');
});
*/

module.exports = app;

