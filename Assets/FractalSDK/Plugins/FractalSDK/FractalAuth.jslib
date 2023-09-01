var FractalPlugin = {
    SetupFractalEvents: function () {
        addEventListener('message', (e) => {
            const validatedOrigin = (e.origin === 'https://fractal.is' || e.origin === 'https://www.fractal.is');
            if (validatedOrigin) {
                //Process the initial handshake with the popup
                if (e.data.event === 'HANDSHAKE') {
                    window.FractalPopUp.postMessage(
                        {
                            event: 'HANDSHAKE_ACK',
                            payload: {
                                platform: 'REACT_SDK',
                            },
                        },
                        e.origin
                    );
                } else {
                    //Forward all of the other messages to the Unity instance.
                    try {
                        window.unityInstance.SendMessage("FractalLoginButton", "HandlePopupMessage", e.data.event);
                    } catch {
                        unityInstance.SendMessage("FractalLoginButton", "HandlePopupMessage", e.data.event);
                    }
                }
            }
        });
    },

    OpenFractalPopup: function (url) {

        const dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
        const dualScreenTop = window.screenTop !== undefined ? window.screenTop : window.screenY;
        const width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        const height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;
        const systemZoom = width / window.screen.availWidth;
        const left = (width - 400) / 2 / systemZoom + dualScreenLeft
        const top = (height - 600) / 2 / systemZoom + dualScreenTop

        var str = UTF8ToString(url);
        const childname = "Authenticate";
        window.FractalPopUp = window.open(str, childname, `
        scrollbars=yes,
        width=${400 / systemZoom}, 
        height=${600 / systemZoom}, 
        top=${top}, 
        left=${left}
        `);
        window.FractalPopUp.focus();
    },

    CloseFractalPopup: function () {
        window.FractalPopUp.close();
    },

    SendFractalPopup: function (payload) {
    }
};

mergeInto(LibraryManager.library, FractalPlugin);