// Current User
const currentUserId = document.getElementById('userId').value
let currentUserObject = null
let currentFriedId = null
let currentFriendObject = null
let currentChatId = null
const signalRChat = new SignalRChat()
let isSeft = false

// Friend Items
const friendListsEle = document.querySelectorAll('.contacts_body .contacts li')
let frientActiveEle = null

// Header Chat
const chatHeaderEle = document.querySelector('.conversation-header')
const imageUserHeaderEle = chatHeaderEle.querySelector('img.user_img')
const nameUserHeaderEle = chatHeaderEle.querySelector('.user_info span');

// Body Chat
const chatBodyEle = document.querySelector('.msg_card_body')
let lastMessageEle = null
const txtContentMessageEle = document.getElementById('txtMessage')
const btnSubmitMessageEle = document.querySelector('.send_btn')

// SignalR 
signalRChat.receivePrivateJoinChat((message) => {
    console.log('Connected to ' + message)
})

// Gọi Api, get thông tin chat của user với friend được focus
const getChatById = function (chatId, callback) {
    const url = `/Chat/GetChatById/${chatId}`
    fetch(url)
        .then(response => response.json())
        .then(body => {
            callback(body)
        })
}

const scrollToBottomChat = function () {
    chatBodyEle.querySelectorAll('.mess-item:last-child')[0].scrollIntoView()
}

const formatDate = function (date) {
    const d = new Date(date)
    const day = d.getDate()
    const month = d.getMonth() + 1
    const year = d.getFullYear()

    const hour = d.getHours()
    const minute = d.getMinutes()

    const hourf = hour.toString().padStart(2, '0')
    const minutef = minute.toString().padStart(2, '0')

    return `${hourf}:${minute}, ${day} Tháng ${month}, ${year}`;
}

const generatorMessageLeft = function (src, time, content) {
    const divWrapEle = document.createElement('div')
    divWrapEle.classList.add('d-flex', 'justify-content-start', 'mb-4','mess-item')

    const divWrapImageEle = document.createElement('div')
    divWrapImageEle.classList.add('img_cont_msg')
    const imgEle = document.createElement('img')
    imgEle.classList.add('rounded-circle', 'user_img_msg')
    imgEle.src = src

    const divWrapContentEle = document.createElement('div')
    divWrapContentEle.classList.add('msg_cotainer')
    divWrapContentEle.append(content)
    const spanTimeEle = document.createElement('span')
    spanTimeEle.classList.add('msg_time')
    spanTimeEle.textContent = formatDate(time)

    divWrapImageEle.appendChild(imgEle)
    divWrapContentEle.appendChild(spanTimeEle)

    divWrapEle.appendChild(divWrapImageEle)
    divWrapEle.appendChild(divWrapContentEle)

    return divWrapEle
}

const generatorMessageRight = function (src, time, content) {
    const divWrapEle = document.createElement('div')
    divWrapEle.classList.add('d-flex', 'justify-content-end', 'mb-4', 'mess-item')

    const divWrapContentEle = document.createElement('div')
    divWrapContentEle.classList.add('msg_cotainer_send')
    divWrapContentEle.append(content)
    const spanTimeEle = document.createElement('span')
    spanTimeEle.classList.add('msg_time_send')
    spanTimeEle.textContent = formatDate(time)

    const divWrapImageEle = document.createElement('div')
    divWrapImageEle.classList.add('img_cont_msg')
    const imgEle = document.createElement('img')
    imgEle.classList.add('rounded-circle', 'user_img_msg')
    imgEle.src = src

    divWrapImageEle.appendChild(imgEle)
    divWrapContentEle.appendChild(spanTimeEle)

    divWrapEle.appendChild(divWrapContentEle)
    divWrapEle.appendChild(divWrapImageEle)  
    return divWrapEle
}

const generatorMessage = function (chat) {
    chatBodyEle.innerHTML = ''
    const messages = chat.data.messages 

    messages.forEach(item => { 
        const sender = item.sender  // id người  gửi
        let src = 'https://localhost:5000/'
        src += sender === currentUserId ? currentUserObject.appUser.avatarPath : currentFriendObject.appUser.avatarPath
        const content = item.content
        const createdDate = item.createdDate

        if (sender === currentUserId) {
            chatBodyEle.appendChild(generatorMessageRight(src, createdDate, content))
        } else {
            chatBodyEle.appendChild(generatorMessageLeft(src, createdDate, content))
        }
    })
}

const getMessageByIdHandler = function (chat) {
    const userChats = chat.data.userChats
    currentUserObject = userChats.find(x => x.appUserId === currentUserId)
    currentFriendObject = userChats.find(x => x.appUserId === currentFriedId) 

    generatorMessage(chat)

    scrollToBottomChat()
}

// Xử lý khi click vào một Item Friend, hoặc friend đầu tiên
const clickFriendItemHanlder = function (friendItemEle) {
    if (frientActiveEle) {
        frientActiveEle.classList.remove('active')
    }

    frientActiveEle = friendItemEle
    frientActiveEle.classList.add('active')

    currentChatId = friendItemEle.dataset.chatid
    currentFriedId = friendItemEle.dataset.friendid

    imageUserHeaderEle.src = friendItemEle.querySelector('.img_cont img.user_img').src
    nameUserHeaderEle.textContent = friendItemEle.querySelector('.user_info span').textContent

    // Get Chat by id
    getChatById(currentChatId, getMessageByIdHandler)

    // Connection To chatHub  
    const state = signalRChat.state
    if (state !== 'Connected') {
        signalRChat.connect(() => {
            signalRChat.joinPrivateChat(currentChatId)
        });
    } else {
        signalRChat.joinPrivateChat(currentChatId)
    } 
}

// Xử lý khi nhận được message từ (Server - bạn) gửi
signalRChat.receiveMessage(function (message) {

    let src = 'https://localhost:5000/'
    src += isSeft ? currentUserObject.appUser.avatarPath : currentFriendObject.appUser.avatarPath 

    if (isSeft) {
        chatBodyEle.appendChild(generatorMessageRight(src, message.createdDate, message.content))
    } else {
        chatBodyEle.appendChild(generatorMessageLeft(src, message.createdDate, message.content))
    }

    scrollToBottomChat()

    isSeft = false
})

// Submit message khi Enter hoặc nhấn submit
const submitMessageHandler = function () {
    const content = txtContentMessageEle.value
    txtContentMessageEle.focus()

    if (!content) {
        return
    }
    isSeft = true

    signalRChat.sendMessage(currentChatId, content)

    txtContentMessageEle.value = ''
};

// Khởi tạo data khi mới vào trang, cho Friend đầu tiên
(function init() {
    const isEmpty = friendListsEle.length <= 0

    if (!isEmpty) {
        clickFriendItemHanlder(friendListsEle[0])
    }
})()

// Gan sự kiện cho danh sách friend item
friendListsEle.forEach(item => {
    item.addEventListener('click', function (e) {
        clickFriendItemHanlder(this)
    })
})

// Submit Event message
btnSubmitMessageEle.addEventListener('click', function (e) {
    submitMessageHandler()
})

// Key Submit Event Enter message
txtContentMessageEle.addEventListener('keypress', function (e) {
    if (e.charCode === 13) {
        submitMessageHandler()
        e.preventDefault()      // Bỏ dấu Enter dư
    }
})
