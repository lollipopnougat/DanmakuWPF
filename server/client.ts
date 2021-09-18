import WebSocket from 'ws';

const ws = new WebSocket('ws://localhost:3000');

interface DanmakuInfo {
    text: string;
    fontSize: string;
    fill: string;
    stroke: string;
    fontFamily: string;
    type: string;
}

let config: DanmakuInfo = {
    text: '发他娘的！',
    fontSize: '36',
    fill: '#00ffff',
    stroke: '#000000',
    fontFamily: '方正姚体',
    type: 'shadow'
};

// 发送
ws.on('open', () => {
    // ws.send('你好');
    // ws.send('我是客户端');
    // ws.send('和服务端通信来了');
    ws.send('/send 发个弹幕试试！');
    ws.send(`/send ${JSON.stringify(config)}`);
    config.text = '我改！';
    config.fontFamily = '微软雅黑';
    config.fontSize = '42';
    config.fill = '#FE315D';
    ws.send(`/send ${JSON.stringify(config)}`);
});

// 接受
ws.on('message', (message) => {
    console.log('服务器发送: %s', message); 
});
