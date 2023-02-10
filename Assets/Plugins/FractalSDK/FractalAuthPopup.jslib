var MyPlugin = {
    OpenBrowserPopup: function (url) {
        const DEFAULT_POPUP_WIDTH_PX = 400;
        const DEFAULT_POPUP_HEIGHT_PX = 600;


        var str = UTF8ToString(url);
        console.log(str)
        var childwin;
        const childname = "Authenticate";
        childwin = window.open(str, childname, 'height=600px, width=400px');

        var timer = setInterval(function () {
            if (childwin.closed) {
                clearInterval(timer);
                //alert('closed');
            }
        }, 1000);

        childwin.focus();
    }
};

mergeInto(LibraryManager.library, MyPlugin);