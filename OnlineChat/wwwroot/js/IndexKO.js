var ViewModel = function () {
    var self = this;
    self.Id = ko.observable(); // "id" текущего пользователя 
    self.UserName = ko.observable();// "User Name " текущего пользователя 
    self.AvatarPath = ko.observable(); // ссылка на аватор текущего пользователя
    self.chatList = ko.observableArray([]); // список чатов текущего пользователя 
    self.selectedChatName = ko.observable(); // имя выбраного чата 
    self.selectedChatId = ko.observable();// "id" выбранного чата 
    self.selectedChatListMessages = ko.observableArray([]);// список сообщений в выбранном чате
    self.inputMessage = ko.observable(); // текстовое сообщения для отправки 
    self.onlineUsersList = ko.observableArray([]); // список онлайн пользователей 
    self.searchParam = ko.observable(); // параметры поиска в внутри чата 
    self.searchResult =ko.observableArray([]); // результаты поиска
    self.listChatsUl = ko.observable(true); // HTML элемент для отображения списка чатов 
    self.listOnlineUsersUl = ko.observable(false);// HTML элемент для отображения списка онлайн пользователей
    self.listSearchResultUl = ko.observable(false);// HTML элемент для отображения списка результата поиска
    self.chatBoxDiv = ko.observable(false); // HTML элемент для отображения блока с сообщениями

    self.searchParam.subscribe(function (value) { // поиск внутри чата 
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
    self.clickEnter = function (d, e) { // отправка сообщений по нажатии "Enter"
        if (e.keyCode == 13) {
            jQuery.post("/Home/SendMessage",{message:self.inputMessage,idChat:self.selectedChatId});
            self.inputMessage("");  
        };
    };
    function getUser() { // получения текущего пользователя
        jQuery.getJSON("/Home/GetUser",function (data) {
            self.Id(data.id);
            self.AvatarPath(data.avatarPath);
            self.UserName(data.userName);            
        });
    }

    self.selectedMessage = function(data) { // выбор сообшения и отображения 
        window.location.href = data.id;
    };
    function getChatList() { // получения списка чатов 
        jQuery.getJSON("/Home/GetListChats", 
            function (data) {
                self.chatList(data);
            }
        );
    }
    self.selectedChat = function(chat){// выбор чата для отображения сообщений 
        self.chatBoxDiv(true);
        self.selectedChatName(chat.name);
        self.selectedChatId(chat.id);
        getSelectedChatMessages(chat.id);
    };
    self.selectedUser = function(user){// выбор ползователя и показ в списке чатов 
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
    self.listChat = function(){ // Отображения списка чатов
        self.listOnlineUsersUl(false);
        self.listChatsUl(true);
        getChatList();
    };
    self.onlineUser = function () {// Получения списка онлайн пользователей
        self.onlineUsersList();
        self.listChatsUl(false);
        self.listOnlineUsersUl(true);
        jQuery.getJSON("/Home/GetOnlineUsers",
            function(data) {
                self.onlineUsersList(data);
            });
    };
    function getSelectedChatMessages(id) {// Получения списка сообщений по "id" чата 
        self.selectedChatListMessages();
        jQuery.getJSON("/Home/SelectedChat",
            { idChat: id },
            function(data) {
                self.selectedChatListMessages(data);
            });
    }
    self.sendMessage = function(){//Отправка сообщений 
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