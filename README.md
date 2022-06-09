# Hsl
使用HslCommunication組件，製作Modbus Master/Slaver的範例

HslCommunication組件相關內容

CopyRight

(C) 2017 - 2020 Richard.Hu, All Rights Reserved

Authorization(授权)

具体可以参照 http://www.hslcommunication.cn/Cooperation

開發平台：VS2019

程式語言：C# WPF

Slaver HMI：Control Panel P07F-N-7"

http://www.ibase-usa.com/english/ProductDetail/IndustrialAutomation/P07F-N

1. 設定
- [ ] NuGet：下載Hslcommunication，V 7.0.1為免費版的最後一版

![1](https://github.com/asgardpz/Hsl/blob/master/Image/5.PNG)

2. 開始
- [ ] Master：開啟 Hsl_Modbus 程式
- [ ] Slaver：VNC 連線到Slaver HMI確認連線狀態，未連線

![1](https://github.com/asgardpz/Hsl/blob/master/Image/1.PNG)

3. 連線
- [ ] Master：按下 Start 後開始接收訊息
- [ ] Slaver：HMI 顯示已連線

![1](https://github.com/asgardpz/Hsl/blob/master/Image/2.PNG)

4. Slaver 輸入
- [ ] Master：顯示 Slaver 訊息
- [ ] Slaver：輸入 訊息

![1](https://github.com/asgardpz/Hsl/blob/master/Image/3.PNG)

5. Master 輸入
- [ ] Master：輸入 訊息
- [ ] Slaver：顯示 Master 訊息

![1](https://github.com/asgardpz/Hsl/blob/master/Image/4.PNG)

以上即可完成簡單的一對一的 Modbus Master/Slaver 測試


