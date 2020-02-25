English | [简体中文](https://github.com/Ke1992/Fiddler-FPlug/blob/master/README-zh_CN.md)
# FPlug
FPlug is a Fiddler plugin that provides a set of tools for web development ([Fiddler Plugin Development Guide](https://github.com/Ke1992/Fiddler-Plug-Example))
# Version
v1.0.4
# Environment
Fiddler version requires 4.6 or higher, recommended version 5.0 or higher（FPlug is based on Fiddler 5.0）, Also requires .NET Framework 4.6.1 or higher
# Install
#### 1、Exe File
Download the [FPlug.exe](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/dist/FPlug.exe) file in the dist folder
#### 2、Custom Installation
Download the dll file in the dist folder and copy it to the Scripts folder in the Fiddler installation directory
# Features
* Host Mapping
* File Mapping
* Https To Http
* Header Replace
* ServerIP
* Disable Cache
* vConsole
* Console Log
* JS Inject
# Basic Configuration Explanation
### Enable/Disable Plugin
![Enable/Disable Plugin](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/switch.gif "Enable/Disable Plugin")
### Project Related
![Project Related](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/item.gif "Project Related")
### Rule Related
![Rule Related](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/rule.gif "Rule Related")
# Features Explanation
### Host Mapping
1、IP: Input the IP address  
2、Port: Not necessary. If it is empty, use the port of the request itself  
3、Url:  
　　(1)、Not limited to domain name, use https://www.example.com/test/index.html as an example  
　　(2)、Can use the full path, example: www.example.com/test  
　　(3)、Can be any part of the url, example: com/test/i  
　　(4)、Support for regular expressions, example: \S*.example.com  
4、Example:
![Host Mapping](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/host.gif "Host Mapping")
### File Mapping
1、Url:  
　　(1)、Not limited to domain name, use https://www.example.com/test/index.html as an example  
　　(2)、Can use the full path, example: www.example.com/test  
　　(3)、Can be any part of the url, example: com/test/i  
　　(4)、Support for regular expressions, example: \S*.example.com  
2、File Path:  
　　(1)、Only correct full path is supported (If the path is incorrect, will be alert an error box！！！)  
　　(2)、If the url has a callback parameter, it will automatically replace the first callback string in the file  
3、Example:
![File Mapping](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/file.gif "File Mapping")
### Https To Http
1、Fiddler's https packet capture must be enabled with this configuration  
2、Url:  
　　(1)、Not limited to domain name, use https://www.example.com/test/index.html as an example  
　　(2)、Can use the full path, example: www.example.com/test  
　　(3)、Can be any part of the url, example: com/test/i  
　　(4)、Support for regular expressions, example: \S*.example.com  
3、Example:
![Https To Http](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/https.gif "Https To Http")
### Header Replace
1、Allow replace the header field of Request or Response   
2、Url:  
　　(1)、Not limited to domain name, use https://www.example.com/test/index.html as an example  
　　(2)、Can use the full path, example: www.example.com/test  
　　(3)、Can be any part of the url, example: com/test/i  
　　(4)、Support for regular expressions, example: \S*.example.com  
3、Key字段: Need to follow the format as User-Agent   
4、Request Example:
![Request Header](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/header_req.gif "Request Header")
5、Response Example:
![Response Header](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/header_res.gif "Response Header")
### ServerIP
1、After opening, it will automatically add a column of ServerIP in the session panel to display the final IP address of the request  
2、Example:  
![ServerIP](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/serverip.gif "ServerIP")
### Disable Cache
1、After opening, it will automatically modify the related fields of the Request and Response headers to disable cache  
　　(1)、Request:  
　　　　a、Delete Expires  
　　　　b、Delete If-None-Match  
　　　　c、Delete If-Modified-Since  
　　　　d、Modify Pragma to no-cache  
　　　　e、Modify Cache-Control to no-cache  
　　(2)、Response:  
　　　　a、Delete Expires  
　　　　b、Modify Pragma to no-cache  
　　　　c、Modify Cache-Control to no-cache  
### vConsole
1、After opening, the vConsole script will be injected into the page   
2、Only text/html is included for Content-Type, and requests starting with &lt;!DOCTYPE or &lt;!doctype are valid   
3、Example:  
![vConsole](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/vconsole.gif "vConsole")
### Console Log
1、After opening, the log panel will be added. At the same time, the JS script will be injected into the page, the console method will be modified, and the post request will be initiated after the log is captured  
2、The order of log output is subject to the serial number  
3、The page that was opened before opening needs to be refreshed in order to capture logs  
4、Only include text/html to the Content-Type, and inject the script into the request starting with &lt;!DOCTYPE or &lt;!doctype  
5、Example:  
![Console Log](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/console.gif "Console Log")
### JS Inject
1、After opening, type the JavaScript script in the input box, and then click Send button, it will automatically inject the corresponding script into the webpage in the proxy  
2、The page that was opened before opening needs to be refreshed in order to respond to the injected script  
3、Only text/html is included for Content-Type, and requests starting with &lt;!DOCTYPE or &lt;!doctype are valid  
4、Will request www.example.com every 2 seconds, please ignore！！！  
5、Example:  
![JS Inject](https://raw.githubusercontent.com/Ke1992/Fiddler-FPlug/master/guide/invade.gif "JS Inject")
