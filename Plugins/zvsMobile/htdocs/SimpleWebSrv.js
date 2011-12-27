var http = require('https');
var fs = require('fs');
var path = require('path');

var options = {
  key: fs.readFileSync('./zvs-key.pem'),
  cert: fs.readFileSync('./zvs-cert.pem')
};


/*var connect = require('connect');
var urlpaser = require('url');

var authCheck = function (req, res, next) {
    url = req.urlp = urlpaser.parse(req.url, true);

    // ####
    // Logout
    if ( url.pathname == "/logout" ) {
      req.session.destroy();
    }

    // ####
    // Is User already validated?
    if (req.session && req.session.auth == true) {
      next(); // stop here and pass to the next onion ring of connect
      return;
    }

    // ########
    // Auth - Replace this simple if with you Database or File or Whatever...
    // If Database, you need a Async callback...
    if ( url.pathname == "/login" && 
         url.query.name == "max" && 
         url.query.pwd == "herewego"  ) {
      req.session.auth = true;
      next();
      return;
    }

    // ####
    // User is not unauthorized. Stop talking to him.
    res.writeHead(403);
    res.end('Sorry you are unauthorized.\n\nFor a login use: /login?name=max&pwd=herewego');
    return;
}

var server = connect.createServer(
      connect.logger({ format: ':method :url' }),
      connect.cookieParser(),
      connect.session({ secret: 'foobar' }),
      connect.bodyParser(),
      authCheck
);
*/
 
http.createServer(options, function (request, response) {
 
     
    var filePath = '.' + request.url;
    if (filePath == './')
        filePath = './index.htm';
        
        console.log(request.method + " " +filePath);
    filePath = getPathFromUrl(filePath);
    
    var extname = path.extname(filePath);
    var contentType = 'text/html';
    switch (extname) {
        case '.js':
            contentType = 'text/javascript';
            break;
        case '.css':
            contentType = 'text/css';
            break;
        case '.manifest':
            contentType = 'text/cache-manifest';
            break;
    }
     
     
    path.exists(filePath, function(exists) {
     
        if (exists) {
            fs.readFile(filePath, function(error, content) {
                if (error) {
                    response.writeHead(500);
                    response.end();
                }
                else {
                    response.writeHead(200, { 'Content-Type': contentType });
                    response.end(content, 'utf-8');
                }
            });
        }
        else {
            response.writeHead(404);
            response.end();
        }
    });
     
}).listen(44311);
 
function getPathFromUrl(url) {
  return url.split("?")[0];
}   
console.log('Server running at ... ');