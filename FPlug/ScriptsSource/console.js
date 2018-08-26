//console日志打印脚本
//实现的太简单、后续参考一下vconsole的实现
(function () {
    window.consoleTotalNum = 0;

    //log方法
    window.console.logOld = window.console.log;
    window.console.log = function () {
        //序号增加
        window.consoleTotalNum++;
        //调用原始的方法
        window.console.logOld.apply(this, arguments);
        //发送请求
        sendLogToFiddler('log', arguments);
    };

    //error方法
    window.console.errorOld = window.console.error;
    window.console.error = function () {
        //序号增加
        window.consoleTotalNum++;
        //调用原始的方法
        window.console.errorOld.apply(this, arguments);
        //发送请求
        sendLogToFiddler('error', arguments);
    };

    //warn方法
    window.console.warnOld = window.console.warn;
    window.console.warn = function () {
        //序号增加
        window.consoleTotalNum++;
        //调用原始的方法
        window.console.warnOld.apply(this, arguments);
        //发送请求
        sendLogToFiddler('warn', arguments);
    };

    function sendLogToFiddler(type, param) {
        if (!param.length) {
            return false;
        }

        var data = '',
            xhr = new XMLHttpRequest(),
            url = location.protocol + '//www.example.com',
            nowurl = location.protocol + '//' + location.host + location.pathname;
        
        //遍历拼接数据
        for (var i = 0, len = param.length; i < len; i++) {
            if (param[i] && Object.prototype.toString.call(param[i]) == '[object Object]') {
                data += JSON.stringify(param[i]) + '   ';
            } else if (param[i]) {
                data += param[i].toString() + '   ';
            }
        }

        //发送请求
        xhr.open('POST', url + '?serial=' + window.consoleTotalNum + '&type='+ type +'&nowurl=' + nowurl, true);
        xhr.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        xhr.send(data.trim());
    }
}());