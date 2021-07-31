const api_call = {
    post: function (url, content, token, callback, errorCallback) {
        const xmlhttp = new XMLHttpRequest();
        xmlhttp.onreadystatechange = function () {
            if (this.readyState === 4 && this.status >= 200 && this.status < 300 && callback) {
                callback(this.responseText);
            } else if (this.readyState === 4 && errorCallback) {
                if (this.status == 401) {
                    Logout_Application();
                    return;
                }
                //commented by satyendra to show original error message
                //errorCallback(this.statusText);
                errorCallback(this.responseText);
            }
        };
        xmlhttp.onerror = function () {
            if (errorCallback) {
                errorCallback();
            }
        };
        xmlhttp.open("POST", url, true);
        xmlhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        if (token) {
            xmlhttp.setRequestHeader("Authorization", "Bearer " + token);
        }
        xmlhttp.send(JSON.stringify(content));
    },
    get: function (url, callback, errorCallback, token) {
        const xmlhttp = new XMLHttpRequest();
        xmlhttp.onreadystatechange = function () {
            if (this.readyState === 4 && this.status >= 200 && this.status < 300 && callback) {
                callback(this.responseText);
            } else if (this.readyState === 4 && errorCallback) {
                errorCallback();
            }
        };
        xmlhttp.onerror = function () {
            if (errorCallback) {
                errorCallback();
            }
        };
        xmlhttp.open("GET", url, true);
        if (token) {
            xmlhttp.setRequestHeader("Authorization", "Bearer " + token);
        }
        xmlhttp.send();
    },
    delete: function (url, callback, errorCallback, token) {
        const xmlhttp = new XMLHttpRequest();
        xmlhttp.onreadystatechange = function () {
            if (this.readyState === 4 && this.status >= 200 && this.status < 300 && callback) {
                callback(this.responseText);
            } else if (this.readyState === 4 && errorCallback) {
                errorCallback();
            }
        };
        xmlhttp.onerror = function () {
            if (errorCallback) {
                errorCallback();
            }
        };
        xmlhttp.open("DELETE", url, true);
        if (token) {
            xmlhttp.setRequestHeader("Authorization", "Bearer " + token);
        }
        xmlhttp.send();
    }
};