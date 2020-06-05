var ViewModel = function () {
    var self = this;
    self.Id = ko.observable();
    self.UserName = ko.observable();
    self.AvatarPath = ko.observable();
    self.chatList = ko.observableArray([]);
    self.selectedChatName = ko.observable();
    self.selectedChatId = ko.observable();
    self.selectedChatListMessages = ko.observableArray([]);
    self.inputMessage = ko.observable();
    self.onlineUsersList = ko.observableArray([]);
    self.searchParam = ko.observable();
    self.searchResult =ko.observableArray([]);
    self.listChatsUl = ko.observable(true);
    self.listOnlineUsersUl = ko.observable(false);
    self.listSearchResultUl = ko.observable(false);
    self.chatBoxDiv = ko.observable(false);
    self.selectedMessageId = ko.observable();
    self.messageDefaultClass = ("d-flex justify-content-between mb-4");

    self.searchParam.subscribe(function (value) {
        jQuery.getJSON("/Home/SearchMessages",
            { searchParam: value, idChat: self.selectedChatId },
            function (data) {
                console.log(data);
                self.searchResult([]);
                if (data === null | data.length === 0) {
                    self.listChatsUl(true);
                    self.listOnlineUsersUl(false);
                    self.listSearchResultUl(false);
                } else {
                    self.listChatsUl(false);
                    self.listSearchResultUl(true);
                    self.listOnlineUsersUl(false);
                    for (var i in data) {
                        data[i].id = "#" + data[i].id;
                        self.searchResult.push(data[i]);

                    };
                };
            });
    });
    self.clickEnter = function (d, e) {
        if (e.keyCode == 13) {
            jQuery.post("/Home/SendMessage",{message:self.inputMessage,idChat:self.selectedChatId});
            self.inputMessage("");  
        };
    };
    function getUser() {
        jQuery.getJSON("/Home/GetUser",function (data) {
            self.Id(data.id);
            self.AvatarPath(data.avatarPath);
            self.UserName(data.userName);            
        });
    }

    self.selectedMessage = function(data) {
        window.location.href = data.id;
    };
    function getChatList() {
        jQuery.getJSON("/Home/GetListChats", 
            function (data) {
                self.chatList(data); //Put the response in ObservableArray
            }
        );
    }
    self.selectedChat = function(chat){
        self.chatBoxDiv(true);
        self.selectedChatName(chat.name);
        self.selectedChatId(chat.id);
        getSelectedChatMessages(chat.id);
    };
    self.selectedUser = function(user){
        self.selectedChatName(user.userName);
        jQuery.getJSON("/Home/SelectedUserChat",
            { userId: user.id },
            function(data) {
                self.chatBoxDiv(false);
                self.listChatsUl(true);
                getChatList();
                self.listOnlineUsersUl(false);
                self.selectedChatListMessages(data);
            });

    };
    self.listChat = function(){
        self.listOnlineUsersUl(false);
        self.listChatsUl(true);
        getChatList();
    };
    self.onlineUser = function () {
        self.onlineUsersList();
        self.listChatsUl(false);
        self.listOnlineUsersUl(true);
        jQuery.getJSON("/Home/GetOnlineUsers",
            function(data) {
                self.onlineUsersList(data);
            });
    };
    function getSelectedChatMessages(id) {
        self.selectedChatListMessages();
        jQuery.getJSON("/Home/SelectedChat",
            { idChat: id },
            function(data) {
                self.selectedChatListMessages(data);
            });
    }
    self.sendMessage = function(){
        jQuery.post("/Home/SendMessage",{message:self.inputMessage,idChat:self.selectedChatId});
        self.inputMessage("");
    };
    var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
    
    connection.on("Knockout",function (idChat,message) {
        console.log(idChat,message,self.selectedChatId);
        if(self.selectedChatId() === idChat){
            self.selectedChatListMessages.push(message);
        }
    });

    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });
    getUser();
    getChatList();
};  

ko.applyBindings(new ViewModel());