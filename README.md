# DanmakuWPF

桌面弹幕发射器

<div style="text-align: center;">
 <img src="https://www.lollipopnougat.top/picbed/danmakuwpf_logo.png" style="width:256px;" title="DanmakuWPF Logo"/>
</div>

## 介绍

个人尝试对[弹幕姬](https://github.com/wspl/DanmakuChi-Client-CSharp)项目重写的玩具，能够实现发送弹幕，支持编辑字体、颜色、类型、大小，支持全桌面取色，采用WebSocket与服务器进行通信，服务器端支持多客户端登录以及发消息

![截图](https://www.lollipopnougat.top/picbed/danmakuwpf_mainwindow.jpg)

## 特点

- 客户端
  - `.net 5`
  - WPF
  - 使用websocket通信
  - 弹幕字体、颜色、样式、大小选择
  - 使用了[HandyControl](https://handyorg.github.io/handycontrol/)
  - ALT+O 全局热键取色
  - 弹幕可以通过服务器广播给所有的弹幕客户端
  - 日志功能

- 服务端
  - 简易(少于100行)
  - nodejs平台
  - 可以多人加入使用(勉强算弹幕聊天软件)
  - 服务端采用 `typescript` 编写
  - 彩色日志(指打印到控制台)

### 特性(?)

1. 连接 websocket 服务器后此客户端程序会给服务器发送文本 "/danmaku"

2. 支持服务器发来的json字符串格式为

```json
{
    "text": "文本",
    "fontSize": "24",
    "fill": "#ffffff",
    "stroke": "#000000",
    "fontFamily": "黑体",
    "type": "shadow"
}
```

#### 注意

fontSize取值在 `[16, 42]`

`fill` 表示字体的颜色，仅支持16进制颜色值

`stroke` 表示描边的颜色

`type` 取值有两种 "outline" (描边) 和 "shadow" (阴影)

json要删除多余空格和换行符(即都写在一行，~~其实是我懒得用json反序列化，就写了几个正则提取的值~~)

3. websocket 客户端给桌面弹幕发射器发送弹幕

`websocket` 客户端可以通过特殊的命令告知服务器给所有的弹幕客户端发送特定弹幕，字符串格式为: "/send {2中的json格式}"(可参考`server/client.ts`)

另外所谓的 `转发到服务器` 按钮也是采用这种方法将弹幕发给服务器的

4. 关闭客户端自动在当前目录输出一个 `danmakuwpf.log` 日志文件，里面记录了上次打开此程序的日志信息

### 运行服务器

```bash
npm i && npm run dev
```

服务器默认监听3000端口，有其他需要可以直接修改 `index.ts`

### 参考

客户端程序

[DanmakuChi-Client-CSharp](https://github.com/wspl/DanmakuChi-Client-CSharp)