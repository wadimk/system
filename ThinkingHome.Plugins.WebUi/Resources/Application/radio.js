var lib = require('lib');

var Radio = lib.common.ApplicationBlock.extend({
    initialize: function () {
        this.route = this.getOption('route');
        this.clientMethod = this.getOption('clientMethod');
        this.serverMethod = this.getOption('serverMethod');
        this.reconnectionTimeout = this.getOption('reconnectionTimeout');
    },

    start: function () {
        this.openConnection();
    },

    onBeforeDestroy: function () {
        var connection = this.connection;
        delete this.connection;

        connection && connection.stop();
    },

    openConnection: function () {
        var onDisconnect = this.bind('onDisconnect');

        const connection = new lib.signalrClient.HubConnectionBuilder()
            .withUrl(this.route)
            .configureLogging(lib.signalrClient.LogLevel.Information)
            .build();

        connection.on(this.clientMethod, this.bind('onMessage'));

        connection.onClosed = onDisconnect;
        connection.onDisconnect = onDisconnect;

        connection.start().catch(onDisconnect);
    },

    onDisconnect: function () {
        if (this.connection) {
            setTimeout(this.bind("openConnection"), this.reconnectionTimeout);
        }
    },

    onMessage: function (message) {
        this.trigger(message.channel, message);
        console.log(message);
    },

    sendMessage: function (channel, data) {
        this.connection && this.connection.invoke(this.serverMethod, channel, data);
        console.log(channel, data);
    }
});

module.exports = Radio;
