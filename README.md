# FPlug
FPlug是一个Fiddler插件，提供Web开发中可能使用到的一系列工具
# 版本
v1.0.0
# 环境要求
Fiddler版本要求4.6以上，推荐5.0版本以上<font color="#ff0000">(开发过程中是以Fiddler 5.0版本为基准)</font>，同时要求.NET Framework 4.6.1以上
# 下载安装
下载dist中的两个dll文件，然后复制到Fiddler安装目录中的Scripts文件夹。
# 特性
* HOST映射
* 文件映射
* HTTPS 转 HTTP
* 显示ServerIP
* 禁止缓存
* console日志
* UA模拟
* JS注入
# 使用方法
### HOST映射
1、IP字段：填写映射机器的IP地址即可  
2、端口字段：非必填，填写则替换为指定端口号，为空则使用请求本身的端口号  
3、URL字段：  
　　(1)、不局限于域名，以映射https://www.example.com/test/index.html为例  
　　(2)、可以指定具体的Path，如：www.example.com/test  
　　(3)、可以是URL的任意部分，如：com/test/i  
　　(4)、支持正则表达式，如：\S*.example.com  
4、示例：
![blockchain](https://github.com/Ke1992/FPlug/blob/master/guide/host.gif "HOST映射")
### 文件映射
1、URL字段：  
　　(1)、不局限于域名，以映射https://www.example.com/test/index.html为例  
　　(2)、可以指定具体的Path，如：www.example.com/test  
　　(3)、可以是URL的任意部分，如：com/test/i  
　　4)、支持正则表达式，如：\S*.example.com  
2、文件路径字段：  
　　(1)、仅支持正确的全路径(如果路径不正确将会有错误弹框！！！)  
　　(2)、如果映射的URL中带有callback字段，则会自动替换文件里面第一个callback字符串  
3、示例：
![blockchain](https://github.com/Ke1992/FPlug/blob/master/guide/file.gif "文件映射")
### HTTPS 转 HTTP
1、使用此配置必须开启Fiddler的HTTPS抓包功能  
2、URL字段：  
　　(1)、不局限于域名，以映射https://www.example.com/test/index.html为例  
　　(2)、可以指定具体的Path，如：www.example.com/test  
　　(3)、可以是URL的任意部分，如：com/test/i  
　　(4)、支持正则表达式，如：\S*.example.com  
3、示例：
![blockchain](https://github.com/Ke1992/FPlug/blob/master/guide/https.gif "HTTPS 转 HTTP")
