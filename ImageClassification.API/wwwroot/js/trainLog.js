"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/trainLog").build();
const messages = document.getElementById("messages");
const clearButton = document.getElementById('clear-button');

clearButton.onclick = () => messages.innerHTML = null;

connection.on("Log", function (...args) {
    let log = args.join(', ');
    const paragraph = document.createElement('p');
    paragraph.innerHTML = log;
    messages.appendChild(paragraph);
    window.scrollTo(0, document.scrollingElement.scrollHeight);
});

connection.start();
