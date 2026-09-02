window.auth = {
    setToken: function (token) {
        localStorage.setItem("token", token);
    },

    getToken: function () {
        return localStorage.getItem("token");
    },

    removeToken: function () {
        localStorage.removeItem("token");
    },
    clearToken: function () {
        localStorage.removeItem("token"); // 🔥 THIS MUST EXIST
    }
};