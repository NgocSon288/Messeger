class SignalRChat {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    }

    // GetSet
    get state() {
        return this.connection.connectionState
    }

    connect(callback) {
        this.connection.start().then(function () {
            if (callback) {
                callback()
            }
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    joinPrivateChat(chatId) {
        this.connection.invoke("JoinPrivateGroup", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    }

    receivePrivateJoinChat(callback) {
        this.connection.on("ReceiveJoinPrivateGroup", function (message) {
            callback(message)
        });
    }

    receiveMessage(callback) {
        this.connection.on("ReceiveMessage", function (message) {
            callback(message)
        });
    }

    sendMessage(chatId, message) {
        const url = '/Chat/CreateMessage'
        const data = {
            chatId: chatId,
            content: message
        } 

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(res => res.json())
            .then(data => console.log(data)) 
    }
}