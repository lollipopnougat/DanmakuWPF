import WebSocket from 'ws';
import UUID from 'short-uuid';

const WServer = WebSocket.Server;
const mPort = 3000;
const wss = new WServer({ port: mPort });

interface ConnectionInfo {
    [key: string]: WebSocket;
}

const BLACK = 30;
const RED = 31;
const GREEN = 32;
const YELLOW = 33;
const BLUE = 34;
const MAGENTA = 35;
const CYAN = 36;
const WHITE = 37;

// 输出颜色
const corlorLog = (text: string, color: number) :void => {
    let tmp = text.split('{}');
    console.log(`${tmp[0]}\x1b[${color}m${tmp[1]}\x1b[0m${tmp[2]}`);
   
};

// 连接池
const conPool: ConnectionInfo = {};
let danmaku: string[] = [];

console.log(`launch server at ${mPort} ...`);

let wsend = (mess: string): number => {
    if (danmaku.length > 0) {
        for(let i of danmaku) {
            console.log(`[SERVER] send [${i}] client  ${mess}`);
            conPool[i].send(mess);
        }
        return 0;
    }
    else {
        corlorLog(`[SERVER] {}error! no danmaku client!{}`, RED);
        return -1;
    }
};

wss.on('connection', (ws) => {
    // 生成一个 uuid 用作用户id
    const uid = UUID.generate().toString();
    conPool[uid] = ws;
    console.log(`[SERVER] client ${uid} has connected!`);
    // 收到消息 事件处理函数
    ws.on('message', (message) => {
        let mess = message.toString().split(' ');
        console.log('[SERVER] recieve [%s]: %s', uid, message);
        if (mess[0] == '/send') {
            let content = mess[1];
            for (let i = 1; i < mess.length; i++) {
                content += mess[i];
            }
            let res = wsend(content);
            let tmp;
            if (res == 0) {
                tmp = `服务器向 弹幕客户端 发送 ${content}`;
            }
            else {
                tmp = '发送失败，弹幕客户端未连接!';
            }
            ws.send(tmp);
        }
        else if (mess[0] == '/danmaku') {
            danmaku.push(uid);
            corlorLog(`[SERVER] {}has recognised danmaku client [${uid}].{}`, GREEN);
            ws.send('/ok');
        }
        else if(mess[0] == '/client') {
            corlorLog(`[SERVER] {}has recognised simple client [${uid}].{}`, CYAN);
            ws.send('/ok');
        }
        else {
            let tmp = '复读--' + message.toString();
            ws.send(tmp);
        }
    });

    ws.on('close', () => {
        let index = danmaku.findIndex((el)=>{return el == uid;})
        if (index != -1) {
            corlorLog(`[SERVER] {}danmaku client [${uid}] is offlining...{}`, YELLOW);
            danmaku.splice(index, 1);
        }
        corlorLog(`[SERVER] {}[${uid}] close{}`, MAGENTA);
        delete conPool[uid];
    });

});

